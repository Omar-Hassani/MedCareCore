using MedCareCore.IRepository;
using MedCareCore.Models;
using MedCareCore.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MedCareCore.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")] // 🔐 يسمح للأدمن وموظف الاستقبال فقط
    public class AppointmentsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public AppointmentsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. عرض المواعيد مع جلب بيانات الطبيب والمريض (Eager Loading)
        public IActionResult Index()
        {  
            // نستخدم includeProperties لجلب البيانات المرتبطة من الجداول الأخرى
            var appointments = _unitOfWork.Appointment.GetAll(includeProperties: "Doctor,Patient");
            return View(appointments);
        }

        // 2. شاشة حجز موعد (GET)
        public IActionResult Create()
        {
            AppointmentVM appointmentVM = new()
            {
                Appointment = new Appointment(),
                DoctorList = _unitOfWork.Doctor.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name + " (" + u.Specialization + ")",
                    Value = u.Id.ToString()
                }),
                PatientList = _unitOfWork.Patient.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };
            return View(appointmentVM);
        }

        // 3. معالجة الحجز مع التحقق من التضارب (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(AppointmentVM obj)
        {
            // 💡 السطر السحري: إزالة كائنات التنقل من الفحص لأننا نعتمد على المعرفات الرقمية فقط (IDs)
            ModelState.Remove("Appointment.Doctor");
            ModelState.Remove("Appointment.Patient");

            // المنطق الذكي: التحقق هل الطبيب لديه موعد آخر في نفس الوقت تماماً؟
            var isBusy = _unitOfWork.Appointment.Get(u =>
                u.DoctorId == obj.Appointment.DoctorId &&
                u.AppointmentDate == obj.Appointment.AppointmentDate);

            if (isBusy != null)
            {
                ModelState.AddModelError("Appointment.AppointmentDate", "This doctor already has an appointment at this time.");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Appointment.Add(obj.Appointment);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            // إعادة ملء القوائم مع تفاصيل التخصص في حال فشل الـ Validation لكي لا تظهر فارغة
            obj.DoctorList = _unitOfWork.Doctor.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name + " (" + u.Specialization + ")",
                Value = u.Id.ToString()
            });
            obj.PatientList = _unitOfWork.Patient.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(obj);
        }

        // 4. شاشة تعديل الموعد (Edit - GET)
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var appointmentFromDb = _unitOfWork.Appointment.Get(u => u.Id == id);
            if (appointmentFromDb == null) return NotFound();

            // نجهز الـ ViewModel ونملأ القوائم المنسدلة ليتمكن المستخدم من التعديل
            AppointmentVM appointmentVM = new()
            {
                Appointment = appointmentFromDb,
                DoctorList = _unitOfWork.Doctor.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name + " (" + u.Specialization + ")",
                    Value = u.Id.ToString()
                }),
                PatientList = _unitOfWork.Patient.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                })
            };

            return View(appointmentVM);
        }

        // 5. معالجة تعديل الموعد مع فحص التضارب (Edit - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(AppointmentVM obj)
        {
            // 💡 الحل السحري: إزالة كائنات التنقل من الفحص لمنع فشل الـ Validation تلقائياً
            ModelState.Remove("Appointment.Doctor");
            ModelState.Remove("Appointment.Patient");

            // فحص التضارب الذكي: نتأكد أن الطبيب ليس مشغولاً بموعد آخر (مع استثناء الموعد الحالي نفسه من الفحص عبر الـ Id)
            var isBusy = _unitOfWork.Appointment.Get(u =>
                u.DoctorId == obj.Appointment.DoctorId &&
                u.AppointmentDate == obj.Appointment.AppointmentDate &&
                u.Id != obj.Appointment.Id);

            if (isBusy != null)
            {
                ModelState.AddModelError("Appointment.AppointmentDate", "This doctor already has another appointment at this time.");
            }

            if (ModelState.IsValid)
            {
                _unitOfWork.Appointment.Update(obj.Appointment);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            // إعادة ملء القوائم مع تفاصيل التخصص (في حال فشل الـ Validation لكي لا تظهر القوائم فارغة أو مشوهة)
            obj.DoctorList = _unitOfWork.Doctor.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name + " (" + u.Specialization + ")",
                Value = u.Id.ToString()
            });

            obj.PatientList = _unitOfWork.Patient.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(obj);
        }

        // 6. شاشة تأكيد حذف الموعد (Delete - GET)
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            // نستخدم الـ Include لجلب اسم الطبيب والمريض لعرضهم في شاشة التأكيد كـ ReadOnly
            var appointment = _unitOfWork.Appointment.Get(u => u.Id == id, includeProperties: "Doctor,Patient");
            if (appointment == null) return NotFound();

            return View(appointment);
        }

        // 7. تنفيذ الحذف الفعلي للموعد (Delete - POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Appointment.Get(u => u.Id == id);
            if (obj == null) return NotFound();

            _unitOfWork.Appointment.Remove(obj);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // 8. شاشة تفاصيل الموعد بالكامل (Details - GET)
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            // جلب الموعد مع كل البيانات المرتبطة بالطبيب والمريض معاً عبر الـ Eager Loading
            var appointment = _unitOfWork.Appointment.Get(u => u.Id == id, includeProperties: "Doctor,Patient");

            if (appointment == null)
            {
                return NotFound();
            }

            return View(appointment);
        }
    }
}