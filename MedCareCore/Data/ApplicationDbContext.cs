using Microsoft.EntityFrameworkCore;
using MedCareCore.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace MedCareCore.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // تسجيل الجداول في قاعدة البيانات
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // يمكنك هنا تخصيص العلاقات أو تحديد قيم افتراضية إضافية إذا لزم الأمر مستقبلاً

            // مثال: ضبط دقة حقل سعر الكشفية في جدول الأطباء ليعمل بدون مشاكل تحذيرية
            modelBuilder.Entity<Doctor>()
                .Property(d => d.ConsultationFee)
                .HasPrecision(18, 2);
        }
    }
}