

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using WebSIMS.Data;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;
using WebSIMS.Services;
using WebSIMS.Services.Interfaces;

namespace WebSIMS;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<SIMSDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
        );
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(option =>
        {
            option.LoginPath = "/Authen/login";
            option.AccessDeniedPath = "/Home/Index";

        });
        
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
            options.AddPolicy("LecturerOnly", policy => policy.RequireRole("Lecturer"));
            options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
        });
        
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<ICourseRepository, CourseRepository>();
        builder.Services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
        builder.Services.AddScoped<IAuthenService, AuthenService>();
        builder.Services.AddScoped<AdminService>();
        builder.Services.AddScoped<StudentService>();
        builder.Services.AddScoped<LecturerService>();
        builder.Services.AddScoped<CourseService>();
        builder.Services.AddScoped<ICookiesService, CookiesService>();

        var app = builder.Build();
        
        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapStaticAssets();
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}");
        app.Run();
    }
}