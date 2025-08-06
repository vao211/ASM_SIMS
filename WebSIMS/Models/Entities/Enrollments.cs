namespace WebSIMS.Models.Entities;

public class Enrollments
{
    public int Id { get; set; }
    
    public int StudentId { get; set; }
    
    public Users Student { get; set; }
    
    public int CourseId { get; set; }
    
    public Courses Courses { get; set; }
    public double? Grade { get; set; }
}