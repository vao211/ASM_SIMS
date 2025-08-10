using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using WebSIMS;
using WebSIMS.Models.ViewModels;

namespace xUnitTest
{
    public class AuthenIntegrationTest : IClassFixture<CustomWebApplicationFactory<Program>>
    {
private readonly HttpClient _client;
        private readonly CustomWebApplicationFactory<Program> _factory;

        public AuthenIntegrationTest(CustomWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false // Quan trọng: Ngăn không cho client tự động chuyển hướng
            });
        }

        [Fact]
        public async Task Login_Post_ValidCredentials_RedirectsToDashboard()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = "test@test.com", // Email này phải khớp với email đã seed trong CustomWebApplicationFactory
                Password = "12345678" // Mật khẩu này phải khớp với mật khẩu đã seed
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Authen/Login");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", loginModel.Email),
                new KeyValuePair<string, string>("Password", loginModel.Password)
            });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
            Assert.Equal("/Dashboard", response.Headers.Location?.OriginalString);
            
            // Kiểm tra cookie: Sửa .AspNetCore.Identity thành .AspNetCore.Cookies
            // Lấy tất cả các header "Set-Cookie"
            var setCookieHeaders = response.Headers.GetValues("Set-Cookie").ToList();
            
            // Kiểm tra xem có cookie xác thực mặc định của ASP.NET Core không
            Assert.Contains(setCookieHeaders, h => h.Contains(".AspNetCore.Cookies"));
            
            // Kiểm tra cookie tùy chỉnh "UserRole" của bạn
            Assert.Contains(setCookieHeaders, h => h.Contains("UserRole=Admin"));
        }
        

        [Fact]
        public async Task Login_Post_InvalidCredentials_ReturnsViewWithErrors()
        {
            // Arrange
            var loginModel = new LoginViewModel
            {
                Email = "invaliduser",
                Password = "wrongpassword"
            };

            var request = new HttpRequestMessage(HttpMethod.Post, "/Authen/Login");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Email", loginModel.Email),
                new KeyValuePair<string, string>("Password", loginModel.Password)
            });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            // Kiểm tra mã trạng thái HTTP là OK (200) vì nó trả về lại view có lỗi
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Kiểm tra nội dung phản hồi để xem có thông báo lỗi không
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Tên đăng nhập hoặc mật khẩu không đúng", responseString); // Thay bằng thông báo lỗi thực tế của bạn
        }

        [Fact]
        public async Task Login_Get_UnauthenticatedUser_ReturnsLoginView()
        {
            // Arrange
            // Không cần setup gì đặc biệt, client mặc định là chưa xác thực

            // Act
            var response = await _client.GetAsync("/Authen/Login");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var responseString = await response.Content.ReadAsStringAsync();
            Assert.Contains("Đăng nhập", responseString); // Kiểm tra nội dung của trang đăng nhập
        }
        
    }
}