using MedCareCore.IRepository;
using MedCareCore.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedCareCore.Controllers
{
    [Authorize(Roles = "Admin,Receptionist")] // 🔐 يسمح للأدمن وموظف الاستقبال فقط
    public class PatientsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public PatientsController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // 1. عرض قائمة المرضى
        public IActionResult Index()
        {
            IEnumerable<Patient> patientList = _unitOfWork.Patient.GetAll();
            return View(patientList);
        }

        // 2. شاشة إضافة مريض (GET)
        public IActionResult Create()
        {
            return View();
        }

        // 3. معالجة إضافة مريض (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Patient obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Patient.Add(obj);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // 4. شاشة تعديل مريض (GET)
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var patientFromDb = _unitOfWork.Patient.Get(u => u.Id == id);
            if (patientFromDb == null) return NotFound();

            return View(patientFromDb);
        }

        // 5. معالجة تعديل مريض (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Patient obj)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Patient.Update(obj);
                _unitOfWork.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(obj);
        }

        // 6. شاشة تأكيد الحذف (GET)
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0) return NotFound();

            var patientFromDb = _unitOfWork.Patient.Get(u => u.Id == id);
            if (patientFromDb == null) return NotFound();

            return View(patientFromDb);
        }

        // 7. تنفيذ الحذف الفعلي (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletePOST(int? id)
        {
            var obj = _unitOfWork.Patient.Get(u => u.Id == id);
            if (obj == null) return NotFound();

            _unitOfWork.Patient.Remove(obj);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}