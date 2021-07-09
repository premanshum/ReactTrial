using Newtonsoft.Json;
using Prem.N3.UserSubscription.Helper;
using System;
using System.Collections.Generic;
using System.Text;

namespace Prem.N3.UserSubscription.Processors.Entities
{
	[CosmoDBContainer(Name = nameof(AccountSubscriptions), PartitionKey = "Payer")]
	public class AccountSubscriptions
	{
		public AccountSubscriptions()
		{
			Id = Guid.NewGuid().ToString();
			UUIDs = new List<string>();
			Active = 1;
		}

		public AccountSubscriptions(string payer, string account, string userId): this()
		{
			Payer = payer;
			Account = account;
			UUIDs.Add(userId);
		}

		[JsonProperty(PropertyName = "id", Order = 0)]
		public string Id { get; set; }

		[JsonProperty(PropertyName = "Account", Order = 1)]
		public string Account { get; set; }

		[JsonProperty(PropertyName = "Payer", Order = 2)]
		public string Payer { get; set; }

		[JsonProperty(PropertyName = "Active", Order = 3)]
		public int? Active { get; set; }

		[JsonProperty(PropertyName = "UUIDs", Order = 4)]
		public List<string> UUIDs { get; set; }
	}
}
