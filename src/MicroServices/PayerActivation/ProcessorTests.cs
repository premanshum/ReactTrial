using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Prem.N3.PayerActivation.Helper;
using Prem.N3.PayerActivation.Processors;
using Prem.N3.PayerActivation.Processors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prem.N3.PayerActivation.Test
{
	[TestClass]
	public class ProcessorTests
	{
		ICosmosDbRepository<AccountSubscriptions> _AccountSubscriptionRepository;

		IProcessor _Process;
		IConfiguration _Config;
		ILogger _Logger;
		string _TestPayer = "TestPayer01";

		[TestInitialize]
		public void TestInitialize()
		{
			_Config = InitConfiguration();
			Environment.SetEnvironmentVariable("CosmosDBEndpoint", _Config["CosmosDBEndpoint"]);
			Environment.SetEnvironmentVariable("CosmosDBAuthKey", _Config["CosmosDBAuthKey"]);
			Environment.SetEnvironmentVariable("CosmosDbId", _Config["CosmosDbId"]);

			//_userAccountRepository = new CosmosDbRepository<UserSubscriptionEntity>();
			var startup = new StartUp();
			var host = new HostBuilder().ConfigureWebJobs(startup.Configure).Build();

			_Process = host.Services.GetRequiredService<IProcessor>();
			//_AccountSubscriptionRepository = host.Services.GetRequiredService<ICosmosDbRepository<AccountSubscriptions>>();

			var cosmosClient = new CosmosClient(_Config["CosmosDBEndpoint"], _Config["CosmosDBAuthKey"], new CosmosClientOptions { AllowBulkExecution = true });
			_AccountSubscriptionRepository = new CosmosDbRepository<AccountSubscriptions>(cosmosClient);



			var mock = new Mock<ILogger>();
			_Logger = mock.Object;
		}

		[TestMethod]
		public async Task PayerActivation_When_Payer_Is_Made_Active_Successfully()
		{
			// 1. Set dummy records in db with Active set to false respectively
			InsertUpdateAccountDocument(_TestPayer, 0);

			// 2. Fire the current process
			var payerActivationCommand = new PayerActivationCommand
			{
				IsActive = true,
				PayerNumber = _TestPayer,
				RequestID = "Some Id",
				Status = "0"
			};

			await _Process.ProcessAsync(payerActivationCommand, _Logger);

			// 3. Check the DB
			var documents = GetAccountSubscriptionDocumentByPayerNumber(_TestPayer);
			Assert.IsTrue(documents != null);
			Assert.IsTrue(documents.All(p => p.Active == 1));
		}

		[TestMethod]
		public async Task PayerActivation_When_Payer_Is_Made_InActive_Successfully()
		{
			// 1. Set dummy records in db with Active set to false respectively
			InsertUpdateAccountDocument(_TestPayer, 1);

			// 2. Fire the current process
			var payerActivationCommand = new PayerActivationCommand
			{
				IsActive = false,
				PayerNumber = _TestPayer
			};
			await _Process.ProcessAsync(payerActivationCommand, _Logger);

			// 3. Check the DB
			var documents = GetAccountSubscriptionDocumentByPayerNumber(_TestPayer);
			Assert.IsTrue(documents != null);
			Assert.IsTrue(documents.All(p => p.Active == 0));
		}

		[TestMethod]
		public void Test_All_Entities()
		{
			var payerActivationCommand = new PayerActivationCommand
			{
				IsActive = true,
				PayerNumber = _TestPayer,
				RequestID = "Some Id",
				Status = "0"
			};

			Assert.IsTrue(payerActivationCommand != null);
			Assert.IsTrue(payerActivationCommand.RequestID != null);
			Assert.IsTrue(payerActivationCommand.Status != null);


			var accountSubscription = new AccountSubscriptions(_TestPayer, "TestAccount01", "UUID1");

			Assert.IsTrue(accountSubscription != null);
			Assert.IsTrue(accountSubscription.Account != null);
			Assert.IsTrue(accountSubscription.Payer != null);
			Assert.IsTrue(accountSubscription.Id != null);
			Assert.IsTrue(accountSubscription.UUIDs != null);
			Assert.IsTrue(accountSubscription.Active != null);

		}

		PayerActivationCommand GetRequest()
		{
			PayerActivationCommand testInput = new PayerActivationCommand
			{
				RequestID = Guid.NewGuid().ToString(),
				PayerNumber = "TestPayer1",
				IsActive = true
			};
			return testInput;
		}

		private static IConfiguration InitConfiguration()
		{
			var config = new ConfigurationBuilder()
				.AddJsonFile("appsettings.test.json")
				.AddEnvironmentVariables()
				.Build();
			return config;
		}

		private IEnumerable<AccountSubscriptions> GetAccountSubscriptionDocumentByUUId(string uuid)
		{
			var subscriptions = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE ARRAY_CONTAINS(repo.UUIDs, '{uuid}')").Result;

			return subscriptions;
		}

		private IEnumerable<AccountSubscriptions> GetAccountSubscriptionDocumentByAccountNumber(string accountNumber)
		{
			var subscriptions = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE repo.Account='{accountNumber}'").Result;

			return subscriptions;
		}

		private IEnumerable<AccountSubscriptions> GetAccountSubscriptionDocumentByPayerNumber(string payerNumber)
		{
			var subscriptions = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE repo.Payer='{payerNumber}'").Result;

			return subscriptions;
		}

		private IEnumerable<AccountSubscriptions> GetAccountSubscriptionDocumentByAccountNumbers(List<string> accountList)
		{
			var accounts = string.Join("','", accountList);
			var subscriptions = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE repo.Account IN ('{accounts}')").Result;

			return subscriptions;
		}

		private void InsertUpdateAccountDocument(string payerNumber, int active)
		{
			var documents = GetAccountSubscriptionDocumentByPayerNumber(payerNumber);
			if (documents.Any())
			{
				foreach(var document in documents)
				{
					document.Active = active;
					var response = _AccountSubscriptionRepository.UpdateItemAsync(document.Id, document).Result;
				}
			}
			else
			{
				var accountSubscription = new AccountSubscriptions(payerNumber, "TestAccount01", "UUID1");
				var response = _AccountSubscriptionRepository.CreateItemAsync(accountSubscription, payerNumber).Result;
				accountSubscription = new AccountSubscriptions(payerNumber, "TestAccount02", "UUID1");
				accountSubscription.Active = null;
				response = _AccountSubscriptionRepository.CreateItemAsync(accountSubscription, payerNumber).Result;
			}

		}
	}
}
