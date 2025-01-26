namespace api_desafioo.tech.Requests
{
    public record CreateNewChallengeRequest(string title, string description, string dificulty, string category, List<string>? links);
}
