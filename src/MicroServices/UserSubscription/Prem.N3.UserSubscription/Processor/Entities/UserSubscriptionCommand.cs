
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Prem.N3.UserSubscription.Processors.Entities
{
    public class UserSubscriptionCommand
    {
        [JsonProperty(PropertyName = "RequestID")]
        public string RequestID { get; set; }

        [JsonProperty(PropertyName = "UserId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "Subscriptions")]
        public IList<SubscriptionDetailCommand> Subscriptions { get; set; }

        [JsonProperty(PropertyName = "UiNotifications")]
        public IList<int> UiNotifications { get; set; }

        [JsonProperty(PropertyName = "EmailNotifications")]
        public IList<int> EmailNotifications { get; set; }

        public string Status { get; set; }
    }
}
