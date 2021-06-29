using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prem.N3.PayerActivation.Processors.Entities;
using System.Threading.Tasks;

namespace Prem.N3.PayerActivation.Processors
{
	public interface IProcessor
	{
		Task ProcessAsync(PayerActivationCommand payerActivationCommand, ILogger logger);
	}
}
