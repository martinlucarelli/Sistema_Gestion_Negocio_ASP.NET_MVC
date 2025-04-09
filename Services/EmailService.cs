using Microsoft.Extensions.Options;
using MimeKit;
using Sistema_Gestion_Negocio_ASP.NET_MVC.Configuration;

namespace Sistema_Gestion_Negocio_ASP.NET_MVC.Services
{
    public class EmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _config;
        public EmailService(IOptions<EmailSettings> emailSettings, IConfiguration configuration)
        {
            _emailSettings = emailSettings.Value;
            _config = configuration;
        }

        public async Task<bool> EnviarCorreo(string destinatario, string asunto, string mensaje)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Mi Aplicación", _emailSettings.User));
                email.To.Add(MailboxAddress.Parse(destinatario));
                email.Subject = asunto;
                email.Body = new TextPart("html") { Text = mensaje };

                using var smtp = new MailKit.Net.Smtp.SmtpClient();
                //trae el servidor y el puerto desde el appsettings
                await smtp.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:SmtpPort"]), false);
                //Treae el usuario y la contraseña desde la clase Emailservice ya que al ser datos sensibles estan en user-secrets
                await smtp.AuthenticateAsync(_emailSettings.User,_emailSettings.Password);
                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error enviando correo: {ex.Message}");
                return false;
            }
        }





    }
}
