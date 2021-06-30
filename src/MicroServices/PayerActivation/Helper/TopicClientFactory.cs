using Microsoft.Azure.ServiceBus;
using System;

namespace Prem.N3.PayerActivation.Helper
{
    public class TopicClientFactory : ITopicClientFactory
    {
        private const string ServiceBusConnectionString = "ServiceBusConnectionString";

        public ITopicClient GetTopicClient(string topicName)
        {
            return new TopicClient(Environment.GetEnvironmentVariable(ServiceBusConnectionString), topicName);
        }

        public ITopicClient GetTopicClient(string serviceBusConnectionString, string topicName)
        {
            return new TopicClient(Environment.GetEnvironmentVariable(serviceBusConnectionString), topicName);
        }
    }
}
