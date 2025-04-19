namespace api_desafioo.tech.Dto
{
    public record ChallengeDto(Guid id, string title, string description, string dificulty, string[] category, string author, int starts, List<string>? links);
}
