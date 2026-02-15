using System;
using System.ComponentModel.DataAnnotations;

namespace AlemarBudgetelHotel.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

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

        [Phone]
        [StringLength(20)]
        public string PhoneNumber { get; set; }

        [StringLength(50)]
        public string Role { get; set; } // SuperAdmin, Manager, Receptionist, etc.

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginAt { get; set; }
    }
}
