using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.Data.Sql;
using Luval.GoalTracker.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.GoalTracker
{
    public class GoalTrackerRepository
    {
        protected IUnitOfWork<GoalDefinition, string> DefinitionUoW { get; private set; }
        protected IUnitOfWork<GoalEntry, string> EntryUoW { get; private set; }
        protected IUnitOfWork<GoalViewModel, string> ViewUoW { get; private set; }

        public GoalTrackerRepository(IUnitOfWorkFactory unitOfWorkFactory)
        {
            DefinitionUoW = unitOfWorkFactory.Create<GoalDefinition, string>();
            EntryUoW = unitOfWorkFactory.Create<GoalEntry, string>();
            ViewUoW = unitOfWorkFactory.Create<GoalViewModel, string>();
        }

        public Task CreateOrUpdateGoalAsync(GoalDefinition definition, string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.AddOrUpdateAndSaveAuditEntityAsync(definition, i => i.Name == definition.Name, userId, cancellationToken);
        }

        public async Task CreateOrUpdateEntryAsync(GoalEntry entry, string userId, CancellationToken cancellationToken)
        {
            entry.GoalDateTime = entry.GoalDateTime.Date; //make sure time is trimmed
            await EntryUoW.AddOrUpdateAndSaveAuditEntityAsync(entry, e => e.GoalDefinitionId == entry.GoalDefinitionId && e.GoalDateTime == entry.GoalDateTime, userId, cancellationToken);
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
            return DefinitionUoW.Entities.Query.GetAsync(i => i.Frequency == frequency && i.CreatedByUserId == userId, cancellationToken);
        }

        public async Task<IEnumerable<GoalViewModel>> GetGoalViewAsync(string userId, CancellationToken cancellationToken)
        {
            var items = await ViewUoW.Entities.Query.GetAsync(i => i.CreatedByUserId == userId, cancellationToken);
            return items;
        }

        public Task<GoalDefinition> GetGoalAsync(string id, CancellationToken cancellationToken)
        {
            return DefinitionUoW.Entities.Query.GetAsync(id, cancellationToken);
        }

        public async Task<GoalDefinition> UpdateProgressAsync(GoalDefinition goal, string userId, CancellationToken cancellationToken)
        {
            if (goal == null) throw new ArgumentNullException(nameof(goal));
            var today = GetToday().Date;
            var startofWeek = StartOfWeek().Date;
            var startofMonth = new DateTime(today.Year, today.Month, 1);
            var startofYear = new DateTime(today.Year, 1, 1);
            var yearlyValue = await EntryUoW.Entities.Query.GetRawAsync(new SqlQueryCommand("SELECT SUM(NumericValue) As Value FROM GoalEntry WHERE GoalDefinitionId = {0} AND GoalDateTime BETWEEN {1} AND {2}".FormatSql(goal.Id, startofYear, today)), cancellationToken);
            var monthlyValue = await EntryUoW.Entities.Query.GetRawAsync(new SqlQueryCommand("SELECT SUM(NumericValue) As Value FROM GoalEntry WHERE GoalDefinitionId = {0} AND GoalDateTime BETWEEN {1} AND {2}".FormatSql(goal.Id, startofMonth, today)), cancellationToken);
            var weeklyValue = await EntryUoW.Entities.Query.GetRawAsync(new SqlQueryCommand("SELECT SUM(NumericValue) As Value FROM GoalEntry WHERE GoalDefinitionId = {0} AND GoalDateTime BETWEEN {1} AND {2}".FormatSql(goal.Id, startofWeek, today)), cancellationToken);
            goal.YearlyProgress = GetProgress(yearlyValue, goal.YearlyTarget);
            goal.MonthlyProgress = GetProgress(monthlyValue, goal.MonthlyTarget);
            goal.WeeklyProgress = GetProgress(weeklyValue, goal.WeeklyTarget);
            goal.UpdatedByUserId = userId;
            goal.UtcUpdatedOn = DateTime.UtcNow;
            await DefinitionUoW.UpdateAndSaveAsync(goal, cancellationToken);
            return goal;
        }

        private double? GetProgress(IEnumerable<IDictionary<string, object>> items, double? target)
        {
            if (target == null) return null;
            var value = GetValue(items);
            if (value == null) return 0d;
            return Math.Round((value.Value / target.Value) * 100, 0);
        }

        private double? GetValue(IEnumerable<IDictionary<string, object>> items)
        {
            if (items == null || !items.Any()) return null;
            var item = items.First();
            if (!item.ContainsKey("Value")) return null;
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
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time"));
        }
    }
}
