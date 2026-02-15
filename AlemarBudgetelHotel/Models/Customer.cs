using System;
using System.ComponentModel.DataAnnotations;

namespace AlemarBudgetelHotel.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

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

        [StringLength(500)]
        public string Address { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginAt { get; set; }

        // Navigation property
        public virtual ICollection<Reservation> Reservations { get; set; }
    }
}
