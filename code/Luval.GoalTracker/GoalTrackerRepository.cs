using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.GoalTracker.Entities;
using System;
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

        public GoalTrackerRepository(IUnitOfWorkFactory unitOfWorkFactory)
        {
            DefinitionUoW = unitOfWorkFactory.Create<GoalDefinition, string>();
            EntryUoW = unitOfWorkFactory.Create<GoalEntry, string>();
        }

        public Task CreateOrUpdateGoalAsync(GoalDefinition definition, string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.AddOrUpdateAndSaveAuditEntityAsync(definition, i => i.Name == definition.Name, userId, cancellationToken);
        }

        public Task CreateOrUpdateEntryAsync(GoalEntry entry, string userId, CancellationToken cancellationToken)
        {
            entry.GoalDateTime = entry.GoalDateTime.Date; //make sure time is trimmed
            return EntryUoW.AddOrUpdateAndSaveAuditEntityAsync(entry, e => e.GoalDateTime == entry.GoalDateTime, userId, cancellationToken);
        }

        public async Task CreateOrUpdateEntryAsync(IEnumerable<GoalEntry> entries, string userId, CancellationToken cancellationToken)
        {
            foreach (var entry in entries)
            {
                await CreateOrUpdateEntryAsync(entry, userId, cancellationToken);
            }
        }

        public Task<IEnumerable<GoalDefinition>> GetGoalsByFrequencyAsync(GoalFrequency frequency, string userId, CancellationToken cancellationToken)
        {
            return DefinitionUoW.Entities.Query.GetAsync(i => i.Frequency == frequency && !i.IsInactive && i.CreatedByUserId == userId, cancellationToken);
        }
    }
}
