namespace api_desafioo.tech.Requests.ChallengeRequests
{
    public record UpdateChallengeRequest(string? title, string? description, string? dificulty, string? category, List<string>? links);
}
