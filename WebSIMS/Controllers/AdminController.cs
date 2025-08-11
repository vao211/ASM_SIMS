using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Services;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly CourseService _courseService;
    private readonly ICookiesService _cookieService;
    
    public AdminController(AdminService adminService, CourseService courseService, ICookiesService cookieService)
    {
        _adminService = adminService;
        _courseService = courseService;
        _cookieService = cookieService;
    }
    
    [HttpGet]
    public async Task<IActionResult> ViewEnrollments(int? courseId)
    {
        var courses = await _adminService.GetAllCoursesAsync();
        var model = new EnrollmentListViewModel
        {
            CoursesList = courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()
        };

        if (courseId.HasValue)
        {
            var enrollments = await _adminService.GetEnrollmentsByCourseAsync(courseId.Value);
            model.CourseId = courseId;
            model.Enrollments = enrollments.Select(e => ViewModelFactory.CreateEnrollmentViewModel(e, e.Student.Name, e.Courses.Name)).ToList();
        }

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> ViewStudent(int id)
    {
        var student =  _adminService.GetAllUsersAsync().Result.FirstOrDefault(u => u.Id == id);
        if (student == null || student.Role != "Student")
        {
            return NotFound();
        }
        var userEmail = student.Email;
        var enrollments = await _adminService.GetEnrollmentsByStudentId(id);
        var model = new StudentViewModel
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Enrollments = enrollments.Select(e => ViewModelFactory.CreateEnrollmentViewModel(e, e.Student.Name, e.Courses.Name)).ToList()
        };

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> ViewLecturer(int id)
    {
        var lecturer =  _adminService.GetAllUsersAsync().Result.FirstOrDefault(u=>u.Id == id);
        if (lecturer == null || lecturer.Role != "Lecturer")
        {
            return NotFound();
        }

        var courses = await _adminService.GetCoursesByLecturerAsync(id);
        var model = ViewModelFactory.CreateLecturerViewModel(lecturer, courses);
        return View(model);
    }
    
    [HttpGet]
    public IActionResult CreateUser()
    {
        return View(new CreateUserViewModel());
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _adminService.CreateUserAsync(model);
                TempData["Notification"] = "User created successfully";
                return RedirectToAction("CreateUser", "Admin");
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message); 
            }
        }
        return View(model);
    }
    [HttpGet]
    public async Task<IActionResult> UserManager()
    {
        var users = await _adminService.GetAllUsersAsync();
        var model = users.Select(u => ViewModelFactory.CreateCreateUserViewModel(u)).ToList();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> EditUser(int id)
    {
        var user =  _adminService.GetAllUsersAsync().Result.FirstOrDefault(u => u.Id == id);
        if (user == null)
        {
            return NotFound();
        }

        var model = ViewModelFactory.CreateCreateUserViewModel(user);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        try
        {
            await _adminService.UpdateUserAsync(model);
            TempData["Notification"] = $"User '{model.Name}' updated successfully.";
            return RedirectToAction("UserManager");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _adminService.DeleteUserAsync(id);
            TempData["Notification"] = "User deleted successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("UserManager");
    }
    [HttpGet]
    public async Task<IActionResult> CreateCourse()
    {
        var lecturerList = await _adminService.GetAllLecturersAsync();
        Console.WriteLine($"found {lecturerList.Count} lecturers");
        foreach (var lecturer in lecturerList)
        {
            Console.WriteLine($"Lecturer {lecturer.Id}, Name: {lecturer.Name} ");
        }
        
        var lecturersSelectList = lecturerList.Select(lecturer => new SelectListItem
        {
            Value = lecturer.Id.ToString(),
            Text = lecturer.Name
        }).ToList();
        
        var model = ViewModelFactory.CreateCreateCourseViewModel(new Courses(), lecturersSelectList);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCourse(CreateCourseViewModel model)
    {
        Console.WriteLine($"Form data received: Name={model.Name ?? "null"}, InstructorId={model.LecturerId}");
        
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            Console.WriteLine("ModelState invalid: " + string.Join(", ", errors));
            //dropbox
            model.lecturersList = (await _adminService.GetAllLecturersAsync()).Select(i => new SelectListItem
            {
                Value = i.Id.ToString(),
                Text = i.Name
            }).ToList();
            return View(model);
        }
        
        await _courseService.CreateCourseAsync(model);
        TempData["Notification"] = $"Course '{model.Name}' created successfully.";
        return RedirectToAction("CreateCourse", "Admin");
    }

    [HttpGet]
    public async Task<IActionResult> AssignStudentToCourse()
    {
        var studentList = await _adminService.GetAllStudentsAsync();
        var courseList = await _adminService.GetAllCoursesAsync();
        
        var model = ViewModelFactory.CreateAssignStudentToCourseViewModel(studentList, courseList);
        return View(model);
    }
    [HttpPost]
    public async Task<IActionResult> AssignStudentToCourse(AssignStudentToCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var students = await _adminService.GetAllStudentsAsync();
            var courses = await _adminService.GetAllCoursesAsync();
            model.StudentsList = students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
            model.CoursesList = courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(model);
        }
        try
        {
            await _adminService.AssignStudentToCourseAsync(model.StudentId, model.CourseId);
            TempData["Notification"] = "Student assigned to course successfully.";
            return RedirectToAction("AssignStudentToCourse", "Admin");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var students = await _adminService.GetAllStudentsAsync();
            var courses = await _adminService.GetAllCoursesAsync();
            model.StudentsList = students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList();
            model.CoursesList = courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList();
            return View(model);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> CourseManager()
    {
        var courses = await _adminService.GetAllCoursesAsync();
        var enrollments = new List<Enrollments>();
        foreach (var course in courses)
        {
            var courseEnrollments = await _adminService.GetEnrollmentsByCourseAsync(course.Id);
            enrollments.AddRange(courseEnrollments);
        }
        var model = ViewModelFactory.CreateCourseManagerViewModel(courses, enrollments);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> RemoveStudentFromCourse(int studentId, int courseId)
    {
        try
        {
            await _adminService.RemoveStudentFromCourseAsync(studentId, courseId);
            TempData["Notification"] = "Student removed from course successfully.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
        }
        return RedirectToAction("CourseManager");
    }


    [HttpGet]
    public async Task<IActionResult> EditCourse(int id)
    {
        var course =  _adminService.GetAllCoursesAsync().Result.FirstOrDefault(c => c.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        var lecturerList = await _adminService.GetAllLecturersAsync();
        var model = ViewModelFactory.CreateCreateCourseViewModel(course, lecturerList.Select(l => new SelectListItem
        {
            Value = l.Id.ToString(),
            Text = l.Name
        }).ToList());

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditCourse(CreateCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var lecturerList = await _adminService.GetAllLecturersAsync();
            model.lecturersList = lecturerList.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            }).ToList();
            return View(model);
        }

        try
        {
            await _courseService.UpdateCourseAsync(model);
            TempData["Notification"] = $"Course '{model.Name}' updated successfully.";
            return RedirectToAction("CourseManager");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var lecturerList = await _adminService.GetAllLecturersAsync();
            model.lecturersList = lecturerList.Select(l => new SelectListItem
            {
                Value = l.Id.ToString(),
                Text = l.Name
            }).ToList();
            return View(model);
        }
    }
}