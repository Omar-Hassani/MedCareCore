using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MedCareCore.Models
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        // ربط السجل بالموعد الذي تمت فيه الزيارة
        [Required]
        public int AppointmentId { get; set; }

        [ForeignKey("AppointmentId")]
        public virtual Appointment Appointment { get; set; }

        // التشخيص الطبي للممرض أو الطبيب
        [Required(ErrorMessage = "Diagnosis is required")]
        [Display(Name = "Diagnosis / Clinical Findings")]
        public string Diagnosis { get; set; }

        // الوصفة الطبية (الأدوية والجرعات)
        [Required(ErrorMessage = "Prescription details are required")]
        [Display(Name = "Prescription (Medications & Dosages)")]
        public string Prescription { get; set; }

        // حقل مخصص لرفع ملفات التحاليل أو الأشعة (سنخزن فيه اسم الملف أو مساره)
        [Display(Name = "Lab Results / X-Ray Attachment")]
        public string? AttachmentFilePath { get; set; }

        // ملاحظات إضافية أو موعد المراجعة القادم
        [Display(Name = "Follow-up Notes")]
        public string FollowUpNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}