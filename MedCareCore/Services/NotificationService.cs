using MedCareCore.Data;
using MedCareCore.Models;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace MedCareCore.Services
{
    public class NotificationService : INotificationService
    {
        private readonly ApplicationDbContext _db;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(ApplicationDbContext db, ILogger<NotificationService> logger)
        {
            _db = db;
            _logger = logger;
        }

        public async Task SendEmailNotificationAsync(string toEmail, string subject, string message)
        {
            // 1. حفظ التنبيه في قاعدة البيانات للأرشفة
            var notification = new Notification
            {
                TargetUserEmail = toEmail,
                Title = subject,
                Message = message,
                CreatedAt = DateTime.Now
            };

            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            // 2. محاكاة الإرسال في الـ Output Console (الـ Logger) لغرض الفحص والتطوير
            _logger.LogInformation($"[MOCK EMAIL SENT] To: {toEmail} | Subject: {subject} | Body: {message}");

            // 💡 ملاحظة هندسية مستقبلية: هنا يمكنك وضع كود SmtpClient الفعلي لإرسال إيميل حقيقي!
        }
    }
}