using FluentValidation;
using api_desafioo.tech.Requests.AuthRequests;

namespace api_desafioo.tech.Validators
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail deve ser válido.");
            RuleFor(x => x.password)
                .NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
