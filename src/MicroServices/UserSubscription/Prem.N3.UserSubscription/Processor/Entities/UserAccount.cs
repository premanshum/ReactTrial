using Newtonsoft.Json;
using Prem.N3.UserSubscription.Helper;
using System;
using System.Collections.Generic;

namespace Prem.N3.UserSubscription.Processors.Entities
{
    [CosmoDBContainer(Name = nameof(UserSubscriptions), PartitionKey = "UUID")]
    public class UserAccount
    {
        public UserAccount()
        {
            Id = Guid.NewGuid().ToString();
            Payers = new List<Payer>();
            Type = "Accounts";
        }

        public UserAccount(string userId, List<Payer> payers) : this()
        {
            UUId = userId;
            Payers = payers;
        }

        [JsonProperty(PropertyName = "id", Order = 0)]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "UUID", Order = 0)]
        public string UUId { get; set; }

        [JsonProperty(PropertyName = "Type", Order = 1)]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "Payers", Order = 2)]
        public List<Payer> Payers { get; set; }
    }

    public class Payer
    {
        public Payer()
        {
            Accounts = new List<string>();
        }

        public Payer(string payerNumber, List<string> accounts)
        {
            PayerNumber = payerNumber;
            Accounts = accounts;
        }

        [JsonProperty(PropertyName = "PayerNumber", Order = 0)]
        public string PayerNumber { get; set; }

        [JsonProperty(PropertyName = "Accounts", Order = 1)]
        public List<string> Accounts { get; set; }
    }
}
