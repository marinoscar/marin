using Luval.Data.Extensions;
using Luval.Data.Interfaces;
using Luval.Media.Gallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class SubscriptionRepository
    {
        protected IUnitOfWork<GraphSubscription, string> UnitOfWork { get; private set; }

        public SubscriptionRepository(IUnitOfWork<GraphSubscription, string> unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        public Task<int> CreateAsync(GraphSubscription subscription, CancellationToken cancellationToken)
        {
            return UnitOfWork.AddAndSaveAsync(subscription, cancellationToken);
        }

        public async Task<GraphSubscription> GetAsync(string subscriptionId, CancellationToken cancellationToken)
        {
            return (await UnitOfWork.Entities.Query.GetAsync(i => i.SubscriptionId == subscriptionId, cancellationToken)).FirstOrDefault();
        }
    }
}
