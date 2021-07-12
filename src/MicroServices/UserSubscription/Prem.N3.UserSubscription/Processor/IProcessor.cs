using Microsoft.Extensions.Logging;
using Prem.N3.UserSubscription.Processors.Entities;
using System.Threading.Tasks;

namespace Prem.N3.UserSubscription.Processors
{
	public interface IProcessor
	{
		Task ProcessAsync(UserSubscriptionCommand userSubscriptionCommand, ILogger logger);
	}
}
