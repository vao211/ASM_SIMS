using Microsoft.AspNetCore.Mvc.Rendering;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Services;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Factory;

public static class ViewModelFactory
{
    public static CreateUserViewModel CreateCreateUserViewModel(Users users)
    {
        return new CreateUserViewModel
        {
            Id = users.Id,
            Name = users.Name,
            Email = users.Email,
            Role = users.Role
        };
    }

    public static EditUserViewModel CreateEditUserViewModel(Users users)
    {
        return new EditUserViewModel
        {
            Id = users.Id,
            Name = users.Name,
            Email = users.Email,
            Role = users.Role
        };
    }
    public static CreateCourseViewModel CreateCreateCourseViewModel(Courses courses, List<SelectListItem> lecturerList)
    {
        return new CreateCourseViewModel
        {
            Id = courses.Id,
            Name = courses.Name,
            LecturerId = courses.LecturerId,
            lecturersList = lecturerList
        };
    }
    public static LecturerViewModel CreateLecturerViewModel(Users lecturer, List<Courses> courses)
    {
        return new LecturerViewModel
        {
            Id = lecturer.Id,
            Name = lecturer.Name,
            Email = lecturer.Email,
            Courses = courses.Select(c => CreateCourseViewModel(c, lecturer.Name)).ToList()
        };
    }

    public static CourseViewModel CreateCourseViewModel(Courses courses, string lecturerName, List<string> studentNames = null)
    {
        return new CourseViewModel
        {
            Id = courses.Id,
            Name = courses.Name,
            LecturerName = lecturerName,
            StudentNames = studentNames ?? new List<string>(),
            LecturerId = courses.Id,
            Enrollments = null
        };
    }
    public static CourseManagerViewModel CreateCourseManagerViewModel(List<Courses> courses, List<Enrollments> enrollments)
    {
        var model = new CourseManagerViewModel();
        foreach (var course in courses)
        {
            var courseEnrollments = enrollments.Where(e => e.CourseId == course.Id).ToList();
            var studentNames = courseEnrollments.Select(e => new { e.Student.Id, e.Student.Name }).Distinct().ToList();
            model.Courses.Add(new CourseInfo
            {
                Id = course.Id,
                Name = course.Name,
                LecturerName = course.Lecturer?.Name ?? "N/A",
                Students = studentNames.Select(s => new StudentInfo { Id = s.Id, Name = s.Name }).ToList()
            });
        }
        return model;
    }
    public static GradeStudentViewModel CreateGradeStudentViewModel(int courseId, string courseName, List<Users> students)
    {
        return new GradeStudentViewModel
        {
            CourseId = courseId,
            CourseName = courseName,
            StudentsList = students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList()
        };
    }
    
    public static AssignStudentToCourseViewModel CreateAssignStudentToCourseViewModel(List<Users> students, List<Courses> courses)
    {
        return new AssignStudentToCourseViewModel
        {
            StudentsList = students.Select(s => new SelectListItem
            {
                Value = s.Id.ToString(),
                Text = s.Name
            }).ToList(),
            CoursesList = courses.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            }).ToList()
        };
    }

public static EnrollmentViewModel CreateEnrollmentViewModel(Enrollments enrollments, string studentName, string courseName)
{
    return new EnrollmentViewModel
    {
        Id = enrollments.Id,
        StudentName = studentName,
        CourseName = courseName,
        LecturerName = enrollments.Courses?.Lecturer?.Name ?? "N/A",
        Grade = enrollments.Grade
    };
}

    public static DashboardViewModel CreateDashboardViewModel(Users users, ICookiesService cookieService, 
                                                            List<Courses> courses = null, List<Enrollments> enrollments = null)
    {
        return new DashboardViewModel
        {
            UserName = users.Name,
            Role = users.Role,
            Courses = courses?.Select(c => CreateCourseViewModel(c, c.Lecturer?.Name ?? "N/A")).ToList() ?? new List<CourseViewModel>(),
            Enrollments = enrollments?.Select(e => CreateEnrollmentViewModel(e, e.Student.Name, e.Courses.Name)).ToList() ?? new List<EnrollmentViewModel>(),
        };
    }
}