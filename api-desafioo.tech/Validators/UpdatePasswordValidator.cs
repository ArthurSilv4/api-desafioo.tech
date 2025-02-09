using api_desafioo.tech.Requests.UserRequests;
using FluentValidation;

namespace api_desafioo.tech.Validators
{
    public class UpdatePasswordValidator : AbstractValidator<UpdatePasswordRequest>
    {
        public UpdatePasswordValidator()
        {
            RuleFor(x => x.oldPassword)
                .NotEmpty().WithMessage("A senha antiga é obrigatória.");
            RuleFor(x => x.newPassword)
                .NotEmpty().WithMessage("A nova senha é obrigatória.")
                .MinimumLength(8).WithMessage("A nova senha deve ter mais de 8 dígitos.")
                .Matches(@"[!@#$%^&*(),.?""{}|<>]").WithMessage("A nova senha deve conter pelo menos um caractere especial.");
            RuleFor(x => x.confirmPassword)
                .Equal(x => x.newPassword).WithMessage("As senhas não coincidem.");
        }
    }
}
