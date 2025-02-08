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

        public bool SendChallengeStartedEmail(string to, string name, string challengeTitle, string description, string difficulty, string category, string author, List<string>? links)
        {
            var subject = "Desafio iniciado";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            line-height: 1.6;
                        }}
                        .container {{
                            width: 80%;
                            margin: auto;
                            padding: 20px;
                            border: 1px solid #ddd;
                            border-radius: 10px;
                            background-color: #f9f9f9;
                        }}
                        .header {{
                            text-align: center;
                            padding-bottom: 20px;
                        }}
                        .content {{
                            margin-top: 20px;
                        }}
                        .footer {{
                            margin-top: 30px;
                            text-align: center;
                            font-size: 0.9em;
                            color: #777;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Desafio Iniciado!</h1>
                        </div>
                        <div class='content'>
                            <p>Olá {name},</p>
                            <p>Você iniciou o desafio <strong>{challengeTitle}</strong>!</p>
                            <p>Descrição: {description}</p>
                            <p>Dificuldade: {difficulty}</p>
                            <p>Categoria: {category}</p>
                            <p>Autor: {author}</p>
                            <p>Links de apoio:</p>
                            <ul>
                                {string.Join("", links?.Select(link => $"<li><a href='{link}'>{link}</a></li>") ?? Enumerable.Empty<string>())}
                            </ul>
                            <p>Estamos muito felizes em vê-lo(a) participar deste desafio. Desejamos a você boa sorte e esperamos que você aproveite a experiência.</p>
                        </div>
                        <div class='footer'>
                            <p>Atenciosamente,</p>
                            <p>Equipe Desafioo.tech</p>
                        </div>
                    </div>
                </body>
                </html>";
            return SendEmail(to, subject, body);
        }
    }
}
