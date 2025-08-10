using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection.Extensions; // <--- THÊM DÒNG NÀY
using WebSIMS.Data; 
using WebSIMS.Models.Entities;
using BCrypt.Net;
using System; // For Exception

namespace xUnitTest
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // **Bước QUAN TRỌNG:** Loại bỏ tất cả các đăng ký DbContextOptions hiện có cho SIMSDbContext
                services.RemoveAll<DbContextOptions<SIMSDbContext>>();
                // Loại bỏ cả đăng ký instance của SIMSDbContext nếu có (cho trường hợp Scoped, Transient, Singleton)
                services.RemoveAll<SIMSDbContext>();

                // **Thêm đăng ký SIMSDbContext MỚI sử dụng cơ sở dữ liệu trong bộ nhớ**
                services.AddDbContext<SIMSDbContext>(options =>
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting"); // Tên database trong bộ nhớ
                });

                // Xây dựng service provider để có thể truy cập SIMSDbContext và seed dữ liệu
                var sp = services.BuildServiceProvider();

                // Tạo scope để lấy DbContext
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<SIMSDbContext>();
                    var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory<TProgram>>>();

                    try
                    {
                        // Đảm bảo database trong bộ nhớ được xóa và tạo lại mới tinh cho mỗi test run
                        db.Database.EnsureDeleted(); // Xóa database nếu tồn tại
                        db.Database.EnsureCreated(); // Tạo database mới

                        // Seed dữ liệu kiểm thử nếu chưa có
                        if (!db.Users.Any())
                        {
                            db.Users.Add(new Users
                            {
                                Id = 1,
                                Name = "Test User",
                                Email = "test@test.com",
                                Password = BCrypt.Net.BCrypt.HashPassword("12345678"), // Mật khẩu được hash
                                Role = "Admin"
                            });
                            db.SaveChanges();
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log lỗi nếu quá trình seed dữ liệu thất bại
                        logger.LogError(ex, "An error occurred seeding the database with test data: {Message}", ex.Message);
                    }
                }
            });
        }
    }
}