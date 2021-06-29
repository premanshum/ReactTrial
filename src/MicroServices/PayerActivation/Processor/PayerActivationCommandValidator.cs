using FluentValidation;
using Prem.N3.PayerActivation.Processors.Entities;

namespace Prem.N3.PayerActivation.Processors
{
    public class PayerActivationCommandValidator : AbstractValidator<PayerActivationCommand>
    {
        public PayerActivationCommandValidator()
        {
            RuleFor(x => x.RequestID)
                .NotEmpty()
                .WithMessage("No RequestId");

            RuleFor(x => x.PayerNumber)
                .NotEmpty();
        }
    }
}
