using Luval.DataStore;
using Luval.DataStore.Database;
using Luval.DataStore.Entities;
using Luval.DataStore.Extensions;
using Luval.GoalTracker.Entities;
using Luval.GoalTracker.Extensions;
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
        protected IUnitOfWork<HabitDefinition> DefinitionUoW { get; private set; }
        protected IUnitOfWork<HabitEntry> EntryUoW { get; private set; }
        protected Dictionary<string, Func<string, DateTime, DateTime, IDataCommand>> CustomCommands { get; private set; }

        public GoalTrackerRepository(IUnitOfWorkFactory unitOfWorkFactory) : this(unitOfWorkFactory, null)
        {

        }

        public GoalTrackerRepository(IUnitOfWorkFactory unitOfWorkFactory, Dictionary<string, Func<string, DateTime, DateTime, IDataCommand>> customCommands)
        {
            DefinitionUoW = unitOfWorkFactory.Create<HabitDefinition>();
            EntryUoW = unitOfWorkFactory.Create<HabitEntry>();
            CustomCommands = customCommands ?? new Dictionary<string, Func<string, DateTime, DateTime, IDataCommand>>() {
                {
                    "Custom",
                    (id, start, today) => {
                        return new SqlDataCommand("SELECT SUM(NumericValue) As Value FROM HabitEntry WHERE HabitDefinitionId = {0} AND EntryDateTime BETWEEN {1} AND {2}".FormatSql(id,start, today));
                    }
                }
            };
        }

        public Task CreateOrUpdateGoalAsync(HabitDefinition definition, string userId, CancellationToken cancellationToken)
        {
            return BatchCreateOrUpdateGoalAsync(new[] { definition }, userId, cancellationToken);
        }

        public Task BatchCreateOrUpdateGoalAsync(IEnumerable<HabitDefinition> definitions, string userId, CancellationToken cancellationToken)
        {
            var def = definitions.FirstOrDefault();
            return DefinitionUoW.AddOrUpdateAndSaveAsync<HabitDefinition, string>(definitions, userId, i => i.Name == def.Name && i.CreatedByUserId == userId, cancellationToken);
        }


        public async Task CreateOrUpdateEntryAsync(HabitEntry entry, string userId, CancellationToken cancellationToken)
        {
            entry.EntryDateTime = entry.EntryDateTime.Date; //make sure time is trimmed
            await EntryUoW.AddOrUpdateAndSaveAsync<HabitEntry, string>(entry, userId, e => e.HabitDefinitionId == entry.HabitDefinitionId && e.EntryDateTime == entry.EntryDateTime, cancellationToken);
            var goal = await GetHabitDefinitionAndTodayEntryAsync(entry.HabitDefinitionId, cancellationToken);
            await UpdateProgressAsync(goal, userId, cancellationToken);
        }

        public async Task CreateOrUpdateEntryAsync(IEnumerable<HabitEntry> entries, string userId, CancellationToken cancellationToken)
        {
            foreach (var entry in entries)
            {
                await CreateOrUpdateEntryAsync(entry, userId, cancellationToken);
            }
        }

        public Task<IEnumerable<HabitDefinition>> GetGoalsByFrequencyAsync(string frequency, string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.Entities.QueryAsync(i => i.Frequency == frequency && i.CreatedByUserId == userId, null, null, cancellationToken);
        }

        public Task<IEnumerable<HabitDefinition>> GetGoalsByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.Entities.QueryAsync(i => i.CreatedByUserId == userId, null, null, cancellationToken);
        }

        public async Task<IEnumerable<HabitDefinition>> GetHabitsAsync(string userId, CancellationToken cancellationToken)
        {
            var items = await DefinitionUoW.Entities.QueryAsync(i => i.CreatedByUserId == userId, null, null, cancellationToken);
            return items;
        }

        public async Task<HabitEntryModelView> GetHabitEntryViewModelWithTodayEntryAsync(string id, CancellationToken cancellationToken)
        {
            var definition = await GetHabitDefinitionAndTodayEntryAsync(id, cancellationToken);
            var model = new HabitEntryModelView()
            {
                DefinitionId = definition.Id,
                Question = definition.Question,
                Type = definition.Type,
                UnitOfMeasure = definition.UnitOfMeasure,
                EntryDateTime = new DateTime().AppDateTime().Date
            };
            if (definition.Entries != null && definition.Entries.Any())
            {
                var entry = definition.Entries.First();
                model.NumberValue = entry.NumericValue;
                model.Notes = entry.Notes;
                model.Difficulty = entry.Difficulty;
                model.EntryDateTime = entry.EntryDateTime;
            }
            return model;
        }

        public async Task<HabitDefinition> GetHabitDefinitionAndTodayEntryAsync(string id, CancellationToken cancellationToken)
        {
            var def = (await DefinitionUoW.Entities.QueryAsync(i => i.Id == id, null, null, cancellationToken)).FirstOrDefault();
            if (def == null) throw new ArgumentException("The provided id returns no results");
            var today = new DateTime().AppDateTime().Date;
            var entries = await EntryUoW.Entities.QueryAsync(i => i.HabitDefinitionId == def.Id && i.EntryDateTime == today, null, null, cancellationToken);
            def.Entries = entries.ToList();
            return def;
        }

        public async Task<HabitDefinition> GetHabitDefinitionAndHistoryEntryAsync(string id, CancellationToken cancellationToken)
        {
            var def = (await DefinitionUoW.Entities.QueryAsync(i => i.Id == id, null, null, cancellationToken)).FirstOrDefault();
            if (def == null) throw new ArgumentException("The provided id returns no results");
            var today = new DateTime().AppDateTime().Date;
            var start = today.AddDays(-15).Date;
            var entries = await EntryUoW.Entities.QueryAsync(i => i.HabitDefinitionId == def.Id && i.EntryDateTime <= today && i.EntryDateTime >= start, 
                o => o.EntryDateTime, true, cancellationToken);
            def.Entries = entries.ToList();
            return def;
        }

        public async Task<HabitDefinition> UpdateProgressAsync(HabitDefinition goal, string userId, CancellationToken cancellationToken)
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
