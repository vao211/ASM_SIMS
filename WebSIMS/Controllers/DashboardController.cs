using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Services;
using WebSIMS.Services.Interfaces;
using WebSIMS.Factory;
using WebSIMS.Repository.Interfaces;


namespace WebSIMS.Controllers
{

    [Authorize]
    public class DashboardController : Controller
    {
        private readonly AdminService _adminService;
        private readonly StudentService _studentService;
        private readonly LecturerService _lecturerService;
        private readonly IUserRepository _userRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IEnrollmentRepository _enrollmentRepository;
        private readonly ICookiesService _cookieService;

        public DashboardController(
            AdminService adminService,
            StudentService studentService,
            LecturerService lecturerService,
            IUserRepository userRepository,
            ICourseRepository courseRepository,
            IEnrollmentRepository enrollmentRepository,
            ICookiesService cookieService)
        {
            _adminService = adminService;
            _studentService = studentService;
            _lecturerService = lecturerService;
            _userRepository = userRepository;
            _courseRepository = courseRepository;
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

            DashboardViewModel model;

            switch (user.Role)
            {
                case "Admin":
                    var allCourses = await _adminService.GetAllCoursesAsync();
                    model = ViewModelFactory.CreateDashboardViewModel(user, _cookieService, courses: allCourses);
                    break;
                case "Lecturer":
                    var lecturerCourses = await _courseRepository.GetCoursesByLecturerAsync(user.Id);
                    model = ViewModelFactory.CreateDashboardViewModel(user, _cookieService, courses: lecturerCourses);
                    break;
                case "Student":
                    var enrollments =
                        await _enrollmentRepository
                            .GetEnrollmentsByStudentAsync(user.Id); 
                    model = ViewModelFactory.CreateDashboardViewModel(user, _cookieService, enrollments: enrollments);
                    break;
                default:
                    return Unauthorized();
            }

            return View(model);
        }
    }
}