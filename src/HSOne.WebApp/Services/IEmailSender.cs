using HSOne.WebApp.Models;

namespace HSOne.WebApp.Services
{
    public interface IEmailSender
    {
        Task SendEmail(EmailData emailData);
    }
}
