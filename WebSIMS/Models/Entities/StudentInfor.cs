using System;
using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.Entities
{
    public class StudentInfor
    {
        [Key]
        public int StudentInfoId { get; set; }

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
        public string StudentId { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; } // Có thể null, không bắt buộc là khóa ngoại

        public Users? User { get; set; } // Navigation property (tùy chọn)
    }
}