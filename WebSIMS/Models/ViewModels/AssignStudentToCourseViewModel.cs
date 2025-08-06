using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSIMS.Models.ViewModels;

public class AssignStudentToCourseViewModel
{
    [Required(ErrorMessage = "Select Student !")]
    public int StudentId { get; set; }

    [Required(ErrorMessage = "Select Course !")]
    public int CourseId { get; set; }

    public List<SelectListItem>? StudentsList { get; set; }
    public List<SelectListItem>? CoursesList { get; set; }
}