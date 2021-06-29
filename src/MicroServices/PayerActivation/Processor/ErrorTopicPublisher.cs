using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Prem.N3.PayerActivation.Helper;
using Prem.N3.PayerActivation.Processors.Entities;
using System.Text;
using System.Threading.Tasks;

namespace Prem.N3.PayerActivation.Processors
{
    public class ErrorTopicPublisher : IErrorTopicPublisher
    {
        private const string ContentType = "application/json";
        private readonly ITopicClientFactory _factory;
        private ITopicClient _client;

        public ErrorTopicPublisher(ITopicClientFactory factory)
        {
            _factory = factory;
        }

        public async Task PublishAsync(PayerActivationCommand payerActivationCommand)
        {
            _client = _client ?? _factory.GetTopicClient(ServiceBusTopic.Error);

            var message = new Message(
                Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payerActivationCommand)))
            {
                ContentType = ContentType
            };
            await _client.SendAsync(message);
        }
    }
}
