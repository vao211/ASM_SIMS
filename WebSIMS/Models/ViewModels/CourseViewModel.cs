namespace WebSIMS.Models.ViewModels;

public class CourseViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }

    public string? LecturerName { get; set; }

    public List<string>? StudentNames { get; set; }

    public int? LecturerId { get; set; }
    
    public List<EnrollmentViewModel>? Enrollments { get; set; }
    
}