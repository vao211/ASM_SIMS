using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Services;

namespace WebSIMS.Controllers;

[Authorize(Policy = "LecturerOnly")]
public class LecturerController : Controller
{
    private readonly LecturerService _lecturerService;
    private readonly IEnrollmentRepository _enrollmentRepository;

    public LecturerController(LecturerService lecturerService, IEnrollmentRepository enrollmentRepository)
    {
        _lecturerService = lecturerService;
        _enrollmentRepository = enrollmentRepository;
    }

    [HttpGet]
    public async Task<IActionResult> ViewCoursesStudents(int courseId)
    {
        var course = await _lecturerService.GetCourseStudentsAsync(courseId);
        if (course == null)
        {
            return NotFound("Course not found.");
        }
        return View(course);
    }

    [HttpGet]
    public async Task<IActionResult> GradeStudent(int courseId)
    {
        var course = await _lecturerService.GetCourseStudentsAsync(courseId);
        if (course == null)
        {
            Console.WriteLine("Course not found for CourseId: " + courseId);
            return NotFound("Course not found.");
        }

        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
        Console.WriteLine($"Enrollments count for CourseId={courseId}: {enrollments.Count}");

        var students = enrollments.Select(e => new Users
        {
            Id = e.StudentId,
            Name = e.Student.Name,
        }).ToList();
        Console.WriteLine($"Students count: {students.Count}");

        var model = ViewModelFactory.CreateGradeStudentViewModel(courseId, course.Name, students);
        Console.WriteLine($"StudentsList count in model: {model.StudentsList.Count}");
        return View(model);
    }

[HttpPost]
public async Task<IActionResult> GradeStudent(GradeStudentViewModel model, List<StudentGradeInputModel> Grades)
{
    Console.WriteLine($"Received model: CourseId={model.CourseId}, CourseName={model.CourseName}");
    Console.WriteLine($"Received Grades: {string.Join(", ", Grades.Select(g => $"StudentId={g.StudentId}, Grade={g.Grade}"))}");
    
    try
    {
        bool hasValidGrades = false;
        foreach (var grade in Grades)
        {
            if (grade.StudentId > 0 && grade.Grade.HasValue)
            {
                Console.WriteLine($"Processing grade for StudentId={grade.StudentId}, Grade={grade.Grade.Value}");
                await _lecturerService.GradeStudentAsync(grade.StudentId, model.CourseId, grade.Grade.Value);
                hasValidGrades = true;
            }
            else
            {
                Console.WriteLine($"Skipping invalid grade: StudentId={grade.StudentId}, Grade={grade.Grade}");
            }
        }

        if (!hasValidGrades)
        {
            Console.WriteLine("No valid grades provided.");
            ModelState.AddModelError("", "Enter valid grades.");
            var course = await _lecturerService.GetCourseStudentsAsync(model.CourseId);
            var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(model.CourseId);
            model.StudentsList = enrollments.Select(e => new SelectListItem
            {
                Value = e.StudentId.ToString(),
                Text = e.Student.Name
            }).ToList();
            return View(model);
        }

        Console.WriteLine("All valid grades saved successfully.");
        TempData["Notification"] = $"Grade saved for course: {model.CourseName}.";
        return RedirectToAction("ViewCoursesStudents", new { courseId = model.CourseId });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in GradeStudent: {ex.Message}\nStackTrace: {ex.StackTrace}");
        ModelState.AddModelError("", $"Erron in grading: {ex.Message}");
        var course = await _lecturerService.GetCourseStudentsAsync(model.CourseId);
        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(model.CourseId);
        model.StudentsList = enrollments.Select(e => new SelectListItem
        {
            Value = e.StudentId.ToString(),
            Text = e.Student.Name
        }).ToList();
        return View(model);
    }
}
    [HttpGet]
    public async Task<IActionResult> ViewCourseGrades(int courseId)
    {
        var course = await _lecturerService.GetCourseStudentsAsync(courseId);
        if (course == null)
        {
            return NotFound("Course not found.");
        }
        var grades = await _lecturerService.GetCourseGradesAsync(courseId);
        ViewBag.CourseName = course.Name;
        ViewBag.CourseId = course.Id;
        return View(grades);
    }

    public class StudentGradeInputModel
    {
        public int StudentId { get; set; }
        public double? Grade { get; set; }
    }
}