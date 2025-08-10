using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Logging;
using Moq;
using WebSIMS.Controllers;
using WebSIMS.Models.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using WebSIMS.Services.Interfaces;
using Xunit;

namespace xUnitTest;

public class AuthenUnitTest
{
    private readonly Mock<ILogger<AuthenController>> _loggerMock;
    private readonly Mock<IAuthenService> _authenServiceMock;
    private readonly Mock<ICookiesService> _cookiesServiceMock;
    private readonly AuthenController _authenController;
    

    public AuthenUnitTest()
    {
        _loggerMock = new Mock<ILogger<AuthenController>>();
        _authenServiceMock = new Mock<IAuthenService>();
        _cookiesServiceMock = new Mock<ICookiesService>();
        
        _authenController = new AuthenController(_loggerMock.Object, _authenServiceMock.Object, _cookiesServiceMock.Object);
        
        var httpContext = new DefaultHttpContext();
        _authenController.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }
    [Fact]
    public void Login_Get_UserAuthenticated_RedirectToDashBoard()
    {
        //arrange
        var user = new ClaimsPrincipal(new ClaimsIdentity(
            new Claim[] {new  Claim(ClaimTypes.Name, "user")}, "mock"
            )
        );
        _authenController.ControllerContext.HttpContext.User = user;
        //act
        var result = _authenController.Login();
        //assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Dashboard", redirectResult.ControllerName);
    }
    
    [Fact]
    public void Login_Get_WhenUserNotAuthenticated_ReturnsView()
    {
        // Arrange
        // (unauthenticated)

        // Act
        var result = _authenController.Login();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.IsType<LoginViewModel>(viewResult.Model);
    }
    
    [Fact]
    public async Task Login_Post_InvalidModel_ReturnsViewWithModel()
    {
        // Arrange
        var model = new LoginViewModel { Email = "invalid", Password = "" };
        _authenController.ModelState.AddModelError("Email", "Invalid email");

        // Act
        var result = await _authenController.Login(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.False(_authenController.ModelState.IsValid);
    }
    
    [Fact]
    public async Task Login_Post_ValidModel_UserNotFound_ReturnsViewWithErrorMessage()
    {
        // Arrange
        var model = new LoginViewModel 
        { 
            Email = "nonexistent@example.com", 
            Password = "password" 
        };
            
        _authenServiceMock.Setup(x => x.LoginUserAsync(model.Email, model.Password))
            .ReturnsAsync((WebSIMS.Models.Entities.Users)null);

        // Act
        var result = await _authenController.Login(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.Equal("Account Invalid, please try again!", 
            _authenController.ViewData["MessageLogin"]);
    }
    [Fact]
    public async Task Login_Post_ValidModel_AuthenticationSucceeds_RedirectsToDashboard()
    {
        // Arrange
        var model = new LoginViewModel
        { Email = "valid@example.com", Password = "correctpassword" };
        var user = new WebSIMS.Models.Entities.Users
        { Email = model.Email, Role = "Admin" };
        _authenServiceMock.Setup(x => x.LoginUserAsync(model.Email, model.Password))
            .ReturnsAsync(user);
        var authServiceMock = new Mock<IAuthenticationService>();
        authServiceMock
            .Setup(x => x.SignInAsync(It.IsAny<HttpContext>(),
                It.IsAny<string>(),
                It.IsAny<ClaimsPrincipal>(),
                It.IsAny<AuthenticationProperties>()))
            .Returns(Task.CompletedTask);
        // Mock ITempDataProvider and TempData
        var tempDataProviderMock = Mock.Of<ITempDataProvider>();
        var tempData = new TempDataDictionary(_authenController.ControllerContext.HttpContext, tempDataProviderMock);
        _authenController.TempData = tempData;
        // Mock IUrlHelperFactory and IUrlHelper
        var urlHelperMock = new Mock<IUrlHelper>();
        urlHelperMock
            .Setup(x => x.Action(It.IsAny<UrlActionContext>()));
        var urlHelperFactoryMock = new Mock<IUrlHelperFactory>();
        urlHelperFactoryMock
            .Setup(x => x.GetUrlHelper(It.IsAny<ActionContext>()))
            .Returns(urlHelperMock.Object);

        var serviceProviderMock = new Mock<IServiceProvider>();
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IAuthenticationService)))
            .Returns(authServiceMock.Object);
        serviceProviderMock
            .Setup(x => x.GetService(typeof(ITempDataDictionaryFactory)))
            .Returns(Mock.Of<ITempDataDictionaryFactory>());
        serviceProviderMock
            .Setup(x => x.GetService(typeof(IUrlHelperFactory)))
            .Returns(urlHelperFactoryMock.Object);
        _authenController.ControllerContext.HttpContext.RequestServices = serviceProviderMock.Object;
        // Act
        var result = await _authenController.Login(model);
        // Assert
        var redirectResult = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Index", redirectResult.ActionName);
        Assert.Equal("Dashboard", redirectResult.ControllerName);
        _cookiesServiceMock.Verify(x => x.SetCookie(
                "UserRole",
                user.Role,
                It.IsAny<bool>()),
            Times.Once);

        Assert.Equal("You have successfully logged in.",
            _authenController.TempData["Notification"]);
    }


    [Fact]
    public async Task Login_Post_ValidModel_AuthenticationFails_ReturnsViewWithModel()
    {
        // Arrange
        var model = new LoginViewModel 
        { 
            Email = "valid@example.com", 
            Password = "wrongpassword" 
        };
            
        _authenServiceMock.Setup(x => x.LoginUserAsync(model.Email, model.Password))
            .ReturnsAsync((WebSIMS.Models.Entities.Users)null);

        // Act
        var result = await _authenController.Login(model);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        Assert.Equal(model, viewResult.Model);
        Assert.Equal("Account Invalid, please try again!", 
            _authenController.ViewData["MessageLogin"]);
    }
    
}
