using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.Entities;

public class Courses
{
    public int Id { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    public int LecturerId { get; set; }
    
    public Users Lecturer { get; set; }
    
    public List<Enrollments> Enrollments { get; set; }
}