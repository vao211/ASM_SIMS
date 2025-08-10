using WebSIMS.Models.Entities;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;

namespace WebSIMS.Services;

public class EnrollmentService 
{
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICourseRepository _courseRepository;

    public EnrollmentService(IEnrollmentRepository enrollmentRepository, IUserRepository userRepository, ICourseRepository courseRepository)
    {
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _courseRepository = courseRepository;
    }
    public async Task AssignStudentToCourseAsync(int studentId, int courseId)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        if (student?.Role != "Student")
        {
            throw new InvalidOperationException("User is not a student.");
        }
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }
        var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(studentId, courseId);
        if (existingEnrollment != null)
        {
            throw new InvalidOperationException("Student already assigned to this course.");
        }
        var enrollment = new Enrollments
        {
            
            StudentId = studentId,
            CourseId = courseId
        };
        await _enrollmentRepository.AddAsync(enrollment);
    }

    public async Task RemoveStudentFromCourseAsync(int studentId, int courseId)
    {
        var enrollment = await _enrollmentRepository.GetEnrollmentAsync(studentId, courseId);
        if (enrollment == null)
        {
            throw new InvalidOperationException("Enrollment not found.");
        }
        await _enrollmentRepository.DeleteAsync(enrollment.Id);
    }

    public async Task<List<Enrollments>> GetEnrollmentsByCourseAsync(int courseId)
    {
        return await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
    }

    public async Task<List<Enrollments>> GetEnrollmentsByStudentAsync(int studentId)
    {
        return await _enrollmentRepository.GetEnrollmentsByStudentAsync(studentId);
    }
}