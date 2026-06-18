using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedCareCore.Models
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        // ربط الموعد بالمريض (Foreign Key)
        [Required(ErrorMessage = "Patient is required")]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public virtual Patient Patient { get; set; }

        // ربط الموعد بالطبيب (Foreign Key)
        [Required(ErrorMessage = "Doctor is required")]
        public int DoctorId { get; set; }

        [ForeignKey("DoctorId")]
        public virtual Doctor Doctor { get; set; }

        // تاريخ ووقت الموعد
        [Required(ErrorMessage = "Appointment date and time are required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Appointment Date & Time")]
        public DateTime AppointmentDate { get; set; }

        // حالة الموعد (نستخدم نص أو Enum والأفضل هنا نص لتسهيل العرض والتحكم)
        // الحالات المتوقعة: Pending (قيد الانتظار), Confirmed (تم التأكيد), Cancelled (ملغي), Completed (تمت زيارة الطبيب)
        [Required]
        public string Status { get; set; } = "Pending";

        // ملاحظات موظف الاستقبال أو شكوى المريض المبدئية
        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}