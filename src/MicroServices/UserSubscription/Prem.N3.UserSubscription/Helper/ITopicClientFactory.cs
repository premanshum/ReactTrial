using Microsoft.Azure.ServiceBus;

namespace Prem.N3.UserSubscription.Helper
{
    public interface ITopicClientFactory
    {
        ITopicClient GetTopicClient(string topicName);

        ITopicClient GetTopicClient(string serviceBusConnectionString, string topicName);
    }
}
