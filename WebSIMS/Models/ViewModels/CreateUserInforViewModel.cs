using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSIMS.Models.ViewModels
{
    public class CreateUserInforViewModel : IValidatableObject
    {
        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; } = DateTime.Today;

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

        [Required]
        [StringLength(50)]
        public string CodeId { get; set; } //StudentId/LecturerId

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; }

        [Required] [StringLength(50)] public string Role { get; set; }

        public SelectList? UsersList { get; set; }
        public SelectList? RoleList { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BirthDate >= DateTime.Today)
            {
                yield return new ValidationResult("Birth date cannot be today or in the future.", new[] { nameof(BirthDate) });
            }

            if (BirthDate > DateTime.Today.AddYears(-17))
            {
                yield return new ValidationResult("User must be at least 17 years old.", new[] { nameof(BirthDate) });
            }

            if (JoinDate > DateTime.Today)
            {
                yield return new ValidationResult("Join date cannot be in the future.", new[] { nameof(JoinDate) });
            }

            if (PhoneNumber.Length != 10)
            {
                yield return new ValidationResult("Phone number must have 10 digits.", new[] { nameof(PhoneNumber) });
            }
        }
    }
}