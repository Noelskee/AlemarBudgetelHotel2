using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemarBudgetelHotel.Models
{
    public enum RoomType
    {
        Single,
        Double,
        Standard,
        Deluxe,
        SuperDeluxe,
        SuperDuper
    }

    public enum RoomStatus
    {
        Available,
        Occupied,
        Reserved,
        Maintenance,
        Cleaning
    }

    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoomNumber { get; set; }

        [Required]
        public RoomType Type { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price3Hours { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price12Hours { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price24Hours { get; set; }

        [Required]
        public RoomStatus Status { get; set; } = RoomStatus.Available;

        [StringLength(500)]
        public string Description { get; set; }

        public int Capacity { get; set; }

        [StringLength(1000)]
        public string? ImageUrl { get; set; }

        public int Floor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime? UpdatedAt { get; set; }

        // Navigation properties
        public virtual ICollection<Reservation> Reservations { get; set; }
        public virtual ICollection<HousekeepingTask> HousekeepingTasks { get; set; }
    }
}
