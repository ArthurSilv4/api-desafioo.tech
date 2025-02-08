namespace api_desafioo.tech.Requests.UserRequests
{
    public record UpdatePasswordRequest(string oldPassword, string newPassword);
}
