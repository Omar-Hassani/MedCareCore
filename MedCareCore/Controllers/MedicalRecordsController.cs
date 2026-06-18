using MedCareCore.IRepository;
using MedCareCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.StaticFiles;

namespace MedCareCore.Controllers
{
    [Authorize(Roles = "Admin,Doctor")] // 🔐 يسمح للأدمن والطبيب فقط لرؤية وكتابة الروشتات
    public class MedicalRecordsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly MedCareCore.Services.INotificationService _notificationService; // 💡 إضافة الخدمة
        private readonly IWebHostEnvironment _webHostEnvironment; // 💡 الإضافة هنا لإدارة مسارات السيرفر
        public MedicalRecordsController(IUnitOfWork unitOfWork, Services.INotificationService notificationService, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _webHostEnvironment = webHostEnvironment; // 💡 تعيين المتغير
        }

        // 1. عرض جميع السجلات الطبية
        public IActionResult Index()
        {
            // جلب الموعد، ومن الموعد نجلب الطبيب والمريض عبر الـ Navigation Properties
            var records = _unitOfWork.MedicalRecord.GetAll(includeProperties: "Appointment,Appointment.Patient,Appointment.Doctor");
            return View(records);
        }

        // 2. شاشة إضافة سجل طبي مخصص لموعد معين (GET)
        public IActionResult Create()
        {
            ViewBag.AppointmentList = _unitOfWork.Appointment.GetAll("Patient,Doctor") // مررنا النص فقط كمعامل أول وحيد
                 .Where(u => u.Status == "Confirmed" || u.Status == "Pending") // الفلترة تتم هنا في الـ Memory
                 .Select(u => new SelectListItem
                 {
                     Text = $"Patient: {u.Patient.Name} | Doctor: {u.Doctor.Name} ({u.AppointmentDate:g})",
                     Value = u.Id.ToString()
                 });

            return View(new MedicalRecord());
        }

        // 3. حفظ السجل الطبي وتحديث حالة الموعد تلقائياً إلى Completed (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MedicalRecord obj, IFormFile? uploadedFile) // 💡 إضافة باراميتر استقبال الملف
        {
            // إزالة فحص كائن الموعد الكامل لمنع فشل الـ Validation تلقائياً
            ModelState.Remove("Appointment");

            if (ModelState.IsValid)
            {
                // 🔥 [منظومة الرفع المتقدمة] التحقق من وجود ملف مرفوع (صور أشعة، تحاليل، إلخ)
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // 1. تحديد المسار داخل wwwroot/medicalFiles
                    string wwwRootPath = _webHostEnvironment.WebRootPath;
                    // توليد اسم فريد للملف باستخدام Guid مع الحفاظ على امتداده الأصلي (.pdf, .png...)
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                    string finalFolder = Path.Combine(wwwRootPath, @"medicalFiles");

                    // 2. إنشاء المجلد على السيرفر تلقائياً إن لم يكن موجوداً
                    if (!Directory.Exists(finalFolder))
                    {
                        Directory.CreateDirectory(finalFolder);
                    }

                    // 3. حفظ الملف الفعلي في المسار المحدد داخل السيرفر
                    string filePath = Path.Combine(finalFolder, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }

                    // 4. تخزين المسار النسبي في قاعدة البيانات (تأكد من مطابقة اسم الحقل لديك: AttachmentUrl أو FilePath)
                    obj.AttachmentFilePath = @"/medicalFiles/" + fileName;
                }

                // حفظ السجل الطبي الجديد شاملاً مسار الملف المرفوع
                _unitOfWork.MedicalRecord.Add(obj);

                // 💡 الحركة الذكية: تحويل حالة الموعد المرتبط تلقائياً إلى "Completed"
                var appointment = _unitOfWork.Appointment.Get(u => u.Id == obj.AppointmentId, includeProperties: "Patient,Doctor");
                if (appointment != null)
                {
                    appointment.Status = "Completed";
                    _unitOfWork.Appointment.Update(appointment);
                }

                // 🔥 [إطلاق التنبيه تلقائياً] إرسال بريد للمريض يفيد باكتمل طبيبه لكتابة تقريره
                if (appointment != null && appointment.Patient != null && !string.IsNullOrEmpty(appointment.Patient.Email))
                {
                    string subject = "MedCare - Medical Consultation Completed";
                    string message = $"Dear {appointment.Patient.Name},\n\nYour consultation with Dr. {appointment.Doctor?.Name} on {appointment.AppointmentDate:g} has been completed. Your digital prescription and diagnosis report are now available in your file.\n\nBest Regards,\nMedCare Team.";

                    // استدعاء الخدمة بدون تجميد السيرفر (Fire and Forget or Async)
                    Task.Run(() => _notificationService.SendEmailNotificationAsync(appointment.Patient.Email, subject, message));
                }

                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            // إعادة ملء القائمة في حال الفشل
            ViewBag.AppointmentList = _unitOfWork.Appointment.GetAll(includeProperties: "Patient,Doctor").Select(u => new SelectListItem
            {
                Text = $"Patient: {u.Patient.Name} | {u.AppointmentDate:g}",
                Value = u.Id.ToString()
            });
            return View(obj);
        }

