using api_desafioo.tech.Requests.UserRequests;
using FluentValidation;

namespace api_desafioo.tech.Validators
{
    public class UpdateUserNameValidator : AbstractValidator<UpdateUserNameRequest>
    {
        public UpdateUserNameValidator()
        {
            RuleFor(x => x.newName)
                .NotEmpty().WithMessage("O novo nome é obrigatório.")
                .MinimumLength(2).WithMessage("O novo nome deve ter no mínimo 2 caracteres.");
        }
    }
}
