namespace WebSIMS.Models.ViewModels;

public class CourseManagerViewModel
{
    public List<CourseInfo> Courses { get; set; } = new List<CourseInfo>();
}

public class CourseInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string LecturerName { get; set; }
    public List<StudentInfo> Students { get; set; } = new List<StudentInfo>();
}

public class StudentInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
}