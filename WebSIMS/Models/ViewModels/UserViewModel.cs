using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.ViewModels;

public class UserViewModel
{
    public int Id { get; set; }
    
    [Required]
    public string Email { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Role { get; set; }
}