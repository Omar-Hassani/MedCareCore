using System.ComponentModel.DataAnnotations;

namespace MedCareCore.Models
{
    public class Doctor
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Doctor name is required")]
        [StringLength(100)]
        public string Name { get; set; }

        [Required(ErrorMessage = "Specialization is required")]
        [StringLength(100)]
        public string Specialization { get; set; }

        [Required]
        [Display(Name = "Consultation Fee")]
        public decimal ConsultationFee { get; set; }

        [Required]
        [Phone]
        public string Phone { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}