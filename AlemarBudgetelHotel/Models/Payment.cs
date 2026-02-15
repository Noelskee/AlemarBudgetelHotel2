using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemarBudgetelHotel.Models
{
    public enum PaymentStatus
    {
        Pending,
        Completed,
        Failed,
        Refunded
    }

    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int ReservationId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentMethod { get; set; } = "GCash";

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        [StringLength(100)]
        public string TransactionReference { get; set; }

        [StringLength(100)]
        public string GCashReferenceNumber { get; set; }

        [StringLength(20)]
        public string GCashPhoneNumber { get; set; }

        [StringLength(500)]
        public string PaymentProof { get; set; } // Path to uploaded proof

        public DateTime? PaidAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation property
        [ForeignKey("ReservationId")]
        public virtual Reservation Reservation { get; set; }
    }
}
