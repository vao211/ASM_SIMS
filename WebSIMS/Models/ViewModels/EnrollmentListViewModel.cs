using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using WebSIMS.Models.ViewModels;

namespace WebSIMS.Models.ViewModels;

public class EnrollmentListViewModel
{
    public int? CourseId { get; set; }
    public List<SelectListItem> CoursesList { get; set; } = new List<SelectListItem>();
    public List<EnrollmentViewModel> Enrollments { get; set; } = new List<EnrollmentViewModel>();

}