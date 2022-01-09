using Luval.DataStore;
using Luval.DataStore.Database;
using Luval.DataStore.Entities;
using Luval.DataStore.Extensions;
using Luval.GoalTracker.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.GoalTracker
{
    public class GoalTrackerRepository
    {
        protected IUnitOfWork<GoalDefinition> DefinitionUoW { get; private set; }
        protected IUnitOfWork<GoalEntry> EntryUoW { get; private set; }
        protected IUnitOfWork<GoalViewModel> ViewUoW { get; private set; }
        protected Dictionary<string, Func<string, DateTime, DateTime, IDataCommand>> CustomCommands { get; private set; }

        public GoalTrackerRepository(IUnitOfWorkFactory unitOfWorkFactory) : this(unitOfWorkFactory, null)
        {

        }

        public GoalTrackerRepository(IUnitOfWorkFactory unitOfWorkFactory, Dictionary<string, Func<string, DateTime, DateTime, IDataCommand>> customCommands)
        {
            DefinitionUoW = unitOfWorkFactory.Create<GoalDefinition>();
            EntryUoW = unitOfWorkFactory.Create<GoalEntry>();
            ViewUoW = unitOfWorkFactory.Create<GoalViewModel>();
            CustomCommands = customCommands ?? new Dictionary<string, Func<string, DateTime, DateTime, IDataCommand>>() {
                {
                    "Custom",
                    (id, start, today) => {
                        return new SqlDataCommand("SELECT SUM(NumericValue) As Value FROM GoalEntry WHERE GoalDefinitionId = {0} AND GoalDateTime BETWEEN {1} AND {2}".FormatSql(id,start, today));
                    }
                }
            };
        }

        public Task CreateOrUpdateGoalAsync(GoalDefinition definition, string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.AddOrUpdateAndSaveAsync(definition, userId, i => i.Name == definition.Name, cancellationToken);
        }

        public async Task CreateOrUpdateEntryAsync(GoalEntry entry, string userId, CancellationToken cancellationToken)
        {
            entry.GoalDateTime = entry.GoalDateTime.Date; //make sure time is trimmed
            await EntryUoW.AddOrUpdateAndSaveAsync(entry, userId, e => e.GoalDefinitionId == entry.GoalDefinitionId && e.GoalDateTime == entry.GoalDateTime, cancellationToken);
            var goal = await GetGoalAsync(entry.GoalDefinitionId, cancellationToken);
            await UpdateProgressAsync(goal, userId, cancellationToken);
        }

        public async Task CreateOrUpdateEntryAsync(IEnumerable<GoalEntry> entries, string userId, CancellationToken cancellationToken)
        {
            foreach (var entry in entries)
            {
                await CreateOrUpdateEntryAsync(entry, userId, cancellationToken);
            }
        }

        public Task<IEnumerable<GoalDefinition>> GetGoalsByFrequencyAsync(string frequency, string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.Entities.QueryAsync(i => i.Frequency == frequency && i.CreatedByUserId == userId, null, null, cancellationToken);
        }

        public Task<IEnumerable<GoalDefinition>> GetGoalsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.Entities.QueryAsync(i => i.CreatedByUserId == userId, null, null, cancellationToken);
        }

        public async Task<IEnumerable<GoalViewModel>> GetGoalViewAsync(string userId, CancellationToken cancellationToken)
        {
            var items = await ViewUoW.Entities.QueryAsync(i => i.CreatedByUserId == userId, null, null, cancellationToken);
            return items;
        }

        public async Task<GoalDefinition> GetGoalAsync(string id, CancellationToken cancellationToken)
        {
            return (await DefinitionUoW.Entities.QueryAsync(i => i.Id == id, null, null, cancellationToken)).FirstOrDefault();
        }

        public async Task<GoalDefinition> UpdateProgressAsync(GoalDefinition goal, string userId, CancellationToken cancellationToken)
        {
            if (goal == null) throw new ArgumentNullException(nameof(goal));
            var today = GetToday().Date;
            var startofWeek = StartOfWeek().Date;
            var startofMonth = new DateTime(today.Year, today.Month, 1);
            var startofYear = new DateTime(today.Year, 1, 1);

            var yearlyValue = await EntryUoW.Entities.QueryAsync(CustomCommands["Custom"](goal.Id, startofYear, today), cancellationToken);
            var monthlyValue = await EntryUoW.Entities.QueryAsync(CustomCommands["Custom"](goal.Id, startofMonth, today), cancellationToken);
            var weeklyValue = await EntryUoW.Entities.QueryAsync(CustomCommands["Custom"](goal.Id, startofWeek, today), cancellationToken);

            goal.YearlyProgress = GetProgress(yearlyValue, goal.YearlyTarget);
            goal.MonthlyProgress = GetProgress(monthlyValue, goal.MonthlyTarget);
            goal.WeeklyProgress = GetProgress(weeklyValue, goal.WeeklyTarget);
            goal.UpdatedByUserId = userId;
            goal.UtcUpdatedOn = DateTime.UtcNow;
            await DefinitionUoW.UpdateAndSaveAsync(goal, cancellationToken);
            return goal;
        }

        private double? GetProgress(IEnumerable<IDataRecord> items, double? target)
        {
            if (target == null) return null;
            var value = GetValue(items);
            if (value == null) return 0d;
            var result = Math.Round((value.Value / target.Value) * 100, 0);
            if (result > 100) result = 100d;
            return result;
        }

        private double? GetValue(IEnumerable<IDataRecord> items)
        {
            if (items == null || !items.Any()) return null;
            var item = items.First();
            if (item["Value"].IsNullOrDbNull()) return null;
            return Convert.ToDouble(item["Value"]);
        }

        private DateTime StartOfWeek()
        {
            var today = GetToday().Date;
            switch (today.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    return today.AddDays(-6);
                case DayOfWeek.Monday:
                    return today;
                case DayOfWeek.Tuesday:
                    return today.AddDays(-1);
                case DayOfWeek.Wednesday:
                    return today.AddDays(-2);
                case DayOfWeek.Thursday:
                    return today.AddDays(-3);
                case DayOfWeek.Friday:
                    return today.AddDays(-4);
                case DayOfWeek.Saturday:
                    return today.AddDays(-5);
                default:
                    return today;
            }

        }
        private DateTime GetToday()
        {
            return DateTime.UtcNow.AddHours(-6);
        }
    }
}
