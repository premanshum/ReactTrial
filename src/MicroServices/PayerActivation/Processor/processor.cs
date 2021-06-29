using Microsoft.Extensions.Logging;
using Prem.N3.PayerActivation.Helper;
using Prem.N3.PayerActivation.Processors.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Prem.N3.PayerActivation.Processors
{
	public class Processor : IProcessor
	{
		private readonly ICosmosDbRepository<AccountSubscriptions> _AccountSubscriptionRepository;

		public Processor(
			ICosmosDbRepository<AccountSubscriptions> accountSubscriptionRepository)
		{
			_AccountSubscriptionRepository = accountSubscriptionRepository;
		}

		public async Task ProcessAsync(PayerActivationCommand payerActivationCommand, ILogger logger)
		{
			try
			{
				logger.LogInformation($"PayerActivation->Request Id:{payerActivationCommand.RequestID}. Processing...");
				await FindAndReplace(payerActivationCommand, logger);
				logger.LogInformation($"PayerActivation->Request Id:{payerActivationCommand.RequestID}. Processing completed");
			}
			catch(Exception ex)
			{
				logger.LogInformation($"PayerActivation->Error in processing message {nameof(payerActivationCommand)}; Error: {ex.Message}", "PayerActivation");
				throw ex;
			}
		}

		private async Task FindAndReplace(PayerActivationCommand payerActivationCommand, ILogger logger)
		{
			await UpdateAccountSubscription(payerActivationCommand, logger);
		}

		private async Task UpdateAccountSubscription(PayerActivationCommand payerActivationCommand, ILogger logger)
		{
			logger.LogInformation($"PayerActivation->Request Id:{payerActivationCommand.RequestID}. Updating Account Subscription.");
			List<Task> taskList = new List<Task>();
			var documents = _AccountSubscriptionRepository
					.GetItemsAsync($"SELECT * FROM repo WHERE repo.Payer='{payerActivationCommand.PayerNumber}'").Result;


			foreach (var document in documents)
			{
				document.Active = payerActivationCommand.IsActive?1:0;
				Task t = _AccountSubscriptionRepository.UpdateItemAsync(document.Id, document);
				taskList.Add(t);
			}

			await Task.WhenAll(taskList.ToArray());
			logger.LogInformation($"PayerActivation->Request Id:{payerActivationCommand.RequestID}. Updated Account Subscription.");
		}

	}
}
