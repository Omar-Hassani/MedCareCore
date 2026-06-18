using MedCareCore.Data;
using MedCareCore.IRepository;
using MedCareCore.Repository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("MyConnection")));

// تسجيل الـ Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<MedCareCore.Services.INotificationService, MedCareCore.Services.NotificationService>();

// Add services to the container.
builder.Services.AddControllersWithViews();

// 💡 إضافة خدمات الـ Identity وتفعيل الصلاحيات (Roles)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6; // إعدادات كلمة المرور سهلة للتجريب حالياً
})

.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// 💡 [تم الإضافة هنا بنجاح] تحديد مسار صفحة تسجيل الدخول المخصصة في حال حاول شخص غير مخول الدخول
builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Account/Login";
});


// تفعيل خدمات الـ Razor Pages لأن شاشات الـ Identity الافتراضية مبنية عليها
builder.Services.AddRazorPages();

builder.Services.AddScoped<MedCareCore.DbInitializer.DbInitializer>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages(); // 💡 لربط صفحات الـ Identity الافتراضية

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// كود تشغيل دالة الـ Seeding تلقائياً عند الإقلاع
using (var scope = app.Services.CreateScope())
{
    var dbInitializer = scope.ServiceProvider.GetRequiredService<MedCareCore.DbInitializer.DbInitializer>();
    dbInitializer.Initialize();
}

app.Run();

