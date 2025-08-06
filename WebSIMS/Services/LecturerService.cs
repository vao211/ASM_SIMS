using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;

namespace WebSIMS.Services;

public class LecturerService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    public LecturerService(ICourseRepository courseRepository, IEnrollmentRepository enrollmentRepository)
    {
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<CourseViewModel>> GetCourseAsync(int lecturerId)
    {
        var courses = await _courseRepository.GetCoursesByLecturerAsync(lecturerId);
        return courses.Select(c => ViewModelFactory.CreateCourseViewModel(c, c.Lecturer.Name)).ToList();
    }
    
    public async Task<CourseViewModel> GetCourseStudentsAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
        var studentNames = enrollments.Select(e => e.Student.Name).ToList();

        var viewModel = ViewModelFactory.CreateCourseViewModel(course, course.Lecturer.Name, studentNames);

        viewModel.Enrollments = enrollments
            .Select(e => ViewModelFactory.CreateEnrollmentViewModel(e, e.Student.Name, e.Courses.Name))
            .ToList();
    
        return viewModel;
    }

    public async Task GradeStudentAsync(int studentId, int courseId, double grade)
    {
        if (grade < 0 || grade > 10)
        {
            throw new ArgumentException("Grade must be between 0 and 10.");
        }
        var existingRecord = await _enrollmentRepository.GetEnrollmentsByStudentAsync(studentId)
            .ContinueWith(t => t.Result.FirstOrDefault(ar => ar.CourseId == courseId));
        if (existingRecord != null)
        {
            existingRecord.Grade = grade;
            await _enrollmentRepository.UpdateAsync(existingRecord);
        }
        else
        {
            var academicRecord = new Enrollments
            {
                StudentId = studentId,
                CourseId = courseId,
                Grade = grade
            };
            await _enrollmentRepository.AddAsync(academicRecord);
        }
        Console.WriteLine("AcademicRecord saved successfully.");
    }


    public async Task<List<EnrollmentViewModel>> GetCourseGradesAsync(int courseId)
    {
        var records = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
        
        return records.Select(r => new EnrollmentViewModel
        {
            Id = r.Id,
            CourseName = r.Courses.Name,
            StudentName = r.Student.Name,
            Grade = r.Grade
        }).ToList();
    }
}