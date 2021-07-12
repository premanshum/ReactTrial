using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prem.N3.UserSubscription.Helper;
using Prem.N3.UserSubscription.Processors;
using Prem.N3.UserSubscription.Processors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using UserSubscriptionEntity = Prem.N3.UserSubscription.Processors.Entities.UserSubscriptions;

namespace Prem.N3.UserSubscription.Test
{
	[TestClass]
	public class ProcessorTests
	{

		ICosmosDbRepository<UserSubscriptionEntity> _UserSubscriptionRepository;
		ICosmosDbRepository<UserAccount> _UserAccountRepository;
		ICosmosDbRepository<AccountSubscriptions> _AccountSubscriptionRepository;
		IProcessor _Process;
		IConfiguration _Config;
		ILogger _Logger;
		string _TestUUID = "Test-UUID";

		[TestInitialize]
		public void TestInitialize()
		{
			_Config = InitConfiguration();
			Environment.SetEnvironmentVariable("CosmosDBEndpoint", _Config["CosmosDBEndpoint"]);
			Environment.SetEnvironmentVariable("CosmosDBAuthKey", _Config["CosmosDBAuthKey"]);
			Environment.SetEnvironmentVariable("CosmosDbId", _Config["CosmosDbId"]);
			var startup = new StartUp();
			var host = new HostBuilder().ConfigureWebJobs(startup.Configure).Build();

			_Process = host.Services.GetRequiredService<IProcessor>();

			var cosmosClient = new CosmosClient(_Config["CosmosDBEndpoint"], _Config["CosmosDBAuthKey"], new CosmosClientOptions { AllowBulkExecution = true });

			_UserSubscriptionRepository = new CosmosDbRepository<UserSubscriptionEntity>(cosmosClient);
			_UserAccountRepository = new CosmosDbRepository<UserAccount>(cosmosClient);
			_AccountSubscriptionRepository = new CosmosDbRepository<AccountSubscriptions>(cosmosClient);
			var mock = new Mock<ILogger>();
			_Logger = mock.Object;
		}

		[TestMethod]
		public async Task ProcessorTest_NewRecordInsertedSuccessfully()
		{
			// 1. Delete all subscriptions related to the user
			DeleteAllExistingSubscriptionsAsync(_TestUUID, _Logger);

			// 2. Remove User From All Accounts
			RemoveUserFromAllAccounts(_TestUUID, _Logger);

			// 3. Fire the current process
			var userSubsCommand = GetUserSubscriptionCommand();
			try
			{
				await _Process.ProcessAsync(userSubsCommand, _Logger);
			}
			catch (Exception ex)
			{

			}

			var userSubscriptions = GetUserSubscriptionDocumentByUUId(_TestUUID);

			Assert.IsTrue(userSubscriptions != null);
			Assert.IsTrue(userSubscriptions.Any());
			Assert.IsTrue(userSubscriptions.ToList().Count == 2);
			Assert.IsTrue(userSubscriptions.Any(p=>p.Type == "Subscription"));
			Assert.IsTrue(userSubscriptions.Any(p=>p.Type == "Accounts"));

			var userSubscription = userSubscriptions.FirstOrDefault(p => p.Type == "Subscription");
			Assert.IsTrue(userSubscription.Events != null);
			Assert.IsTrue(userSubscription.Events.Count > 0);
			Assert.IsTrue(userSubscription.Events.Any(p => p.EventType == 71));
			Assert.IsTrue(userSubscription.Events.Any(p => p.EventType == 72));
			Assert.IsTrue(userSubscription.Events.Any(p => p.EventType == 73));
			Assert.IsTrue(userSubscription.Events.Any(p => p.EventType == 74));

			var events = userSubscription.Events;

			foreach(var item in events.Where(p => p.EventType == 73 || p.EventType == 74).ToList())
			{
				Assert.IsTrue(item.ChannelType.Contains(1));
				Assert.IsTrue(item.ChannelType.Contains(2));
			}

			foreach (var item in events.Where(p => p.EventType == 71 || p.EventType == 72).ToList())
			{
				Assert.IsTrue(item.ChannelType.Contains(2));
			}

			foreach (var item in events.Where(p => p.EventType == 75 || p.EventType == 76).ToList())
			{
				Assert.IsTrue(item.ChannelType.Contains(1));
			}

			var userAccounts = GetUserAccountDocumentByUUId(_TestUUID);

			Assert.IsTrue(userAccounts != null);
			Assert.IsTrue(userAccounts.Any());
			Assert.IsTrue(userAccounts.ToList().Count == 2);
			Assert.IsTrue(userAccounts.Any(p => p.Type == "Subscription"));
			Assert.IsTrue(userAccounts.Any(p => p.Type == "Accounts"));

			var userAccount = userAccounts.FirstOrDefault(p => p.Type == "Accounts");
			Assert.IsTrue(userAccount.Payers != null);
			Assert.IsTrue(userAccount.Payers.Count == 3);
			Assert.IsTrue(userAccount.Payers.Any(p => p.PayerNumber == "TestPayer01"));
			Assert.IsTrue(userAccount.Payers.Any(p => p.PayerNumber == "TestPayer02"));
			Assert.IsTrue(userAccount.Payers.Any(p => p.PayerNumber == "TestPayer03"));

		}

