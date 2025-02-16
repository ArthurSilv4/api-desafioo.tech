namespace api_desafioo.tech.Requests.ChallengeRequests
{
    public record CreateNewChallengeRequest(string title, string description, string dificulty, string[] category, List<string>? links);
}
