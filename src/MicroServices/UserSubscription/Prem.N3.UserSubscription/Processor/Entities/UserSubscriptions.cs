using Newtonsoft.Json;
using Prem.N3.UserSubscription.Helper;
using System;
using System.Collections.Generic;

namespace Prem.N3.UserSubscription.Processors.Entities
{
    [CosmoDBContainer(Name = nameof(UserSubscriptions), PartitionKey = "UUID")]
    public class UserSubscriptions
    {
        public UserSubscriptions()
        {
            Id = Guid.NewGuid().ToString();
            Events = new List<Event>();
            Type = "Subscription";
        }

        public UserSubscriptions(string userId, List<Event> events) : this()
        {
            UUId = userId;
            Events = events;
        }

        [JsonProperty(PropertyName = "id", Order = 0)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "UUID", Order = 1)]
        public string UUId { get; set; }

        [JsonProperty(PropertyName = "Type", Order = 2)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "Events", Order = 3)]
        public List<Event> Events { get; set; }

        [JsonProperty(PropertyName = "PreferredLanguage", Order = 4)]
        public string PreferredLanguage { get; set; }

        [JsonProperty(PropertyName = "DateFormat", Order = 5)]
        public string DateFormat { get; set; }

        [JsonProperty(PropertyName = "TimeFormat", Order = 6)]
        public string TimeFormat { get; set; }
    }

    public class Event
    {
        public Event()
        {
            ChannelType = new List<int>();
        }

        public Event(int eventType, int channelType)
        {
            EventType = eventType;

            ChannelType = new List<int>();
            ChannelType.Add(channelType);
        }

        [JsonProperty(PropertyName = "EventType", Order = 0)]
        public int EventType { get; set; }

        [JsonProperty(PropertyName = "ChannelType", Order = 1)]
        public List<int> ChannelType { get; set; }
    }
}
