using FluentValidation;
using Prem.N3.UserSubscription.Processors.Entities;

namespace Prem.N3.UserSubscription.Processors
{
    public class UserSubscriptionCommandValidator : AbstractValidator<UserSubscriptionCommand>
    {
        public UserSubscriptionCommandValidator()
        {
            RuleFor(x => x.RequestID)
                .NotEmpty()
                .WithMessage("No RequestId");

            RuleFor(x => x.UserId)
                .NotEmpty();

            RuleFor(x => x.Subscriptions)
                .NotEmpty()
                .WithMessage("No subscriptions");

            RuleForEach(x => x.Subscriptions)
                .Must(p=>p.AccountNumbers.Count > 0)
                .WithMessage("No subscriptions Accounts");
        }
    }
}
