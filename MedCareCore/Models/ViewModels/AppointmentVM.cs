using Microsoft.AspNetCore.Mvc.Rendering;
using MedCareCore.Models;

namespace MedCareCore.ViewModels
{
    public class AppointmentVM
    {
        public Appointment Appointment { get; set; }

        // قائمة منسدلة للأطباء
        public IEnumerable<SelectListItem>? DoctorList { get; set; }

        // قائمة منسدلة للمرضى
        public IEnumerable<SelectListItem>? PatientList { get; set; }
    }
}