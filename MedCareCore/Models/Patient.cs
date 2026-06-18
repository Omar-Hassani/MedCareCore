using System.ComponentModel.DataAnnotations;

namespace MedCareCore.Models
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        [StringLength(250)]
        public string Address { get; set; }

        // سجل طبي مصغر عام (مثل الحساسية أو فصيلة الدم)
        [Display(Name = "Medical History Notes")]
        public string MedicalHistoryNotes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;
    }
}