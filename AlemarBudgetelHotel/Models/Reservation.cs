using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemarBudgetelHotel.Models
{
    public enum ReservationStatus
    {
        Pending,
        Confirmed,
        CheckedIn,
        CheckedOut,
        Cancelled,
        NoShow
    }

    public enum DurationOption
    {
        ThreeHours,
        TwelveHours,
        TwentyFourHours
    }

    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime CheckInDateTime { get; set; }

        [Required]
        public DateTime CheckOutDateTime { get; set; }

        [Required]
        public DurationOption Duration { get; set; }

        [Required]
        public int NumberOfGuests { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public ReservationStatus Status { get; set; } = ReservationStatus.Pending;

        [StringLength(500)]
        public string SpecialRequests { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        public int? CheckedInByAdminId { get; set; }
        public DateTime? ActualCheckInTime { get; set; }

        public int? CheckedOutByAdminId { get; set; }
        public DateTime? ActualCheckOutTime { get; set; }

        // Navigation properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        public virtual Payment Payment { get; set; }
    }
}
