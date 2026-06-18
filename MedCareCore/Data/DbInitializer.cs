using Microsoft.AspNetCore.Identity;
using MedCareCore.Data;
using Microsoft.EntityFrameworkCore;

namespace MedCareCore.DbInitializer
{
    public class DbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            // 1. تشغيل الـ Migrations تلقائياً إذا لم تكن مرفوعة
            try
            {
                if (_db.Database.GetPendingMigrations().Any())
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception) { }

            // 2. إنشاء الأدوار (Roles) في قاعدة البيانات إذا لم تكن موجودة
            if (!_roleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole("Admin")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Doctor")).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole("Receptionist")).GetAwaiter().GetResult();

                // 3. إنشاء مستخدم مدير افتراضي (Admin) وتعيينه كـ Admin
                _userManager.CreateAsync(new IdentityUser
                {
                    UserName = "admin@medcare.com",
                    Email = "admin@medcare.com",
                    EmailConfirmed = true
                }, "Admin123*").GetAwaiter().GetResult();

                IdentityUser user = _db.Users.FirstOrDefault(u => u.Email == "admin@medcare.com");
                _userManager.AddToRoleAsync(user, "Admin").GetAwaiter().GetResult();
            }
        }
    }
}