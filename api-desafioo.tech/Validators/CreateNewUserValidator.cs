using api_desafioo.tech.Requests.UserRequests;
using FluentValidation;

namespace api_desafioo.tech.Validators
{
    public class CreateNewUserValidator : AbstractValidator<CreateNewUserRequest>
    {
        public CreateNewUserValidator()
        {
            RuleFor(x => x.name)
               .NotEmpty().WithMessage("O novo nome é obrigatório.")
               .MinimumLength(2).WithMessage("O novo nome deve ter no mínimo 2 caracteres.");
            RuleFor(x => x.email)
                .NotEmpty().WithMessage("O e-mail é obrigatório.")
                .EmailAddress().WithMessage("O e-mail deve ser válido.");
            RuleFor(x => x.password)
               .NotEmpty().WithMessage("A nova senha é obrigatória.")
               .MinimumLength(8).WithMessage("A nova senha deve ter mais de 8 dígitos.")
               .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("A nova senha deve conter pelo menos um caractere especial.");
        }
    }
}
