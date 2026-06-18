using MedCareCore.Data; // 💡 تأكد من وجود هذا الـ namespace
using MedCareCore.IRepository;
using MedCareCore.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace MedCareCore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db; // 💡 إضافة سياق البيانات للتنبيهات

        public HomeController(IUnitOfWork unitOfWork, ApplicationDbContext db)
        {
            _unitOfWork = unitOfWork;
            _db = db;
        }

        public IActionResult Index()
        {
            // 1. حساب العدادات الذكية الحالية (Cards) كما هي
            ViewBag.TotalPatients = _unitOfWork.Patient.GetAll().Count();
            ViewBag.TotalDoctors = _unitOfWork.Doctor.GetAll().Count();

            // مواعيد اليوم الحالي فقط
            DateTime today = DateTime.Today;
            var allAppointments = _unitOfWork.Appointment.GetAll(includeProperties: "Patient,Doctor");

            ViewBag.TodayAppointmentsCount = allAppointments.Count(u => u.AppointmentDate.Date == today);

            // 2. حساب إحصائيات الحالات (For Analytics) كما هي
            ViewBag.PendingCount = allAppointments.Count(u => u.Status == "Pending");
            ViewBag.ConfirmedCount = allAppointments.Count(u => u.Status == "Confirmed");
            ViewBag.CompletedCount = allAppointments.Count(u => u.Status == "Completed");
            ViewBag.CancelledCount = allAppointments.Count(u => u.Status == "Cancelled");

            // 3. جلب جدول مواعيد اليوم الحالي وترتيبها حسب الوقت تصاعدياً
            var todayAppointmentsList = allAppointments
                .Where(u => u.AppointmentDate.Date == today)
                .OrderBy(u => u.AppointmentDate)
                .ToList();

            // 🔥 [دمج التنبيهات الحية] جلب آخر 5 تنبيهات غير مقروءة من الأحدث للأقدم
            ViewBag.LatestNotifications = _db.Notifications
                                         .Where(n => !n.IsRead)
                                         .OrderByDescending(n => n.CreatedAt)
                                         .Take(5)
                                         .ToList();

            return View(todayAppointmentsList);
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}