using Prem.N3.PayerActivation.Processors.Entities;
using System.Threading.Tasks;

namespace Prem.N3.PayerActivation.Processors
{
    public interface IErrorTopicPublisher
    {
        Task PublishAsync(PayerActivationCommand payerActivationCommand);
    }
}
