using System.Net;
using System.Net.Mail;
using System.Text;

namespace IdentityProject.UI.Services
{
    public class EmailService
    {
        public Task Execute(string userEmail, string body, string subject)
        {
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 1000000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new NetworkCredential("mohamadmtvk75@gmail.com", "teac uvlo mcep ajdf");

            MailMessage message = new MailMessage("mohamadmtvk75@gmail.com", userEmail, subject, body);

            message.IsBodyHtml = true;
            message.BodyEncoding = UTF8Encoding.UTF8;
            message.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;

            client.Send(message);

            return Task.CompletedTask;

        }
    }
}
