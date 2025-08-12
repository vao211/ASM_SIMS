using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace WebSIMS.Models.ViewModels
{
    public class AssignUserInforViewModel
    {
        [Required]
        [StringLength(50)]
        public string Role { get; set; } // "Student" hoặc "Lecturer"

        [Required]
        public int InforId { get; set; } // Id của StudentInfor hoặc LecturerInfor

        [Required]
        public int UserId { get; set; } // Id của Users

        public SelectList? RoleList { get; set; } // Dropdown cho Role
        public SelectList? InforList { get; set; } // Dropdown cho StudentInfor hoặc LecturerInfor
        public SelectList? UserList { get; set; } // Dropdown cho Users

        // Thông tin hiển thị cho Infor
        public string? InforName { get; set; }
        public string? InforCodeId { get; set; }
        public DateTime? InforJoinDate { get; set; }
        public DateTime? InforBirthDate { get; set; }
        public string? InforPhoneNumber { get; set; }

        // Thông tin hiển thị cho User
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserRole { get; set; }
    }
}