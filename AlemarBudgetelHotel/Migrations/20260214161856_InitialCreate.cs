using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AlemarBudgetelHotel.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Admins", x => x.AdminId);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    LogId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LogType = table.Column<int>(type: "int", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: true),
                    Username = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserAgent = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.LogId);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    CustomerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.CustomerId);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    RoomId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Price3Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price12Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Price24Hours = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Floor = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.RoomId);
                });

            migrationBuilder.CreateTable(
                name: "HousekeepingTasks",
                columns: table => new
                {
                    TaskId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    TaskDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    AssignedToAdminId = table.Column<int>(type: "int", nullable: true),
                    ScheduledDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedByAdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HousekeepingTasks", x => x.TaskId);
                    table.ForeignKey(
                        name: "FK_HousekeepingTasks_Admins_AssignedToAdminId",
                        column: x => x.AssignedToAdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HousekeepingTasks_Admins_CreatedByAdminId",
                        column: x => x.CreatedByAdminId,
                        principalTable: "Admins",
                        principalColumn: "AdminId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_HousekeepingTasks_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    RoomId = table.Column<int>(type: "int", nullable: false),
                    CheckInDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CheckOutDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    NumberOfGuests = table.Column<int>(type: "int", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    SpecialRequests = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedInByAdminId = table.Column<int>(type: "int", nullable: true),
                    ActualCheckInTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CheckedOutByAdminId = table.Column<int>(type: "int", nullable: true),
                    ActualCheckOutTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.ReservationId);
                    table.ForeignKey(
                        name: "FK_Reservations_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Reservations_Rooms_RoomId",
                        column: x => x.RoomId,
                        principalTable: "Rooms",
                        principalColumn: "RoomId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    PaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    TransactionReference = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GCashReferenceNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    GCashPhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PaymentProof = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    PaidAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.PaymentId);
                    table.ForeignKey(
                        name: "FK_Payments_Reservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "Reservations",
                        principalColumn: "ReservationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Rooms",
                columns: new[] { "RoomId", "Capacity", "CreatedAt", "Description", "Floor", "ImageUrl", "Price12Hours", "Price24Hours", "Price3Hours", "RoomNumber", "Status", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2362), "Cozy single room", 1, "/images/rooms/single.jpg", 800m, 1200m, 300m, "101", 0, 0, null },
                    { 2, 1, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2372), "Cozy single room", 1, "/images/rooms/single.jpg", 800m, 1200m, 300m, "102", 0, 0, null },
                    { 3, 1, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2383), "Cozy single room", 1, "/images/rooms/single.jpg", 800m, 1200m, 300m, "103", 0, 0, null },
                    { 4, 2, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2393), "Comfortable double room", 2, "/images/rooms/double.jpg", 1200m, 1800m, 500m, "201", 0, 1, null },
                    { 5, 2, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2403), "Comfortable double room", 2, "/images/rooms/double.jpg", 1200m, 1800m, 500m, "202", 0, 1, null },
                    { 6, 2, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2411), "Comfortable double room", 2, "/images/rooms/double.jpg", 1200m, 1800m, 500m, "203", 0, 1, null },
                    { 7, 2, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2420), "Standard room with modern amenities", 3, "/images/rooms/standard.jpg", 1800m, 2500m, 700m, "301", 0, 2, null },
                    { 8, 2, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2428), "Standard room with modern amenities", 3, "/images/rooms/standard.jpg", 1800m, 2500m, 700m, "302", 0, 2, null },
                    { 9, 2, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2436), "Standard room with modern amenities", 3, "/images/rooms/standard.jpg", 1800m, 2500m, 700m, "303", 0, 2, null },
                    { 10, 3, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2443), "Deluxe room with premium facilities", 4, "/images/rooms/deluxe.jpg", 2500m, 3500m, 1000m, "401", 0, 3, null },
                    { 11, 3, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2451), "Deluxe room with premium facilities", 4, "/images/rooms/deluxe.jpg", 2500m, 3500m, 1000m, "402", 0, 3, null },
                    { 12, 3, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2459), "Deluxe room with premium facilities", 4, "/images/rooms/deluxe.jpg", 2500m, 3500m, 1000m, "403", 0, 3, null },
                    { 13, 4, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2467), "Super deluxe room with luxury amenities", 5, "/images/rooms/super-deluxe.jpg", 3500m, 5000m, 1500m, "501", 0, 4, null },
                    { 14, 4, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2476), "Super deluxe room with luxury amenities", 5, "/images/rooms/super-deluxe.jpg", 3500m, 5000m, 1500m, "502", 0, 4, null },
                    { 15, 4, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2484), "Super deluxe room with luxury amenities", 5, "/images/rooms/super-deluxe.jpg", 3500m, 5000m, 1500m, "503", 0, 4, null },
                    { 16, 5, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2492), "Premium super duper suite", 6, "/images/rooms/super-duper.jpg", 5000m, 7000m, 2000m, "601", 0, 5, null },
                    { 17, 5, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2500), "Premium super duper suite", 6, "/images/rooms/super-duper.jpg", 5000m, 7000m, 2000m, "602", 0, 5, null },
                    { 18, 5, new DateTime(2026, 2, 15, 0, 18, 53, 782, DateTimeKind.Local).AddTicks(2507), "Premium super duper suite", 6, "/images/rooms/super-duper.jpg", 5000m, 7000m, 2000m, "603", 0, 5, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Admins_Email",
                table: "Admins",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Admins_Username",
                table: "Admins",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Email",
                table: "Customers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_Username",
                table: "Customers",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HousekeepingTasks_AssignedToAdminId",
                table: "HousekeepingTasks",
                column: "AssignedToAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_HousekeepingTasks_CreatedByAdminId",
                table: "HousekeepingTasks",
                column: "CreatedByAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_HousekeepingTasks_RoomId",
                table: "HousekeepingTasks",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_ReservationId",
                table: "Payments",
                column: "ReservationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_CustomerId",
                table: "Reservations",
                column: "CustomerId");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_RoomId",
                table: "Reservations",
                column: "RoomId");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_RoomNumber",
                table: "Rooms",
                column: "RoomNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "HousekeepingTasks");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Customers");

            migrationBuilder.DropTable(
                name: "Rooms");
        }
    }
}