        // 4. شاشة تفاصيل السجل الطبي الكامل (Details - GET)
        public IActionResult Details(int? id)
        {
            if (id == null || id == 0) return NotFound();

            // جلب السجل مع الموعد والطبيب والمريض بعملية ربط واحدة
            var record = _unitOfWork.MedicalRecord.Get(
                u => u.Id == id,
                includeProperties: "Appointment,Appointment.Patient,Appointment.Doctor"
            );

            if (record == null) return NotFound();

            return View(record);
        }

        // 5. شاشة تعديل السجل الطبي (Edit - GET)
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var recordFromDb = _unitOfWork.MedicalRecord.Get(u => u.Id == id);
            if (recordFromDb == null) return NotFound();

            // نجهز قائمة المواعيد المتاحة للتعديل لو أراد الطبيب تغيير الموعد المرتبط
            ViewBag.AppointmentList = _unitOfWork.Appointment.GetAll(includeProperties: "Patient,Doctor")
                .Select(u => new SelectListItem
                {
                    Text = $"Patient: {u.Patient.Name} | Date: {u.AppointmentDate:g}",
                    Value = u.Id.ToString()
                });

            return View(recordFromDb);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MedicalRecord obj, IFormFile? uploadedFile)
        {
            ModelState.Remove("Appointment");
            ModelState.Remove("AttachmentFilePath");

            // 💡 حيلة ذكية: نجلب فقط نص المسار من قاعدة البيانات كـ string وليس كـ Object كامل لحماية الـ EF من التتبع المتداخل
            string? oldFilePathFromDb = null;
            var recordInDb = _unitOfWork.MedicalRecord.Get(u => u.Id == obj.Id);
            if (recordInDb != null)
            {
                oldFilePathFromDb = recordInDb.AttachmentFilePath;

                // 🔥 السر هنا: نقوم بفصل الكائن المجلوب فوراً من الـ Entity Framework Tracker لكي لا يعترض عند الـ Update
                _unitOfWork.MedicalRecord.Detach(recordInDb);
            }

            if (uploadedFile == null && string.IsNullOrEmpty(obj.AttachmentFilePath))
            {
                obj.AttachmentFilePath = oldFilePathFromDb;
            }

            if (ModelState.IsValid)
            {
                if (uploadedFile != null && uploadedFile.Length > 0)
                {
                    // حذف الملف القديم من السيرفر ماديّاً
                    if (!string.IsNullOrEmpty(oldFilePathFromDb))
                    {
                        var oldFilePathOnDisk = Path.Combine(_webHostEnvironment.WebRootPath, oldFilePathFromDb.TrimStart('/'));
                        if (System.IO.File.Exists(oldFilePathOnDisk))
                        {
                            System.IO.File.Delete(oldFilePathOnDisk);
                        }
                    }

                    // رفع الملف الجديد
                    string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "medicalFiles");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadedFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        uploadedFile.CopyTo(fileStream);
                    }

                    obj.AttachmentFilePath = "/medicalFiles/" + uniqueFileName;
                }
                else
                {
                    // الاحتفاظ بالقديم
                    obj.AttachmentFilePath = oldFilePathFromDb;
                }

                // الآن التحديث سيتم بسلاسة مطلقة وبدون أي خطأ تتبع!
                _unitOfWork.MedicalRecord.Update(obj);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }

            ViewBag.AppointmentList = _unitOfWork.Appointment.GetAll(includeProperties: "Patient,Doctor")
                .Select(u => new SelectListItem
                {
                    Text = $"Patient: {u.Patient.Name} | Date: {u.AppointmentDate:g}",
                    Value = u.Id.ToString()
                });

            return View(obj);
        }

        // 7. شاشة تأكيد حذف السجل الطبي (Delete - GET)
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var record = _unitOfWork.MedicalRecord.Get(
                u => u.Id == id,
                includeProperties: "Appointment,Appointment.Patient,Appointment.Doctor"
            );

            if (record == null) return NotFound();

            return View(record);
        }

        // 8. تنفيذ الحذف الفعلي (Delete - POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.MedicalRecord.Get(u => u.Id == id);
            if (obj == null) return NotFound();

            _unitOfWork.MedicalRecord.Remove(obj);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }

        // GET: MedicalRecords/PrintPrescription/5
        public IActionResult PrintPrescription(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }

            // جلب السجل الطبي مع الموعد، وجلب المريض والطبيب مع الموعد لضمان اكتمال البيانات
            var medicalRecord = _unitOfWork.MedicalRecord.Get(
                u => u.Id == id,
                includeProperties: "Appointment"
            );

            if (medicalRecord == null)
            {
                return NotFound();
            }

            // بما أن الـ Repository قد لا تدعم الـ Nested Includes العميقة في سطر واحد لبعض التصاميم، 
            // سنقوم بضمان جلب كائنات المريض والطبيب يدوياً وبأمان لمنع الـ Null:
            if (medicalRecord.Appointment != null)
            {
                medicalRecord.Appointment.Patient = _unitOfWork.Patient.Get(u => u.Id == medicalRecord.Appointment.PatientId);
                medicalRecord.Appointment.Doctor = _unitOfWork.Doctor.Get(u => u.Id == medicalRecord.Appointment.DoctorId);
            }

            return View(medicalRecord);
        }

        public IActionResult DownloadAttachment(int id)
        {
            // 1. جلب السجل الطبي من قاعدة البيانات
            var record = _unitOfWork.MedicalRecord.Get(u => u.Id == id);

            if (record == null || string.IsNullOrEmpty(record.AttachmentFilePath))
            {
                return NotFound("عذراً، لا يوجد ملف مرفق لهذا السجل الطبي.");
            }

            // 2. دمج مسار الـ wwwroot مع المسار المخزن في قاعدة البيانات
            // نستخدم التابع TrimStart('/') لتفادي مشاكل دمج المسارات إذا كان النص يبدأ بـ /
            string relativePath = record.AttachmentFilePath.TrimStart('/');
            string filePath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath);

            // 3. التحقق من وجود الملف فعلياً على القرص الصلب للسيرفر
            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("الملف غير موجود على السيرفر، قد يكون قد تم حذفه.");
            }

            // 4. تحديد نوع الملف تلقائياً (Content-Type) لكي يفتح في المتصفح مباشرة (Inline)
            var provider = new FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out string contentType))
            {
                contentType = "application/octet-stream"; // نوع افتراضي في حال لم يتعرف عليه
            }

            // 5. إرجاع الملف لفتحه مباشرة في المتصفح دون إجبار المستخدم على التحميل الفوري
            return File(System.IO.File.ReadAllBytes(filePath), contentType);
        }
    }
}