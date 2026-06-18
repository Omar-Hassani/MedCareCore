using MedCareCore.IRepository;
using MedCareCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCareCore.Controllers
{
    [Authorize(Roles = "Admin")] // 🔐 قفل الكلاس بالكامل للأدمن فقط
    public class DoctorsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        // حقن الـ Unit of Work عبر الـ Dependency Injection
        public DoctorsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. عرض قائمة الأطباء (Index)
        public IActionResult Index()
        {
            IEnumerable<Doctor> doctorList = _unitOfWork.Doctor.GetAll();
            return View(doctorList);
        }

        // 2. شاشة إضافة طبيب جديد (Create - GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. معالجة بيانات الإضافة (Create - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Doctor obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Doctor.Add(obj);
                _unitOfWork.Save(); // حفظ في قاعدة البيانات دفعة واحدة
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // 4. شاشة تعديل بيانات طبيب (Edit - GET)
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var doctorFromDb = _unitOfWork.Doctor.Get(u => u.Id == id);

            if (doctorFromDb == null)
            {
                return NotFound();
            }

            return View(doctorFromDb);
        }

        // 5. معالجة بيانات التعديل (Edit - POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Doctor obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Doctor.Update(obj);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // 6. شاشة تأكيد الحذف (Delete - GET)
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var doctorFromDb = _unitOfWork.Doctor.Get(u => u.Id == id);

            if (doctorFromDb == null)
            {
                return NotFound();
            }

            return View(doctorFromDb);
        }

        // 7. تنفيذ الحذف الفعلي (Delete - POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Doctor.Get(u => u.Id == id);

            if (obj == null)
            {
                return NotFound();
            }

            _unitOfWork.Doctor.Remove(obj);
            _unitOfWork.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}