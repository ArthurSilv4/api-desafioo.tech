namespace api_desafioo.tech.Dto
{
    public record ChallengeDto(Guid id, string title, string description, string dificulty, string category, string author, List<string>? links);
}
