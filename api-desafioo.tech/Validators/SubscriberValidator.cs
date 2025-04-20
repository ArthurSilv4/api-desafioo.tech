using api_desafioo.tech.Requests.SubscriberRequests;
using FluentValidation;

namespace api_desafioo.tech.Validators
{
    public class SubscriberValidator : AbstractValidator<SubscriberRequest>
    {
        public SubscriberValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail deve ser válido.");
        }
    }
}
