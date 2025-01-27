using api_desafioo.tech.Requests;
using FluentValidation;

namespace api_desafioo.tech.Validators
{
    public class StartChallengeValidator : AbstractValidator<StartChallengeRequest>
    {
        public StartChallengeValidator()
        {
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail deve ser válido.");
            RuleFor(x => x.name)
                .NotEmpty().WithMessage("O novo nome é obrigatório.")
                .MinimumLength(2).WithMessage("O novo nome deve ter no mínimo 2 caracteres.");
        }
    }
}
