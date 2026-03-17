using Microsoft.AspNetCore.Identity.UI.Services;

namespace _2380600659_HieuNguyen.Services
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Đây là class giả lập. Trong thực tế, bạn sẽ dùng thư viện như MailKit hoặc SendGrid ở đây.
            return Task.CompletedTask;
        }
    }
}