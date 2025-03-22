using api_desafioo.tech.Configurations;
using api_desafioo.tech.Helpers;
using Markdig;
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
                var Email = SmtpConfig.Email;
                var Username = SmtpConfig.Username;
                var Name = SmtpConfig.Name;
                var Password = SmtpConfig.Password;
                var Host = SmtpConfig.Host;
                var Port = SmtpConfig.Port;
                var EnableSsl = SmtpConfig.EnableSsl;

                _logger.LogInformation("Iniciando envio de e-mail para {To}", to);

                MailMessage mail = new MailMessage()
                {
                    From = new MailAddress(Email, Name)
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

        public bool SendChallengeStartedEmail(string to, string name, string challengeTitle, string description, string difficulty, string[] category, string author, List<string>? links)
        {
            var subject = "Desafio iniciado";
            var body = $@"
                        <html>
                        <head>
                            <style>
                                .markdown-container {{
                                    font-family: 'Arial', sans-serif;
                                    line-height: 1.6;
                                }}
                                .markdown-container h1 {{
                                    font-size: 2.2rem;
                                    font-weight: bold;
                                    margin-top: 1.5rem;
                                    margin-bottom: 1rem;
                                    color: #333;
                                }}
                                .markdown-container h2 {{
                                    font-size: 1.8rem;
                                    font-weight: bold;
                                    margin-top: 1.5rem;
                                    margin-bottom: 1rem;
                                    color: #444;
                                }}
                                .markdown-container h3 {{
                                    font-size: 1.5rem;
                                    font-weight: bold;
                                    margin-top: 1.5rem;
                                    margin-bottom: 1rem;
                                    color: #555;
                                }}
                                .markdown-container ul, .markdown-container ol {{
                                    margin-left: 2rem;
                                    padding-left: 1rem;
                                }}
                                .markdown-container li {{
                                    margin-bottom: 0.5rem;
                                    color: #555;
                                }}
                                .markdown-container a {{
                                    color: #007BFF;
                                    text-decoration: none;
                                    font-weight: 500;
                                }}
                                .markdown-container a:hover {{
                                    text-decoration: underline;
                                    color: #0056b3;
                                }}
                                .markdown-container blockquote {{
                                    background-color: #f9f9f9;
                                    border-left: 5px solid #ccc;
                                    padding: 1rem;
                                    margin: 1rem 0;
                                    font-style: italic;
                                    color: #555;
                                }}
                                .markdown-container blockquote p {{
                                    margin: 0;
                                }}
                                .markdown-container code {{
                                    background-color: #f0f0f0;
                                    color: #d63384;
                                    padding: 0.2rem 0.4rem;
                                    border-radius: 4px;
                                    font-size: 1rem;
                                }}
                                .markdown-container pre code {{
                                    background-color: #2e2e2e;
                                    color: #f5f5f5;
                                    padding: 1rem;
                                    border-radius: 4px;
                                    overflow-x: auto;
                                    display: block;
                                    white-space: pre-wrap;
                                    word-wrap: break-word;
                                    font-size: 1rem;
                                }}
                                .markdown-container table {{
                                    width: 100%;
                                    border-collapse: collapse;
                                    margin-top: 1rem;
                                    margin-bottom: 1rem;
                                }}
                                .markdown-container th, .markdown-container td {{
                                    border: 1px solid #ddd;
                                    padding: 0.5rem;
                                    text-align: left;
                                }}
                                .markdown-container th {{
                                    background-color: #f4f4f4;
                                    font-weight: bold;
                                    color: #333;
                                }}
                                .markdown-container td {{
                                    color: #555;
                                }}
                                .markdown-container img {{
                                    max-width: 100%;
                                    height: auto;
                                    display: block;
                                    margin: 1rem 0;
                                }}
                                .markdown-container ol {{
                                    list-style-type: decimal;
                                }}
                                .markdown-container ol li {{
                                    margin-bottom: 0.5rem;
                                }}
                            </style>
                        </head>
                        <body>
                            <div class='markdown-container'>
                                <div class='header'>
                                    <h1>Desafio Iniciado!</h1>
                                </div>
                                <div class='content'>
                                    <p>Olá {name},</p>
                                    <p>Você iniciou o desafio: <strong>{challengeTitle}</strong>!</p>
                                    <p>Descrição: {Markdown.ToHtml(description)}</p>
                                    <p>Dificuldade: {difficulty}</p>
                                    <p>Categoria: {string.Join(", ", category)}</p>
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

        public bool SendConfirmationEmail(string to, string name, string confirmationCode)
        {
            var subject = "Confirme seu e-mail";
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
                            <h1>Confirme seu e-mail</h1>
                        </div>
                        <div class='content'>
                            <p>Olá, {name}</p>
                            <p>Para confirmar seu e-mail, copie o código abaixo e cole no campo de confirmação:</p>
                            <p><strong>{confirmationCode}</strong></p>
                            <p>Se você não solicitou a confirmação de e-mail, por favor, ignore este e-mail.</p>
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
