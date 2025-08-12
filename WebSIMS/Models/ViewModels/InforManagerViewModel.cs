using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSIMS.Models.ViewModels
{
    public class InforManagerViewModel
    {
        public string Role { get; set; }
        public SelectList? RoleList { get; set; } 

        public List<object>? InforList { get; set; }
    }
}