		//[TestMethod]
		//public void JsonExtensions_POCOTest()
		//{
		//	var userSubsCommand = GetUserSubscriptionCommand();
		//	try
		//	{
		//		InsertUserSubscription(userSubsCommand, _Logger);
		//	}
		//	catch(Exception ex)
		//	{

		//	}
		//}

		[TestMethod]
		public void Test_UserSubscriptionEntity()
		{
			var userSubscriptionEntity = GetUserSubscriptionEntity();

			Assert.IsTrue(userSubscriptionEntity != null);
			Assert.IsTrue(userSubscriptionEntity.Type != null);
			Assert.IsTrue(userSubscriptionEntity.Type == "Subscription");
			Assert.IsTrue(userSubscriptionEntity.Events != null);
			Assert.IsTrue(userSubscriptionEntity.Events.Count == 3);
			Assert.IsTrue(userSubscriptionEntity.Events[0] != null);
			Assert.IsTrue(userSubscriptionEntity.Events[0].EventType == 1);
			Assert.IsTrue(userSubscriptionEntity.Events[0].ChannelType != null);
			Assert.IsTrue(userSubscriptionEntity.Events[0].ChannelType.Count == 2);
		}


		[TestMethod]
		public void Test_UserAccountEntity()
		{
			var userAccountEntity = GetUserAccountEntity();

			Assert.IsTrue(userAccountEntity != null);
			Assert.IsTrue(userAccountEntity.Type != null);
			Assert.IsTrue(userAccountEntity.Type == "Accounts");
			Assert.IsTrue(userAccountEntity.Id != null);
			Assert.IsTrue(userAccountEntity.Payers != null);
			Assert.IsTrue(userAccountEntity.Payers.Count == 3);
			Assert.IsTrue(userAccountEntity.Payers[0] != null);
			Assert.IsTrue(userAccountEntity.Payers[0].PayerNumber != null);
			Assert.IsTrue(userAccountEntity.Payers[0].Accounts != null);
			Assert.IsTrue(userAccountEntity.Payers[0].Accounts.Count == 2);
		}

		UserSubscriptionEntity GetUserSubscriptionEntity()
		{
			var userSubscriptionEntity = new UserSubscriptionEntity
			{
				Id = Guid.NewGuid().ToString(),
				Events = new List<Event>()
				{
					new Event
					{
						EventType = 1,
						ChannelType = new List<int>
						{
							1, 2
						}
					},
					new Event
					{
						EventType = 2,
						ChannelType = new List<int>
						{
							1
						}
					},
					new Event
					{
						EventType = 3,
						ChannelType = new List<int>
						{
							2
						}
					}
				},
				Type = "Subscription",
				UUId = _TestUUID
			};
			return userSubscriptionEntity;
		}

		UserAccount GetUserAccountEntity()
		{
			var userAccount = new UserAccount
			{
				UUId = _TestUUID,
				Payers = new List<Payer>
				{
					new Payer
					{
						PayerNumber = "TestPayer01",
						Accounts = new List<string>
						{
							"TP01Account01",
							"TP01Account02"
						}
					},
					new Payer
					{
						PayerNumber = "TestPayer02",
						Accounts = new List<string>
						{
							"TP02Account01",
							"TP02Account02",
							"TP02Account03"
						}
					},
					new Payer
					{
						PayerNumber = "TestPayer03",
						Accounts = new List<string>
						{
							"TP03Account01"
						}
					},
				}
			};
			return userAccount;
		}

