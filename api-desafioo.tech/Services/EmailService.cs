using api_desafioo.tech.Configurations;
using api_desafioo.tech.Helpers;
using System.Net.Mail;

namespace api_desafioo.tech.Services
{
    public class EmailService : IEmail
    {
        private readonly ILogger<EmailService> _logger;

        public EmailService(ILogger<EmailService> logger)
        {
            _logger = logger;
        }

        public bool SendEmail(string to, string subject, string body)
        {
            try
            {
                var Username = SmtpConfig.Username;
                var Name = SmtpConfig.Name;
                var Password = SmtpConfig.Password;
                var Host = SmtpConfig.Host;
                var Port = SmtpConfig.Port;
                var EnableSsl = SmtpConfig.EnableSsl;

                _logger.LogInformation("Iniciando envio de e-mail para {To}", to);

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(Username, Name)
                };

                mail.To.Add(to);
                mail.Subject = subject;
                mail.Body = body;
                mail.IsBodyHtml = true;
                mail.Priority = MailPriority.High;

                using (SmtpClient smtp = new SmtpClient(Host, Port))
                {
                    smtp.Credentials = new System.Net.NetworkCredential(Username, Password);
                    smtp.EnableSsl = EnableSsl;
                    smtp.Send(mail);

                    _logger.LogInformation("E-mail enviado com sucesso para {To}", to);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Erro ao enviar e-mail para {To}", to);
                return false;
            }
        }
    }
}
