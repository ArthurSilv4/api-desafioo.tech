namespace api_desafioo.tech.Dtos.ChallengeDtos
{
    public record DeleteChallengeDto(Guid id, string title, string description, string dificulty, string[] category, string author, int starts, List<string>? links);
}
