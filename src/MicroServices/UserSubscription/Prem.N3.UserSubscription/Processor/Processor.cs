using Microsoft.Extensions.Logging;
using Prem.N3.UserSubscription.Helper;
using Prem.N3.UserSubscription.Processors.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UserSubscriptionEntity = Prem.N3.UserSubscription.Processors.Entities.UserSubscriptions;

namespace Prem.N3.UserSubscription.Processors
{
	public class Processor : IProcessor
	{
		private readonly ICosmosDbRepository<UserSubscriptionEntity> _UserSubscriptionRepository;
		private readonly ICosmosDbRepository<UserAccount> _UserAccountRepository;
		private readonly ICosmosDbRepository<AccountSubscriptions> _AccountSubscriptionRepository;

		public Processor(
			ICosmosDbRepository<UserSubscriptionEntity> userSubscriptionRepository,
			ICosmosDbRepository<UserAccount> userAccountRepository,
			ICosmosDbRepository<AccountSubscriptions> accountSubscriptionRepository)
		{
			_UserSubscriptionRepository = userSubscriptionRepository;
			_UserAccountRepository = userAccountRepository;
			_AccountSubscriptionRepository = accountSubscriptionRepository;
		}

		public async Task ProcessAsync(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			try
			{
				logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Processing...");
				FindAndReplace(userSubscriptionCommand, logger);
				logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Processing completed");
			}
			catch(Exception ex)
			{
				logger.LogWarning($"Error in Process: {ex.Message}, payload: {userSubscriptionCommand.AsJson()}.");
				throw ex;
			}
		}

		private async void FindAndReplace(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			// 1. Delete the existing subscription documents
			await DeleteAllExistingSubscriptionsAsync(userSubscriptionCommand, logger);

			// 2. Insert user subscription document
			var isInsertSuccess = InsertUserSubscription(userSubscriptionCommand, logger);

			// if Insertion is success that means there are events for which user subscribed
			// Otherwise there are no events to subscribe. in that case delete all te entries of the user.
			if (isInsertSuccess)
			{
				// 3. Insert user account document
				InsertUserAccount(userSubscriptionCommand, logger);

				// 4. Insert account documents
				InsertAccountSubscription(userSubscriptionCommand, logger);

				// 5. Remove the user from accounts where they are not entitled to exist
				RemoveUserFromUnAssociatedAccounts(userSubscriptionCommand, logger);
			}
			else
			{
				// 6. Remove user from all account
				RemoveUserFromAllAccounts(userSubscriptionCommand, logger);
			}
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
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Start Insert into User-Subscription.");
			bool isUserSubscriptionSuccess = false;
			List<Event> events = new List<Event>();

			if(userSubscriptionCommand.UiNotifications != null && userSubscriptionCommand.UiNotifications.Count > 0)
			{
				foreach(var item in userSubscriptionCommand.UiNotifications)
				{
					events.Add(new Event(item, 1));
				}
			}

			if (userSubscriptionCommand.EmailNotifications != null && userSubscriptionCommand.EmailNotifications.Count > 0)
			{
				foreach (var item in userSubscriptionCommand.EmailNotifications)
				{
					var existingEvent = events.FirstOrDefault(p => p.EventType == item);
					if(existingEvent != null)
					{
						existingEvent.ChannelType.Add(2);
					}
					else
					{
						events.Add(new Event(item, 2));
					}
				}
			}

			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Events: {events.AsJson()}.");

			if (events.Count > 0)
			{
				var userSubscription = new UserSubscriptionEntity(userSubscriptionCommand.UserId, events);
				try
				{
					logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Insert into User-Subscription {userSubscription.AsJson()}.");
					var response = _UserSubscriptionRepository.CreateItemAsync(userSubscription, userSubscription.UUId).Result;
					logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Completed Inserting into User-Subscription {userSubscription.AsJson()}.");
					isUserSubscriptionSuccess = true;
				}
				catch(Exception ex)
				{
					isUserSubscriptionSuccess = false;
				}
			}

			return isUserSubscriptionSuccess;
		}

