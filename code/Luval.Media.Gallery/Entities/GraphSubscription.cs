using Luval.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luval.Media.Gallery.Entities
{
    public class GraphSubscription : BaseAuditEntity
    {
        public string SubscriptionId { get; set; }

        /// <summary>
        /// The user's ID associated with the subscription
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// The tenant ID of the organization
        /// </summary>
        public string TenantId { get; set; }

        /// <summary>
        /// The client state set in the subscription
        /// </summary>
        public string ClientState { get; set; }

        public string ChangeType { get; set; }

        public override string ToString()
        {
            return string.Format("UserId: {0} ChangeType: {1} Client State: {2}", UserId, ChangeType, ClientState);
        }
    }
}
