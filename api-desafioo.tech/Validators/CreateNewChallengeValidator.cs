using api_desafioo.tech.Requests.ChallengeRequests;
using FluentValidation;

namespace api_desafioo.tech.Validators
{
    public class CreateNewChallengeValidator : AbstractValidator<CreateNewChallengeRequest>
    {
        public CreateNewChallengeValidator()
        {
            RuleFor(c => c.title)
                .NotEmpty()
                .MaximumLength(200)
                .WithMessage("O título deve ter no máximo 200 caracteres");
            RuleFor(c => c.description)
                .NotEmpty()
                .MaximumLength(5000)
                .WithMessage("A descrição deve ter no máximo 5000 caracteres");
            RuleFor(c => c.dificulty)
                .NotEmpty()
                .Must(d => d == "Facil" || d == "Media" || d == "Dificil")
                .WithMessage("A dificuldade deve ser 'Facil', 'Media' ou 'Dificil'");
            RuleFor(c => c.category)
                .NotEmpty()
                .Must(c => new[] { "Backend", "Frontend", "Fullstack", "Mobile", "Ciência de Dados", "DevOps", "Segurança", "IA/ML", "Desenvolvimento de Jogos", "Sistemas Embarcados", "IoT", "Blockchain", "AR/VR", "Computação em Nuvem", "Redes", "Banco de Dados", "Desenvolvimento Web", "Teste de Software", "Design UI/UX", "Gestão de Projetos" }.Contains(c))
                .WithMessage("A categoria deve ser uma das seguintes: 'Backend', 'Frontend', 'Fullstack', 'Mobile', 'Ciência de Dados', 'DevOps', 'Segurança', 'IA/ML', 'Desenvolvimento de Jogos', 'Sistemas Embarcados', 'IoT', 'Blockchain', 'AR/VR', 'Computação em Nuvem', 'Redes', 'Banco de Dados', 'Desenvolvimento Web', 'Teste de Software', 'Design UI/UX', 'Gestão de Projetos'");
            RuleFor(c => c.links)
                .Must(l => l == null || l.Count <= 5)
                .WithMessage("A lista de links deve ter no máximo 5 elementos");
        }
    }
}
