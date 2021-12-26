using Luval.Data.Interfaces;
using Luval.Media.Gallery.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.OneDrive
{
    public class SubscriptionRepository
    {
        protected IUnitOfWork<GraphSubscription, string> UnitOfWork { get; private set; }
    }
}
