using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSIMS.Models.ViewModels;

public class GradeStudentViewModel
{
    [Required]
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int StudentId { get; set; } 
    public string StudentName { get; set; } 
    public double? Grade { get; set; }
    public List<SelectListItem> StudentsList { get; set; } 
}