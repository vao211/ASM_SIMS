namespace WebSIMS.Models.ViewModels;

public class DashboardViewModel
{
    public string UserName { get; set; }
    public string Role { get; set; }
    public List<CourseViewModel> Courses { get; set;}
    public List<EnrollmentViewModel> Enrollments { get; set; } 
    
}