using WebSIMS.Factory;
using WebSIMS.Models.Entities;
using WebSIMS.Models.ViewModels;
using WebSIMS.Repository;
using WebSIMS.Repository.Interfaces;


namespace WebSIMS.Services;

public class CourseService
{
    private readonly ICourseRepository _courseRepository;
    private readonly IUserRepository _userRepository;

    public CourseService(ICourseRepository courseRepository, IUserRepository userRepository)
    {
        _userRepository  = userRepository;
        _courseRepository = courseRepository;
    }

    public async Task<CourseViewModel> CreateCourseAsync(CreateCourseViewModel model)
    {
        var lecturer = await _userRepository.GetByIdAsync(model.LecturerId);
        if (lecturer == null || lecturer.Role != "Lecturer")
        {
            throw new InvalidOperationException("Invalid Lecturer ID or Lecturer not found");
        }
        var course = new Courses
        {
            Name = model.Name,
            LecturerId = lecturer.Id,
        };
        await _courseRepository.AddAsync(course);
        return ViewModelFactory.CreateCourseViewModel(course,"Lecturer Name");
    }
    public async Task UpdateCourseAsync(CreateCourseViewModel model)
    {
        var course = await _courseRepository.GetByIdAsync(model.Id);
        if (course == null)
        {
            throw new InvalidOperationException("Course not found.");
        }

        course.Name = model.Name;
        course.LecturerId = model.LecturerId;
        await _courseRepository.UpdateAsync(course);
    }
}