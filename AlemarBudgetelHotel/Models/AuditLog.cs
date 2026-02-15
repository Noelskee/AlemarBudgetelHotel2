using System;
using System.ComponentModel.DataAnnotations;

namespace AlemarBudgetelHotel.Models
{
    public enum LogType
    {
        Login,
        Logout,
        Registration,
        ReservationCreated,
        ReservationModified,
        ReservationCancelled,
        CheckIn,
        CheckOut,
        PaymentReceived,
        RoomStatusChanged,
        AdminAction,
        SecurityAlert,
        SystemError
    }

    public class AuditLog
    {
        [Key]
        public int LogId { get; set; }

        [Required]
        public LogType LogType { get; set; }

        [Required]
        [StringLength(50)]
        public string UserType { get; set; } // "Admin" or "Customer"

        public int? UserId { get; set; }

        [StringLength(200)]
        public string Username { get; set; }

        [Required]
        [StringLength(500)]
        public string Action { get; set; }

        [StringLength(2000)]
        public string Details { get; set; }

        [StringLength(50)]
        public string IpAddress { get; set; }

        [StringLength(500)]
        public string UserAgent { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
