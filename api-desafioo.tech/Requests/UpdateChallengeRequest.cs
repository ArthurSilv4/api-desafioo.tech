namespace api_desafioo.tech.Requests
{
    public record UpdateChallengeRequest(string title, string description, string dificulty, string category, List<string>? links);
}
