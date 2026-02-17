using Microsoft.EntityFrameworkCore;
using AlemarBudgetelHotel.Helpers;
using AlemarBudgetelHotel.Models;

namespace AlemarBudgetelHotel.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Admin> Admins { get; set; }
        public DbSet<Staff> Staffs { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<HousekeepingTask> HousekeepingTasks { get; set; }
        public DbSet<AuditLog> AuditLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Admin entity
            modelBuilder.Entity<Admin>(entity =>
            {
                entity.HasKey(e => e.AdminId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Staff entity
            modelBuilder.Entity<Staff>(entity =>
            {
                entity.HasKey(e => e.StaffId);
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.Username).IsUnique();
            });

            // Configure Customer entity (no login - just info)
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.HasKey(e => e.CustomerId);
            });

            // Configure Room entity
            modelBuilder.Entity<Room>(entity =>
            {
                entity.HasKey(e => e.RoomId);
                entity.HasIndex(e => e.RoomNumber).IsUnique();
                entity.Property(e => e.Price3Hours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Price12Hours).HasColumnType("decimal(18,2)");
                entity.Property(e => e.Price24Hours).HasColumnType("decimal(18,2)");
            });

            // Configure Reservation entity
            modelBuilder.Entity<Reservation>(entity =>
            {
                entity.HasKey(e => e.ReservationId);
                entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Customer)
                    .WithMany(c => c.Reservations)
                    .HasForeignKey(e => e.CustomerId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.Reservations)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure Payment entity
            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId);
                entity.Property(e => e.Amount).HasColumnType("decimal(18,2)");

                entity.HasOne(e => e.Reservation)
                    .WithOne(r => r.Payment)
                    .HasForeignKey<Payment>(e => e.ReservationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Configure HousekeepingTask entity
            modelBuilder.Entity<HousekeepingTask>(entity =>
            {
                entity.HasKey(e => e.TaskId);

                entity.HasOne(e => e.Room)
                    .WithMany(r => r.HousekeepingTasks)
                    .HasForeignKey(e => e.RoomId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.AssignedTo)
                    .WithMany()
                    .HasForeignKey(e => e.AssignedToAdminId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.CreatedBy)
                    .WithMany()
                    .HasForeignKey(e => e.CreatedByAdminId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    AdminId = 1,
                    FullName = "System Administrator",
                    Email = "admin@alemarbudgetel.com",
                    Username = "admin",
                    PasswordHash = AdminPasswordHasher.HashPassword("admin123"),
                    PhoneNumber = "09123456789",
                    Role = "SuperAdmin",
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );

            modelBuilder.Entity<Staff>().HasData(
                new Staff
                {
                    StaffId = 1,
                    FullName = "Front Desk Staff",
                    Email = "staff@alemarbudgetel.com",
                    Username = "staff",
                    PasswordHash = StaffPasswordHasher.HashPassword("staff123"),
                    PhoneNumber = "09123456780",
                    Role = StaffRole.Receptionist,
                    IsActive = true,
                    CreatedAt = new DateTime(2024, 1, 1)
                }
            );

            var rooms = new[]
            {
                new Room { RoomId = 1, RoomNumber = "101", Type = RoomType.Single, Price3Hours = 300M, Price12Hours = 800M, Price24Hours = 1200M, Status = RoomStatus.Available, Description = "Cozy single room", Capacity = 1, Floor = 1, ImageUrl = "/images/rooms/single.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 2, RoomNumber = "102", Type = RoomType.Single, Price3Hours = 300M, Price12Hours = 800M, Price24Hours = 1200M, Status = RoomStatus.Available, Description = "Cozy single room", Capacity = 1, Floor = 1, ImageUrl = "/images/rooms/single.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 3, RoomNumber = "103", Type = RoomType.Single, Price3Hours = 300M, Price12Hours = 800M, Price24Hours = 1200M, Status = RoomStatus.Available, Description = "Cozy single room", Capacity = 1, Floor = 1, ImageUrl = "/images/rooms/single.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 4, RoomNumber = "201", Type = RoomType.Double, Price3Hours = 500M, Price12Hours = 1200M, Price24Hours = 1800M, Status = RoomStatus.Available, Description = "Comfortable double room", Capacity = 2, Floor = 2, ImageUrl = "/images/rooms/double.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 5, RoomNumber = "202", Type = RoomType.Double, Price3Hours = 500M, Price12Hours = 1200M, Price24Hours = 1800M, Status = RoomStatus.Available, Description = "Comfortable double room", Capacity = 2, Floor = 2, ImageUrl = "/images/rooms/double.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 6, RoomNumber = "203", Type = RoomType.Double, Price3Hours = 500M, Price12Hours = 1200M, Price24Hours = 1800M, Status = RoomStatus.Available, Description = "Comfortable double room", Capacity = 2, Floor = 2, ImageUrl = "/images/rooms/double.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 7, RoomNumber = "301", Type = RoomType.Standard, Price3Hours = 700M, Price12Hours = 1800M, Price24Hours = 2500M, Status = RoomStatus.Available, Description = "Standard room with modern amenities", Capacity = 2, Floor = 3, ImageUrl = "/images/rooms/standard.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 8, RoomNumber = "302", Type = RoomType.Standard, Price3Hours = 700M, Price12Hours = 1800M, Price24Hours = 2500M, Status = RoomStatus.Available, Description = "Standard room with modern amenities", Capacity = 2, Floor = 3, ImageUrl = "/images/rooms/standard.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 9, RoomNumber = "303", Type = RoomType.Standard, Price3Hours = 700M, Price12Hours = 1800M, Price24Hours = 2500M, Status = RoomStatus.Available, Description = "Standard room with modern amenities", Capacity = 2, Floor = 3, ImageUrl = "/images/rooms/standard.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 10, RoomNumber = "401", Type = RoomType.Deluxe, Price3Hours = 1000M, Price12Hours = 2500M, Price24Hours = 3500M, Status = RoomStatus.Available, Description = "Deluxe room with premium facilities", Capacity = 3, Floor = 4, ImageUrl = "/images/rooms/deluxe.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 11, RoomNumber = "402", Type = RoomType.Deluxe, Price3Hours = 1000M, Price12Hours = 2500M, Price24Hours = 3500M, Status = RoomStatus.Available, Description = "Deluxe room with premium facilities", Capacity = 3, Floor = 4, ImageUrl = "/images/rooms/deluxe.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 12, RoomNumber = "403", Type = RoomType.Deluxe, Price3Hours = 1000M, Price12Hours = 2500M, Price24Hours = 3500M, Status = RoomStatus.Available, Description = "Deluxe room with premium facilities", Capacity = 3, Floor = 4, ImageUrl = "/images/rooms/deluxe.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 13, RoomNumber = "501", Type = RoomType.SuperDeluxe, Price3Hours = 1500M, Price12Hours = 3500M, Price24Hours = 5000M, Status = RoomStatus.Available, Description = "Super deluxe room with luxury amenities", Capacity = 4, Floor = 5, ImageUrl = "/images/rooms/super-deluxe.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 14, RoomNumber = "502", Type = RoomType.SuperDeluxe, Price3Hours = 1500M, Price12Hours = 3500M, Price24Hours = 5000M, Status = RoomStatus.Available, Description = "Super deluxe room with luxury amenities", Capacity = 4, Floor = 5, ImageUrl = "/images/rooms/super-deluxe.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 15, RoomNumber = "503", Type = RoomType.SuperDeluxe, Price3Hours = 1500M, Price12Hours = 3500M, Price24Hours = 5000M, Status = RoomStatus.Available, Description = "Super deluxe room with luxury amenities", Capacity = 4, Floor = 5, ImageUrl = "/images/rooms/super-deluxe.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 16, RoomNumber = "601", Type = RoomType.SuperDuper, Price3Hours = 2000M, Price12Hours = 5000M, Price24Hours = 7000M, Status = RoomStatus.Available, Description = "Premium super duper suite", Capacity = 5, Floor = 6, ImageUrl = "/images/rooms/super-duper.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 17, RoomNumber = "602", Type = RoomType.SuperDuper, Price3Hours = 2000M, Price12Hours = 5000M, Price24Hours = 7000M, Status = RoomStatus.Available, Description = "Premium super duper suite", Capacity = 5, Floor = 6, ImageUrl = "/images/rooms/super-duper.jpg", CreatedAt = new DateTime(2024, 1, 1) },
                new Room { RoomId = 18, RoomNumber = "603", Type = RoomType.SuperDuper, Price3Hours = 2000M, Price12Hours = 5000M, Price24Hours = 7000M, Status = RoomStatus.Available, Description = "Premium super duper suite", Capacity = 5, Floor = 6, ImageUrl = "/images/rooms/super-duper.jpg", CreatedAt = new DateTime(2024, 1, 1) }
            };

            modelBuilder.Entity<Room>().HasData(rooms);
        }
    }
}
