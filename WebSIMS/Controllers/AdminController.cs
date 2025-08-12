using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository.Interfaces;
using WebSIMS.Services;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Controllers;

[Authorize(Policy = "AdminOnly")]
public class AdminController : Controller
{
    private readonly AdminService _adminService;
    private readonly CourseService _courseService;
    private readonly ICookiesService _cookieService;
    private readonly IStudentInforRepository  _studentInforRepository;
    private readonly ILecturerInforRepository _lecturerInforRepository;
    
    public AdminController(AdminService adminService, CourseService courseService, 
        ICookiesService cookieService,  IStudentInforRepository studentInforRepository,
        ILecturerInforRepository lecturerInforRepository)
    {
        _adminService = adminService;
        _courseService = courseService;
        _cookieService = cookieService;
        _studentInforRepository = studentInforRepository;
        _lecturerInforRepository = lecturerInforRepository;
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
        var enrollments = await _adminService.GetEnrollmentsByStudentId(id);
        
        var studentInfo = await _studentInforRepository.GetAllAsync();
        var studentInfor = studentInfo.FirstOrDefault(si => si.UserId == id);
        
        var model = new StudentViewModel
        {
            Id = student.Id,
            Name = student.Name,
            Email = student.Email,
            Enrollments = enrollments.Select(e => ViewModelFactory.CreateEnrollmentViewModel(e, e.Student.Name, e.Courses.Name)).ToList(),
            StudentInfoName = studentInfor?.Name,
            JoinDate = studentInfor?.JoinDate,
            BirthDate = studentInfor?.BirthDate,
            StudentId = studentInfor?.StudentId,
            PhoneNumber = studentInfor?.PhoneNumber
        };

        return View(model);
    }
    
