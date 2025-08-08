namespace WebSIMS.Models.ViewModels;

public class LecturerViewModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public List<CourseViewModel> Courses { get; set; }
}