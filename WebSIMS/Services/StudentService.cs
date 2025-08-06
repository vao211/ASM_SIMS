using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;

namespace WebSIMS.Services;

public class StudentService
{
    private readonly IEnrollmentRepository _enrollmentRepository;

    public StudentService(IEnrollmentRepository enrollmentRepository)
    {
        _enrollmentRepository = enrollmentRepository;
    }

    public async Task<List<CourseViewModel>> GetAllCoursesEnrolled(int studentId)
    {
        var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(studentId);
        return enrollments.Select(e=>ViewModelFactory.CreateCourseViewModel(e.Courses,e.Courses.Lecturer.Name)).ToList();
    }
}
