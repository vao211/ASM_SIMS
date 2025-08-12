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
    private readonly IStudentInforRepository _studentInforRepository;
    private readonly ILecturerInforRepository _lecturerInforRepository;

    public AdminService(IAuthenService authenService, ICourseRepository courseRepository,
        IEnrollmentRepository enrollmentRepository, IUserRepository userRepository,
        IStudentInforRepository studentInforRepository,  ILecturerInforRepository lecturerInforRepository)
    {
        _authenService = authenService;
        _courseRepository = courseRepository;
        _enrollmentRepository = enrollmentRepository;
        _userRepository = userRepository;
        _studentInforRepository = studentInforRepository;
        _lecturerInforRepository = lecturerInforRepository;
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
        
        var existingEnrollment = await _enrollmentRepository.GetEnrollmentAsync(studentId, courseId);
        if (existingEnrollment != null)
        {
            throw new InvalidOperationException("Student is already enrolled in this course.");
        }

        var enrollment = new Enrollments
        {
            StudentId = studentId,
            CourseId = courseId
        };
        await _enrollmentRepository.AddAsync(enrollment);
    }
    
    public async Task DeleteCourseAsync(int courseId)
    {
        var course = await _courseRepository.GetByIdAsync(courseId);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        var enrollments = await _enrollmentRepository.GetEnrollmentsByCourseAsync(courseId);
        if (enrollments.Any())
        {
            throw new InvalidOperationException("Cannot delete course with enrolled students.");
        }

        await _courseRepository.DeleteAsync(courseId);
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
    
    public async Task CreateUserInforAsync(CreateUserInforViewModel model)
    {
        if (model.Role != "Student" && model.Role != "Lecturer")
        {
            throw new InvalidOperationException("Invalid role selected.");
        }

        if (model.UserId.HasValue)
        {
            var user = await _userRepository.GetByIdAsync(model.UserId.Value);
            if (user == null)
            {
                throw new InvalidOperationException("Selected user does not exist.");
            }
            if (user.Role != model.Role)
            {
                throw new InvalidOperationException($"Selected user role ({user.Role}) does not match the selected information type ({model.Role}).");
            }
        }

        if (model.Role == "Student")
        {
            var existingStudentInfor = await _studentInforRepository.GetByStudentIdAsync(model.CodeId);
            if (existingStudentInfor != null)
            {
                throw new InvalidOperationException("Student ID is already in use.");
            }

            var studentInfor = new StudentInfor
            {
                Name = model.Name,
                JoinDate = model.JoinDate,
                BirthDate = model.BirthDate,
                StudentId = model.CodeId,
                PhoneNumber = model.PhoneNumber,
                UserId = model.UserId
            };
            await _studentInforRepository.AddAsync(studentInfor);
        }
        else // Lecturer
        {
            var existingLecturerInfor = await _lecturerInforRepository.GetByLecturerIdAsync(model.CodeId);
            if (existingLecturerInfor != null)
            {
                throw new InvalidOperationException("Lecturer ID is already in use.");
            }

            var lecturerInfor = new LecturerInfor
            {
                Name = model.Name,
                JoinDate = model.JoinDate,
                BirthDate = model.BirthDate,
                LecturerId = model.CodeId,
                PhoneNumber = model.PhoneNumber,
                UserId = model.UserId
            };
            await _lecturerInforRepository.AddAsync(lecturerInfor);
        }
    }
    
    
     public async Task<List<InforListItem>> GetInforListByRoleAsync(string role)
    {
        if (role == "Student")
        {
            var students = await _studentInforRepository.GetAllAsync();
            return students.Select(s => new InforListItem { Id = s.StudentInfoId, Name = s.Name }).ToList();
        }
        else if (role == "Lecturer")
        {
            var lecturers = await _lecturerInforRepository.GetAllAsync();
            return lecturers.Select(l => new InforListItem { Id = l.LecturerInfoId, Name = l.Name }).ToList();
        }
        throw new InvalidOperationException("Invalid role.");
    }

    public async Task<object> GetInforDetailsAsync(int id, string role)
    {
        if (role == "Student")
        {
            var student = await _studentInforRepository.GetByIdAsync(id);
            if (student == null) throw new InvalidOperationException("Student information not found.");
            return new
            {
                Name = student.Name,
                JoinDate = student.JoinDate,
                BirthDate = student.BirthDate,
                CodeId = student.StudentId,
                PhoneNumber = student.PhoneNumber,
                UserId = student.UserId
            };
        }
        else if (role == "Lecturer")
        {
            var lecturer = await _lecturerInforRepository.GetByIdAsync(id);
            if (lecturer == null) throw new InvalidOperationException("Lecturer information not found.");
            return new
            {
                Name = lecturer.Name,
                JoinDate = lecturer.JoinDate,
                BirthDate = lecturer.BirthDate,
                CodeId = lecturer.LecturerId,
                PhoneNumber = lecturer.PhoneNumber,
                UserId = lecturer.UserId
            };
        }
        throw new InvalidOperationException("Invalid role.");
    }

    public async Task UpdateUserInforAsync(EditUserInforViewModel model)
    {
        if (model.Role == "Student")
        {
            var student = await _studentInforRepository.GetByIdAsync(model.InforId);
            if (student == null) throw new InvalidOperationException("Student information not found.");

            var existingCode = await _studentInforRepository.GetByStudentIdAsync(model.CodeId);
            if (existingCode != null && existingCode.StudentInfoId != model.InforId)
                throw new InvalidOperationException("Student ID is already in use.");

            student.Name = model.Name;
            student.JoinDate = model.JoinDate;
            student.BirthDate = model.BirthDate;
            student.StudentId = model.CodeId;
            student.PhoneNumber = model.PhoneNumber;
            student.UserId = model.UserId;
            await _studentInforRepository.UpdateAsync(student);
        }
        else if (model.Role == "Lecturer")
        {
            var lecturer = await _lecturerInforRepository.GetByIdAsync(model.InforId);
            if (lecturer == null) throw new InvalidOperationException("Lecturer information not found.");

            var existingCode = await _lecturerInforRepository.GetByLecturerIdAsync(model.CodeId);
            if (existingCode != null && existingCode.LecturerInfoId != model.InforId)
                throw new InvalidOperationException("Lecturer ID is already in use.");

            lecturer.Name = model.Name;
            lecturer.JoinDate = model.JoinDate;
            lecturer.BirthDate = model.BirthDate;
            lecturer.LecturerId = model.CodeId;
            lecturer.PhoneNumber = model.PhoneNumber;
            lecturer.UserId = model.UserId;
            await _lecturerInforRepository.UpdateAsync(lecturer);
        }
        else
        {
            throw new InvalidOperationException("Invalid role.");
        }
    }

    public async Task DeleteUserInforAsync(int id, string role)
    {
        if (role == "Student")
        {
            await _studentInforRepository.DeleteAsync(id);
        }
        else if (role == "Lecturer")
        {
            await _lecturerInforRepository.DeleteAsync(id);
        }
        else
        {
            throw new InvalidOperationException("Invalid role.");
        }
    }
    
    
    public async Task<List<StudentInfor>> GetAllStudentInforAsync()
    {
        return await _studentInforRepository.GetAllAsync();
    }

    public async Task<List<LecturerInfor>> GetAllLecturerInforAsync()
    {
        return await _lecturerInforRepository.GetAllAsync();
    }

    
    public async Task<List<Users>> GetUnassignedUsersByRoleAsync(string role)
    {
        return await _userRepository.GetUnassignedUsersByRoleAsync(role);
    }

    public async Task AssignUserInforAsync(AssignUserInforViewModel model)
    {
        var user = await _userRepository.GetByIdAsync(model.UserId);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        if (user.Role != model.Role)
            throw new InvalidOperationException($"User role ({user.Role}) does not match selected role ({model.Role}).");

        if (model.Role == "Student")
        {
            var student = await _studentInforRepository.GetByIdAsync(model.InforId);
            if (student == null)
                throw new InvalidOperationException("Student information not found.");

            if (student.UserId.HasValue)
                throw new InvalidOperationException("Student is already assigned to a user.");

            student.UserId = model.UserId;
            await _studentInforRepository.UpdateAsync(student);
        }
        else if (model.Role == "Lecturer")
        {
            var lecturer = await _lecturerInforRepository.GetByIdAsync(model.InforId);
            if (lecturer == null)
                throw new InvalidOperationException("Lecturer information not found.");

            if (lecturer.UserId.HasValue)
                throw new InvalidOperationException("Lecturer is already assigned to a user.");

            lecturer.UserId = model.UserId;
            await _lecturerInforRepository.UpdateAsync(lecturer);
        }
        else
        {
            throw new InvalidOperationException("Invalid role.");
        }
    }

    public async Task<object> GetUserDetailsAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new InvalidOperationException("User not found.");

        return new
        {
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }
}
