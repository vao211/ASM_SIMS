using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.ViewModels;

public class StudentViewModel
{
    public int Id { get; set; }
    
    public string Name { get; set; }
    public string Email { get; set; }
    
    public List<EnrollmentViewModel>? Enrollments { get; set; }
    
    //Student Infor
    public string StudentInfoName { get; set; }
    [DataType(DataType.Date)]
    public DateTime? JoinDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }
    public string StudentId { get; set; }
    public string PhoneNumber { get; set; }
}