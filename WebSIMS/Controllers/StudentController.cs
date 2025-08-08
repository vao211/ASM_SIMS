using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebSIMS.Factory;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;

[Authorize(Roles = "Student")]
public class StudentController : Controller
{
    private readonly IUserRepository _userRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly ICookiesService _cookieService;

    public StudentController(IUserRepository userRepository, IEnrollmentRepository enrollmentRepository, ICookiesService cookieService)
    {
        _userRepository = userRepository;
        _enrollmentRepository = enrollmentRepository;
        _cookieService = cookieService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userEmail = User.Identity.Name;
        var user = await _userRepository.GetByEmailAsync(userEmail);

        if (user == null)
        {
            return RedirectToAction("Login", "Authen");
        }

        var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(user.Id);
        var model = new StudentViewModel
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Enrollments = enrollments.Select(e => ViewModelFactory.CreateEnrollmentViewModel(e, e.Student.Name, e.Courses.Name)).ToList()
        };

        return View(model);
    }
}