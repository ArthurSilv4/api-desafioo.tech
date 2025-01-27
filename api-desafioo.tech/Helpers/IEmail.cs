namespace api_desafioo.tech.Helpers
{
    public interface IEmail
    {
        bool SendEmail(string to, string subject, string body);
    }
}
