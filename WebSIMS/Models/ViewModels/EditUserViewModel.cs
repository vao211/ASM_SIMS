using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.ViewModels;

public class EditUserViewModel
{
    public int Id { get; set; }
    
    [Required]
    [EmailAddress]
    public string Email { get; set; }
    
    [Required]
    public string Name { get; set; }
    
    [Required]
    public string Role { get; set; }
}