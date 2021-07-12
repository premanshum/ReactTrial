using Prem.N3.UserSubscription.Processors.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Prem.N3.UserSubscription.Processors
{
    public interface IErrorTopicPublisher
    {
        Task PublishAsync(UserSubscriptionCommand userSubscriptionCommand);
    }
}
