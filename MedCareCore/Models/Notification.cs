using System;
using System.ComponentModel.DataAnnotations;

namespace MedCareCore.Models
{
    public class Notification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string TargetUserEmail { get; set; } = string.Empty; // إيميل الشخص المستهدف (طبيب أو مريض)

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Message { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public bool IsRead { get; set; } = false;
    }
}