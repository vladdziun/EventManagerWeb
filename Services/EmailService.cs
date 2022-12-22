using EventManagerWeb.DTO;
using MailKit.Net.Smtp;
using MimeKit;

namespace EventManagerWeb.Services
{
    public class EmailService
    {

        private readonly IConfiguration _configuration;
        
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendEmail (EmailDto request)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Ethereal:Email"]));
            email.To.Add(MailboxAddress.Parse(request.To));
            email.Subject = request.Subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = request.Body };

            using var smtp = new SmtpClient();
            smtp.CheckCertificateRevocation = false;
            smtp.Connect("smtp.ethereal.email", 587, MailKit.Security.SecureSocketOptions.StartTls);
            smtp.Authenticate(_configuration["Ethereal:Email"], _configuration["Ethereal:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