		UserSubscriptionCommand GetUserSubscriptionCommand()
		{
			var userSubscriptionEntity = new UserSubscriptionCommand
			{
				UserId = _TestUUID,
				Status = "1",
				Subscriptions = new List<SubscriptionDetailCommand>
				{
					new SubscriptionDetailCommand
					{
						PayerNumber = "TestPayer01",
						AccountNumbers = new List<string>{ "TP01Account01", "TP01Account02", "TP01Account03" },

					},
					new SubscriptionDetailCommand
					{
						PayerNumber = "TestPayer02",
						AccountNumbers = new List<string>{ "TP02Account01"},

					},
					new SubscriptionDetailCommand
					{
						PayerNumber = "TestPayer03",
						AccountNumbers = new List<string>{ "TP03Account01", "TP03Account02"},

					}
				},
				EmailNotifications = new List<int> { 71, 72, 73, 74},
				UiNotifications = new List<int> { 73, 74, 75, 76 },
				RequestID = "Sample Request Id"
			};
			return userSubscriptionEntity;
		}

		private static IConfiguration InitConfiguration()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.test.json")
				.Build();
			return config;
		}

		private void DeleteAllExistingSubscriptionsAsync(string userUUId, ILogger logger)
		{
			var subscriptions = _UserSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM UserSubRepo WHERE UserSubRepo.UUID='{userUUId}'").Result;

			foreach (var subscription in subscriptions)
			{
				var resp = _UserSubscriptionRepository.DeleteItemAsync(subscription.Id, subscription.UUId).Result;
			}
		}

		private void RemoveUserFromAllAccounts(string userUUId, ILogger logger)
		{
			var documents = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE ARRAY_CONTAINS(repo.UUIDs, '{userUUId}')").Result;

			foreach (var document in documents)
			{
				document.UUIDs.Remove(userUUId);
				var resp = _AccountSubscriptionRepository.UpdateItemAsync(document.Id, document).Result;
			}
		}

		private IEnumerable<UserSubscriptions> GetUserSubscriptionDocumentByUUId(string userUUId)
		{
			var subscriptions = _UserSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM UserSubRepo WHERE UserSubRepo.UUID='{userUUId}'").Result;

			return subscriptions;
		}

		private IEnumerable<UserAccount> GetUserAccountDocumentByUUId(string userUUId)
		{
			var subscriptions = _UserAccountRepository
				.GetItemsAsync($"SELECT * FROM UserSubRepo WHERE UserSubRepo.UUID='{userUUId}'").Result;

			return subscriptions;
		}

		/// <summary>
		/// Inserts user subscription in DB. 
		/// If there are no events then 
		///		1. there will be no insertion
		///		2. false will be returned
		/// </summary>
		/// <param name="userSubscriptionCommand"></param>
		/// <param name="logger"></param>
		/// <returns></returns>
		private bool InsertUserSubscription(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			bool isUserSubscriptionSuccessFul = false;
			List<Event> events = new List<Event>();

			if (userSubscriptionCommand.UiNotifications != null && userSubscriptionCommand.UiNotifications.Count > 0)
			{
				foreach (var item in userSubscriptionCommand.UiNotifications)
				{
					events.Add(new Event(item, 1));
				}
			}

			if (userSubscriptionCommand.EmailNotifications != null && userSubscriptionCommand.EmailNotifications.Count > 0)
			{
				foreach (var item in userSubscriptionCommand.EmailNotifications)
				{
					var existingEvent = events.FirstOrDefault(p => p.EventType == item);
					if (existingEvent != null)
					{
						existingEvent.ChannelType.Add(2);
					}
					else
					{
						events.Add(new Event(item, 2));
					}
				}
			}

			if (events.Count > 0)
			{
				var userSubscription = new UserSubscriptionEntity(userSubscriptionCommand.UserId, events);
				try
				{
					var response = _UserSubscriptionRepository.CreateItemAsync(userSubscription, userSubscription.UUId).Result;
					isUserSubscriptionSuccessFul = true;
				}
				catch (Exception ex)
				{
					isUserSubscriptionSuccessFul = false;
				}
			}

			return isUserSubscriptionSuccessFul;
		}

	}
}
