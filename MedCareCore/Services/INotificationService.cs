using System.Threading.Tasks;

namespace MedCareCore.Services
{
    public interface INotificationService
    {
        Task SendEmailNotificationAsync(string toEmail, string subject, string message);
    }
}