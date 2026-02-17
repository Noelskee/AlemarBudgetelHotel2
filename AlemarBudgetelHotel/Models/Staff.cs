using System;
using System.ComponentModel.DataAnnotations;

namespace AlemarBudgetelHotel.Models
{
    public enum StaffRole
    {
        Staff,
        Manager,
        Receptionist,
        Housekeeper
    }

    public class Staff
    {
        [Key]
        public int StaffId { get; set; }

        [Required]
        [StringLength(100)]
        public string FullName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        [Required]
        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [Required]
        public StaffRole Role { get; set; } = StaffRole.Staff;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginAt { get; set; }
    }
}
