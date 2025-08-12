using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.ViewModels;

public class LecturerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<CourseViewModel> Courses { get; set; }
    
    //Lecturer Infor
    public string LecturerInfoName { get; set; }
    
    [DataType(DataType.Date)]
    public DateTime? JoinDate { get; set; }
    [DataType(DataType.Date)]
    public DateTime? BirthDate { get; set; }
    public string LecturerId { get; set; }
    public string PhoneNumber { get; set; }
}