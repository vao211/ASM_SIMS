using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using WebSIMS.Models.ViewModels;
using Console = System.Console;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;
[Authorize]
public class AuthenController : Controller
{
    private readonly ILogger<AuthenController> _logger;
    private readonly ICookiesService _cookiesService;
    private readonly IAuthenService _authenService;
    public AuthenController(ILogger<AuthenController> logger, IAuthenService authenService, ICookiesService cookiesService)
    {
        _logger = logger;
        _authenService = authenService;
        _cookiesService = cookiesService;
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            Console.WriteLine("User is logged in, redirecting to Dashboard");
            return RedirectToAction("Index", "Dashboard");
        }
        return View(new LoginViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            Console.Write("Login Model invalid: " + string.Join(",", errors));
            return View(model);
        }
        var user = await _authenService.LoginUserAsync(model.Email, model.Password);
        if (user == null)
        {
            Console.WriteLine($"Login failed for email: {model.Email}");
            ViewData["MessageLogin"] = "Account Invalid, please try again!";
            return View(model);
        }
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role)
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

        _cookiesService.SetCookie("UserRole", user.Role, isPersistent: true);
        TempData["Notification"] = "You have successfully logged in.";
        Console.WriteLine("Login successful, TempData set, redirecting to Dashboard");
        return RedirectToAction("Index", "Dashboard");
    }
    
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        foreach (var item in Request.Cookies.Keys)
        {
            _cookiesService.DeleteCookie(item);
        }
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Notification"] = "You have successfully logged out.";
        Console.WriteLine("Logout successful, redirecting to Home/Index");
        return RedirectToAction("Index", "Home");
    }
}