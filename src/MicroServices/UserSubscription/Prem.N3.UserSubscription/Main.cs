using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Prem.N3.UserSubscription.Helper;
using Prem.N3.UserSubscription.Processors;
using Prem.N3.UserSubscription.Processors.Entities;
using System;

namespace Prem.N3.UserSubscription
{
    public class Main
    {
        private readonly IProcessor _Processor;
        private readonly IErrorTopicPublisher _ErrorTopicPublisher;

        public Main(IProcessor processor,
            IErrorTopicPublisher errorTopicPublisher)
        {
            _Processor = processor;
            _ErrorTopicPublisher = errorTopicPublisher;
        }

        [FunctionName("UserSubscription")]
        public async System.Threading.Tasks.Task RunAsync(
            [ServiceBusTrigger("%ServiceBusTopicName%", "%ServiceBusTopicSubscription%",
            Connection = "ServiceBusConnectionString")]
            string sbMsg,
            ILogger logger)
        {
            try
            {
                logger.LogInformation($"UserSubscription->Started processing message {sbMsg}.");

                logger.LogInformation($"UserSubscription->Serializing.");

                var message = sbMsg.AsPoco<UserSubscriptionCommand>();

                logger.LogInformation($"UserSubscription->Validating {message.RequestID}.");
                var validationResult = new UserSubscriptionCommandValidator().Validate(message);

                if (validationResult.IsValid)
                {
                    logger.LogInformation($"UserSubscription->Validation success for message {message.RequestID}.");
                    await _Processor.ProcessAsync(message, logger);
                    logger.LogInformation($"UserSubscription->Completed processing message {message.RequestID}.");
                }
                else
                {
                    logger.LogWarning($"UserSubscription->failed validation, " +
                        $"RequestID:{message.RequestID}, Validation Errors:{string.Join(";", validationResult.Errors)}.");

                    message.Status = string.Join(";", validationResult.Errors);

                    await _ErrorTopicPublisher.PublishAsync(message);

                    logger.LogWarning($"UserSubscription->message published into error topic with RequestID:{message.RequestID}.");
                }
            }
            catch (Exception exception)
            {
                logger.LogError($"UserSubscription->Unable to process the request: {sbMsg.AsJson()}.", exception, "UserSubscription");
                throw;
            }
        }
    }
}
