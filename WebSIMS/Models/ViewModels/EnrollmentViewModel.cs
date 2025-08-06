namespace WebSIMS.Models.ViewModels;

public class EnrollmentViewModel
{
    public int Id { get; set; }
    
    public string StudentName { get; set; }
    
    public string CourseName { get; set; }
    public double? Grade { get; set; }
    public string LecturerName { get; set; }

}