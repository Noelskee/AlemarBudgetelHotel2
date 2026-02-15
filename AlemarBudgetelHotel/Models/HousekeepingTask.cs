using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AlemarBudgetelHotel.Models
{
    public enum TaskStatus
    {
        Pending,
        InProgress,
        Completed,
        Cancelled
    }

    public enum TaskPriority
    {
        Low,
        Medium,
        High,
        Urgent
    }

    public class HousekeepingTask
    {
        [Key]
        public int TaskId { get; set; }

        [Required]
        public int RoomId { get; set; }

        [Required]
        [StringLength(200)]
        public string TaskDescription { get; set; }

        [Required]
        public TaskStatus Status { get; set; } = TaskStatus.Pending;

        [Required]
        public TaskPriority Priority { get; set; } = TaskPriority.Medium;

        public int? AssignedToAdminId { get; set; }

        public DateTime ScheduledDateTime { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int CreatedByAdminId { get; set; }

        // Navigation properties
        [ForeignKey("RoomId")]
        public virtual Room Room { get; set; }

        [ForeignKey("AssignedToAdminId")]
        public virtual Admin AssignedTo { get; set; }

        [ForeignKey("CreatedByAdminId")]
        public virtual Admin CreatedBy { get; set; }
    }
}
