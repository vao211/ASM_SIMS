using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebSIMS.Models.Entities;

namespace WebSIMS.Models.ViewModels;

public class CreateCourseViewModel
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; }
    
    [Required(ErrorMessage = "Instructor Id is required")]
    public int LecturerId { get; set; }
    
    public string? LecturerName { get; set; }
    
    public List<SelectListItem>? lecturersList { get; set; }

}