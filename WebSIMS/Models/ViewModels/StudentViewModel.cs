namespace WebSIMS.Models.ViewModels;

public class StudentViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    
    public List<EnrollmentViewModel>? Enrollments { get; set; }
}