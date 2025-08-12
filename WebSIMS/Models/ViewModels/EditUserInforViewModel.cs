using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSIMS.Models.ViewModels
{
    public class EditUserInforViewModel : IValidatableObject
    {
        public int InforId { get; set; } // Id của StudentInfor hoặc LecturerInfor

        [Required]
        [StringLength(255)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime JoinDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }

        [Required]
        [StringLength(50)]
        public string CodeId { get; set; } // StudentId hoặc LecturerId

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; } // Có thể null

        [Required]
        [StringLength(50)]
        public string Role { get; set; } // "Student" hoặc "Lecturer"

        public SelectList? UsersList { get; set; } // Dropdown cho UserId
        public SelectList? RoleList { get; set; } // Dropdown cho Role
        public SelectList? InforList { get; set; } // Dropdown cho tên từ bảng Infor

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Kiểm tra BirthDate không được là hôm nay hoặc tương lai
            if (BirthDate >= DateTime.Today)
            {
                yield return new ValidationResult("Birth date cannot be today or in the future.", new[] { nameof(BirthDate) });
            }

            // Kiểm tra độ tuổi tối thiểu là 17
            if (BirthDate > DateTime.Today.AddYears(-17))
            {
                yield return new ValidationResult("User must be at least 17 years old.", new[] { nameof(BirthDate) });
            }

            // Kiểm tra JoinDate không được là tương lai
            if (JoinDate > DateTime.Today)
            {
                yield return new ValidationResult("Join date cannot be in the future.", new[] { nameof(JoinDate) });
            }
        }
    }
}