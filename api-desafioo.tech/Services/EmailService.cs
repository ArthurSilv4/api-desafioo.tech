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
            var subject = "Desafio Iniciado";
            var body = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                            background-color: #f4f4f9;
                            margin: 0;
                            padding: 0;
                        }}
                        .container {{
                            width: 100%;
                            max-width: 600px;
                            margin: 0 auto;
                            background-color: #ffffff;
                            padding: 20px;
                            border-radius: 8px;
                            box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
                        }}
                        .header {{
                            text-align: center;
                            background-color: #007bff;
                            color: #ffffff;
                            padding: 20px;
                            border-radius: 8px 8px 0 0;
                        }}
                        .header h1 {{
                            margin: 0;
                            font-size: 2rem;
                        }}
                        .content {{
                            padding: 20px;
                        }}
                        .content p {{
                            font-size: 1rem;
                            line-height: 1.6;
                            color: #333333;
                        }}
                        .content strong {{
                            color: #007bff;
                        }}
                        .links ul {{
                            list-style-type: none;
                            padding: 0;
                        }}
                        .links li {{
                            margin-bottom: 10px;
                        }}
                        .links a {{
                            color: #007bff;
                            text-decoration: none;
                            font-weight: 500;
                        }}
                        .links a:hover {{
                            text-decoration: underline;
                        }}
                        .support {{
                            background-color: #f9f9f9;
                            border-left: 5px solid #007bff;
                            padding: 10px 20px;
                            margin-top: 20px;
                            font-size: 1rem;
                            color: #333333;
                        }}
                        .footer {{
                            text-align: center;
                            padding: 10px;
                            font-size: 0.875rem;
                            color: #777777;
                            border-top: 1px solid #eeeeee;
                            margin-top: 20px;
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
                            <p>Você iniciou o desafio: <strong>{challengeTitle}</strong>!</p>
                            <p><strong>Descrição:</strong> {Markdown.ToHtml(description)}</p>
                            <p><strong>Dificuldade:</strong> {difficulty}</p>
                            <p><strong>Categoria:</strong> {string.Join(", ", category)}</p>
                            <p><strong>Autor:</strong> {author}</p>
                            <div class='links'>
                                <p><strong>Links de apoio:</strong></p>
                                <ul>
                                    {string.Join("", links?.Select(link => $"<li><a href='{link}'>{link}</a></li>") ?? Enumerable.Empty<string>())}
                                </ul>
                            </div>
                            <div class='support'>
                                <p><strong>Gostou do desafio? Apoie nosso projeto open source!</strong></p>
                                <p>Nosso projeto é open source e depende do seu apoio para continuar crescendo e oferecendo desafios inovadores. Cada contribuição é fundamental para o sucesso da iniciativa!</p>
                                <p><a href='https://apoia.se/desafiootech' target='_blank' style='font-weight: bold; color: #007bff; text-decoration: none;'>Clique aqui para apoiar</a></p>
                                <p>Obrigado por participar! Boa sorte no desafio!</p>
                            </div>
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
