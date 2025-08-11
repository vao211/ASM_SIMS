using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;
using WebSIMS.Services.Interfaces;

namespace WebSIMS.Services;

public class AdminService
{
    private readonly IAuthenService _authenService;
    private readonly ICourseRepository _courseRepository;
    private readonly IEnrollmentRepository _enrollmentRepository;
    private readonly IUserRepository _userRepository;

    public AdminService(IAuthenService authenService, ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository, IUserRepository userRepository)
    {
        _authenService = authenService;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
    }

    public async Task<List<Courses>> GetAllCoursesAsync()
    {
        return await _courseRepository.GetAllAsync();
    }
    
    public async Task<CreateUserViewModel> CreateUserAsync(CreateUserViewModel model)
    {
        var existingUser = await _userRepository.GetByEmailAsync(model.Email);
        if (existingUser != null)
        {
            throw new InvalidOperationException("Email is already in use.");
        }

        var user = await _authenService.CreateUserAsync(model);
        return ViewModelFactory.CreateCreateUserViewModel(user);
    }
    public async Task UpdateUserAsync(EditUserViewModel model)
    {
        var user = await _userRepository.GetByIdAsync(model.Id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        if (user.Email != model.Email)
        {
            var existingUser = await _userRepository.GetByEmailAsync(model.Email);
            if (existingUser != null && existingUser.Id != model.Id)
            {
                throw new InvalidOperationException("Email is already in use.");
            }
            user.Email = model.Email;
        }

        user.Name = model.Name;
        user.Role = model.Role;
        await _userRepository.UpdateAsync(user);
    }

    public async Task DeleteUserAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new InvalidOperationException("User not found.");
        }
        
        if (user.Role == "Lecturer")
        {
            var courses = await _courseRepository.GetCoursesByLecturerAsync(id);
            if (courses.Any())
            {
                throw new InvalidOperationException("Cannot delete lecturer with assigned courses.");
            }
        }
        else if (user.Role == "Student")
        {
            //enrollments
            var enrollments = await _enrollmentRepository.GetEnrollmentsByStudentAsync(id);
            foreach (var enrollment in enrollments)
            {
                await _enrollmentRepository.DeleteAsync(enrollment.Id);
            }
        }
        await _userRepository.DeleteAsync(id);
    }
    public async Task<List<Users>> GetAllStudentsAsync()
    {
        return await _userRepository.GetStudentsAsync();
    }
    
    public async Task AssignStudentToCourseAsync(int studentId, int courseId)
    {
        var student = await _userRepository.GetByIdAsync(studentId);
        if (student?.Role != "Student")
        {
            throw new InvalidOperationException("User is not a student.");
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
    
    public async Task<List<Users>> GetAllLecturersAsync()
    {
        return await _userRepository.GetLecturersAsync();
    }
    public async Task<List<Users>> GetAllUsersAsync()
    {
        return await _userRepository.GetAllAsync();
    }
    public async Task<List<Enrollments>> GetEnrollmentsByStudentId(int id)
    {
        return await _enrollmentRepository.GetEnrollmentsByStudentAsync(id);
    }
    public async Task<List<Enrollments>> GetEnrollmentsByCourseAsync(int courseId)
    {
        return await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
    }
    public async Task<List<Courses>> GetCoursesByLecturerAsync(int lecturerId)
    {
        return await _courseRepository.GetCoursesByLecturerAsync(lecturerId);
    }
}