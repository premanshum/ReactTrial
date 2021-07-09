using System.Collections.Generic;

namespace Prem.N3.UserSubscription.Processors.Entities
{
    public class SubscriptionDetailCommand
    {
        public int ColCoId { get; set; }

        public string PayerNumber { get; set; }

        public IList<string> AccountNumbers { get; set; }
    }
}