    [HttpGet]
    public async Task<IActionResult> ViewLecturer(int id)
    {
        var lecturer = _adminService.GetAllUsersAsync().Result.FirstOrDefault(u => u.Id == id);
        if (lecturer == null || lecturer.Role != "Lecturer")
        {
            return NotFound();
        }

        var courses = await _adminService.GetCoursesByLecturerAsync(id);
        var lecturerInfo = await _lecturerInforRepository.GetAllAsync();
        var lecturerInfor = lecturerInfo.FirstOrDefault(li => li.UserId == id);

        var model = new LecturerViewModel
        {
            Id = lecturer.Id,
            Name = lecturer.Name,
            Email = lecturer.Email,
            Courses = courses.Select(c => ViewModelFactory.CreateCourseViewModel(c, c.Lecturer.Name)).ToList(),
            LecturerInfoName = lecturerInfor?.Name,
            JoinDate = lecturerInfor?.JoinDate,
            BirthDate = lecturerInfor?.BirthDate,
            LecturerId = lecturerInfor?.LecturerId,
            PhoneNumber = lecturerInfor?.PhoneNumber
        };

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
    public async Task<IActionResult> StudentManager()
    {
        var students = await _adminService.GetAllStudentsAsync();
        var model = students.Select(s => ViewModelFactory.CreateCreateUserViewModel(s)).ToList();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> LecturerManager()
    {
        var lecturers = await _adminService.GetAllLecturersAsync();
        var model = lecturers.Select(l => ViewModelFactory.CreateCreateUserViewModel(l)).ToList();
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

        var model = ViewModelFactory.CreateEditUserViewModel(user);
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUser(EditUserViewModel model)
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
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
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

    [HttpPost]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        try
        {
            await _adminService.DeleteCourseAsync(id);
            TempData["Notification"] = "Course deleted successfully.";
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
    
    
    [HttpGet]
    public async Task<IActionResult> CreateUserInfor()
    {
        var users = await _adminService.GetAllUsersAsync();
        var model = new CreateUserInforViewModel
        {
            UsersList = new SelectList(users, "Id", "Name"),
            RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text")
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUserInfor(CreateUserInforViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var users = await _adminService.GetAllUsersAsync();
            model.UsersList = new SelectList(users, "Id", "Name");
            model.RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text");
            return View(model);
        }

        try
        {
            await _adminService.CreateUserInforAsync(model);
            TempData["Notification"] = $"{model.Role} information created successfully.";
            return RedirectToAction("CreateUserInfor");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var users = await _adminService.GetAllUsersAsync();
            model.UsersList = new SelectList(users, "Id", "Name");
            model.RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text");
            return View(model);
        }
    }
    
    [HttpGet]
    public async Task<IActionResult> EditUserInfor()
    {
        var users = await _adminService.GetAllUsersAsync();
        var model = new EditUserInforViewModel
        {
            UsersList = new SelectList(users, "Id", "Name"),
            RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text")
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> EditUserInfor(EditUserInforViewModel model)
    {
        if (!ModelState.IsValid)
        {
            var users = await _adminService.GetAllUsersAsync();
            model.UsersList = new SelectList(users, "Id", "Name");
            model.RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text");
            model.InforList = new SelectList(await _adminService.GetInforListByRoleAsync(model.Role), "Id", "Name");
            return View(model);
        }

        try
        {
            await _adminService.UpdateUserInforAsync(model);
            TempData["Notification"] = $"{model.Role} information updated successfully.";
            return RedirectToAction("EditUserInfor");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            var users = await _adminService.GetAllUsersAsync();
            model.UsersList = new SelectList(users, "Id", "Name");
            model.RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text");
            model.InforList = new SelectList(await _adminService.GetInforListByRoleAsync(model.Role), "Id", "Name");
            return View(model);
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> ManagerDeleteUserInfor(int id, string role)
    {
        try
        {
            await _adminService.DeleteUserInforAsync(id, role);
            TempData["Notification"] = $"{role} information deleted successfully.";
            return Json(new { success = true });
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return Json(new { success = false, error = ex.Message });
        }
    }
    [HttpPost]
    public async Task<IActionResult> DeleteUserInfor(EditUserInforViewModel model)
    {
        try
        {
            await _adminService.DeleteUserInforAsync(model.InforId, model.Role);
            TempData["Notification"] = $"{model.Role} information deleted successfully.";
            return RedirectToAction("EditUserInfor");
        }
        catch (InvalidOperationException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToAction("EditUserInfor");
        }
    }

    [HttpGet]
    public async Task<JsonResult> GetInforList(string role)
    {
        var inforList = await _adminService.GetInforListByRoleAsync(role);
        return Json(inforList.Select(i => new { Id = i.Id, Name = i.Name }));
    }

    [HttpGet]
    public async Task<JsonResult> GetInforDetails(int id, string role)
    {
        var details = await _adminService.GetInforDetailsAsync(id, role);
        return Json(details);
    }
    
    
    
    [HttpGet]
    public async Task<JsonResult> GetUnassignedUsers(string role)
    {
        var users = await _adminService.GetUnassignedUsersByRoleAsync(role);
        return Json(users.Select(u => new { Id = u.Id, Name = u.Name }));
    }
    
    [HttpGet]
    public async Task<JsonResult> GetUserDetails(int id)
    {
        var details = await _adminService.GetUserDetailsAsync(id);
        return Json(details);
    }
    
    [HttpGet]
    public async Task<IActionResult> AssignUserInfor()
    {
        var model = new AssignUserInforViewModel
        {
            RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text")
        };
        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> AssignUserInfor(AssignUserInforViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text");
            model.InforList = new SelectList(await _adminService.GetInforListByRoleAsync(model.Role), "Id", "Name");
            model.UserList = new SelectList(await _adminService.GetUnassignedUsersByRoleAsync(model.Role), "Id", "Name");
            return View(model);
        }

        try
        {
            await _adminService.AssignUserInforAsync(model);
            TempData["Notification"] = $"User assigned to {model.Role} successfully.";
            return RedirectToAction("AssignUserInfor");
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text");
            model.InforList = new SelectList(await _adminService.GetInforListByRoleAsync(model.Role), "Id", "Name");
            model.UserList = new SelectList(await _adminService.GetUnassignedUsersByRoleAsync(model.Role), "Id", "Name");
            return View(model);
        }
    }
    
    [HttpGet]
    public IActionResult InforManager()
    {
        var model = new InforManagerViewModel
        {
            RoleList = new SelectList(new[]
            {
                new { Value = "Student", Text = "Student" },
                new { Value = "Lecturer", Text = "Lecturer" }
            }, "Value", "Text")
        };
        return View(model);
    }

    [HttpGet]
    public async Task<JsonResult> GetInforListByRole(string role)
    {
        if (role == "Student")
        {
            var students = await _adminService.GetAllStudentInforAsync();
            return Json(students.Select(s => new
            {
                Id = s.StudentInfoId,
                Name = s.Name,
                JoinDate = s.JoinDate.ToString("yyyy-MM-dd"),
                BirthDate = s.BirthDate.ToString("yyyy-MM-dd"),
                CodeId = s.StudentId,
                PhoneNumber = s.PhoneNumber,
                UserId = s.UserId.HasValue ? s.UserId.ToString() : "Chưa gán"
            }));
        }
        else if (role == "Lecturer")
        {
            var lecturers = await _adminService.GetAllLecturerInforAsync();
            return Json(lecturers.Select(l => new
            {
                Id = l.LecturerInfoId,
                Name = l.Name,
                JoinDate = l.JoinDate.ToString("yyyy-MM-dd"),
                BirthDate = l.BirthDate.ToString("yyyy-MM-dd"),
                CodeId = l.LecturerId,
                PhoneNumber = l.PhoneNumber,
                UserId = l.UserId.HasValue ? l.UserId.ToString() : "Chưa gán"
            }));
        }
        return Json(new List<object>());
    }
}