		private void InsertUserAccount(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			List<Payer> Payers = new List<Payer>();

			if (userSubscriptionCommand.Subscriptions != null && userSubscriptionCommand.Subscriptions.Count > 0)
			{
				foreach (var subscription in userSubscriptionCommand.Subscriptions)
				{
					Payers.Add(new Payer(subscription.PayerNumber, subscription.AccountNumbers.ToList()));
				}
			}

			var userAccount = new UserAccount(userSubscriptionCommand.UserId, Payers);

			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Insert into User-Account {userAccount.AsJson()}.");

			var response = _UserAccountRepository.CreateItemAsync(userAccount, userAccount.UUId).Result;
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Completed Inserting into User-Account {userSubscriptionCommand.UserId}.");

		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="userSubscriptionCommand"></param>
		/// <param name="logger"></param>
		private void InsertAccountSubscription(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			var userId = userSubscriptionCommand.UserId;
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Insert into Account Subscription {userSubscriptionCommand.UserId}.");

			foreach (var subscription in userSubscriptionCommand.Subscriptions)
			{
				var documents = _AccountSubscriptionRepository
					.GetItemsAsync($"SELECT * FROM repo WHERE repo.Payer='{subscription.PayerNumber}'").Result;

				// To do
				if (documents != null && documents.Any())
				{
					foreach (var accountItem in subscription.AccountNumbers)
					{
						AccountSubscriptions accountDocument = documents.FirstOrDefault(p => p.Account == accountItem);
						if (accountDocument != null)
						{
							if (!accountDocument.UUIDs.Any(p => p == userId))
							{
								accountDocument.UUIDs.Add(userId);
								var resp = _AccountSubscriptionRepository.UpdateItemAsync(accountDocument.Id, accountDocument).Result;
							}
						}
						else
						{
							accountDocument = new AccountSubscriptions(subscription.PayerNumber, accountItem, userId);
							var response = _AccountSubscriptionRepository.CreateItemAsync(accountDocument, subscription.PayerNumber).Result;
						}
					}

					var accountsForRemoval = documents.Where(p => !subscription.AccountNumbers.Contains(p.Account));

					foreach (var accountDocument in accountsForRemoval)
					{
						accountDocument.UUIDs.Remove(userId);
						var resp = _AccountSubscriptionRepository.UpdateItemAsync(accountDocument.Id, accountDocument).Result;
					}
				}
				else
				{
					foreach (var accountItem in subscription.AccountNumbers)
					{
						AccountSubscriptions accountDocument = new AccountSubscriptions(subscription.PayerNumber, accountItem, userId);
						var response = _AccountSubscriptionRepository.CreateItemAsync(accountDocument, subscription.PayerNumber).Result;
					}
				}
			}
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Completed Inserting into Account Subscription {userSubscriptionCommand.UserId}.");
		}

		/// <summary>
		/// Remove user from the accounts where he is not subscribed anymore.
		/// </summary>
		/// <param name="userSubscriptionCommand"></param>
		/// <param name="logger"></param>
		private void RemoveUserFromUnAssociatedAccounts(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			List<string> accountList = new List<string>();

			foreach(var subscription in userSubscriptionCommand.Subscriptions)
			{
				accountList.AddRange(subscription.AccountNumbers);
			}

			var accounts = string.Join("','", accountList);
			var documents = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE ARRAY_CONTAINS(repo.UUIDs, '{userSubscriptionCommand.UserId}') AND repo.Account NOT IN ('{accounts}')").Result;
			
			var unAssociatedAccounts = string.Join("','", documents.Select(p => p.Account).ToList());
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Remove User From UnAssociated Accounts {userSubscriptionCommand.UserId}. Count: {documents.ToList().Count}. Accounts: {unAssociatedAccounts}.");

			foreach (var document in documents)
			{
				document.UUIDs.Remove(userSubscriptionCommand.UserId);
				var resp = _AccountSubscriptionRepository.UpdateItemAsync(document.Id, document).Result;
			}
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Completed Removing User From UnAssociated Accounts.");
		}

		/// <summary>
		/// To remove users from all the accounts. This will happen when there are no events to subscribe.
		/// </summary>
		/// <param name="userSubscriptionCommand"></param>
		/// <param name="logger"></param>
		private void RemoveUserFromAllAccounts(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			var documents = _AccountSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM repo WHERE ARRAY_CONTAINS(repo.UUIDs, '{userSubscriptionCommand.UserId}')").Result;

			var accounts = string.Join("','", documents.Select(p => p.Account).ToList());

			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Remove User From All Accounts {userSubscriptionCommand.UserId}. Count: {documents.ToList().Count}. Accounts: {accounts}");

			foreach (var document in documents)
			{
				document.UUIDs.Remove(userSubscriptionCommand.UserId);
				var resp = _AccountSubscriptionRepository.UpdateItemAsync(document.Id, document).Result;
			}

			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Completed Removing User From All Accounts.");

		}

		private async Task DeleteAllExistingSubscriptionsAsync(UserSubscriptionCommand userSubscriptionCommand, ILogger logger)
		{
			List<Task> TaskList;
			var subscriptions = _UserSubscriptionRepository
				.GetItemsAsync($"SELECT * FROM UserSubRepo WHERE UserSubRepo.UUID='{userSubscriptionCommand.UserId}'").Result;

			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Deleting all subscription for {userSubscriptionCommand.UserId}. Count: {subscriptions.ToList().Count}");

			if (!subscriptions.Any())
			{
				return;
			}
			TaskList = new List<Task>();

			foreach (var subscription in subscriptions)
			{
				Task t = _UserSubscriptionRepository.DeleteItemAsync(subscription.Id, subscription.UUId);
				TaskList.Add(t);
			}

			Task.WaitAll(TaskList.ToArray());
			logger.LogInformation($"UserSubscription->Request Id:{userSubscriptionCommand.RequestID}. Deleted all subscription.");
		}
	}
}
