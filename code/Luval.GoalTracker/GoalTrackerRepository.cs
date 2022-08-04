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
            var items = await DefinitionUoW.Entities.QueryAsync(i => i.CreatedByUserId == userId, i => i.Sort, false, cancellationToken);
            var weekStart = DateTime.Today.WeekStart();
            foreach (var item in items.Where(i => i.CurrentWeek != weekStart))
            {
                await UpdateProgressAsync(item, userId, cancellationToken);
            }
            foreach (var item in items)
            {
                var today = weekStart.AppDateTime().Date;
                var todayValue = (await EntryUoW.Entities?.QueryAsync(i => i.HabitDefinitionId == item.Id && i.EntryDateTime == today, i => i.EntryDateTime, true, cancellationToken)).FirstOrDefault();
                if (todayValue != null) item.Entries?.Add(todayValue);
            }
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
            def.Entries = entries.OrderByDescending(i => i.EntryDateTime).ToList();
            return def;
        }

        public async Task<HabitDefinition> UpdateProgressAsync(HabitDefinition definition, string userId, CancellationToken cancellationToken)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            var today = DateTime.Today.AppDateTime().Date;
            var startofWeek = today.WeekStart();
            var startofMonth = today.MonthStart();
            var startofYear = today.YearStart();

            var yearlyValue = await EntryUoW.Entities.QueryAsync(CustomCommands["Custom"](definition.Id, startofYear, today), cancellationToken);
            var monthlyValue = await EntryUoW.Entities.QueryAsync(CustomCommands["Custom"](definition.Id, startofMonth, today), cancellationToken);
            var weeklyValue = await EntryUoW.Entities.QueryAsync(CustomCommands["Custom"](definition.Id, startofWeek, today), cancellationToken);

            definition.YearlyProgress = GetProgress(yearlyValue, definition.YearlyTarget);
            definition.MonthlyProgress = GetProgress(monthlyValue, definition.MonthlyTarget);
            definition.WeeklyProgress = GetProgress(weeklyValue, definition.WeeklyTarget);
            definition.UpdatedByUserId = userId;
            definition.UtcUpdatedOn = DateTime.UtcNow;
            definition.CurrentWeek = startofWeek;

            await DefinitionUoW.UpdateAndSaveAsync(definition, cancellationToken);

            return definition;
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
    }
}
