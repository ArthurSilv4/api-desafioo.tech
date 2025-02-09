namespace api_desafioo.tech.Requests.UserRequests
{
    public record UpdatePasswordRequest(string code, string oldPassword, string newPassword, string confirmPassword);
}
