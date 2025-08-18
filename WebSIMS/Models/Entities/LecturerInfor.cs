using System;
using System.ComponentModel.DataAnnotations;

namespace WebSIMS.Models.Entities
{
    public class LecturerInfor
    {
        [Key]
        public int LecturerInfoId { get; set; }

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
        public string LecturerId { get; set; }

        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        public int? UserId { get; set; } 

        public Users? User { get; set; }
    }
}