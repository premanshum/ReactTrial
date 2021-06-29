using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Prem.N3.PayerActivation.Helper;
using Prem.N3.PayerActivation.Processors;
using Prem.N3.PayerActivation.Processors.Entities;
using System;

namespace Prem.N3.PayerActivation
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

        [FunctionName("PayerActivation")]
        public async System.Threading.Tasks.Task RunAsync(
            [ServiceBusTrigger("%ServiceBusTopicName%", "%ServiceBusTopicSubscription%",
            Connection = "ServiceBusConnectionString")]
            string sbMsg,
            ILogger logger)
        {
            try
            {
                logger.LogInformation($"PayerActivation->Started processing message {sbMsg}.");

                logger.LogInformation($"PayerActivation->Serializing.");

                var message = sbMsg.AsPoco<PayerActivationCommand>();

                logger.LogInformation($"PayerActivation->Validating {message.RequestID}.");

                var validationResult = new PayerActivationCommandValidator().Validate(message);

                if (validationResult.IsValid)
                {
                    logger.LogInformation($"PayerActivation->Validation success for message {message.RequestID}.");
                    await _Processor.ProcessAsync(message, logger);
                    logger.LogInformation($"PayerActivation->Completed processing message {message.RequestID}.");
                }
                else
                {
                    logger.LogWarning($"PayerActivation-> failed validation, " +
                        $"RequestID:{message.RequestID}, Validation Errors:{string.Join(";", validationResult.Errors)}");

                    message.Status = string.Join(";", validationResult.Errors);

                    await _ErrorTopicPublisher.PublishAsync(message);

                    logger.LogWarning($"PayerActivation->message published into error topic with RequestID:{message.RequestID}");
                }
            }
            catch (Exception exception)
            {
                logger.LogError($"PayerActivation->Unable to process the request: {sbMsg.AsJson()}.", exception, "UserSubscription");
                throw;
            }
        }
    }
}
