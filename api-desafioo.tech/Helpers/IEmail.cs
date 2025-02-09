namespace api_desafioo.tech.Helpers
{
    public interface IEmail
    {
        bool SendEmail(string to, string subject, string body);
        bool SendChallengeStartedEmail(string email, string name, string title, string description, string difficulty, string category, string author, List<string>? links);
        bool SendConfirmationEmail(string email, string name, string confirmationCode);
    }
}
