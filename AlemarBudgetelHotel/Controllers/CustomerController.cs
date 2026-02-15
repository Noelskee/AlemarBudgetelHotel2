using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemarBudgetelHotel.Data;
using AlemarBudgetelHotel.Models;
using AlemarBudgetelHotel.Helpers;
using System.Linq;

namespace AlemarBudgetelHotel.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // DASHBOARD
        // ==========================================
        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");
            return View();
        }

        // ==========================================
        // LOGIN
        // ==========================================
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var customer = await _context.Customers
                .FirstOrDefaultAsync(c => c.Username == username && c.IsActive);

            if (customer != null && CustomerPasswordHasher.VerifyPassword(password, customer.PasswordHash))
            {
                HttpContext.Session.SetInt32("CustomerId", customer.CustomerId);
                HttpContext.Session.SetString("CustomerName", customer.FullName);
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // ==========================================
        // REGISTER
        // ==========================================
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Customer customer, string password, string confirmPassword)
        {
            try
            {
                if (password != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match!";
                    return View(customer);
                }

                if (password.Length < 6)
                {
                    ViewBag.Error = "Password must be at least 6 characters long!";
                    return View(customer);
                }

                // Check if username already exists
                var existingUsername = await _context.Customers
                    .AnyAsync(c => c.Username == customer.Username);
                if (existingUsername)
                {
                    ViewBag.Error = "Username already exists!";
                    return View(customer);
                }

                // Check if email already exists
                var existingEmail = await _context.Customers
                    .AnyAsync(c => c.Email == customer.Email);
                if (existingEmail)
                {
                    ViewBag.Error = "Email already exists!";
                    return View(customer);
                }

                customer.PasswordHash = CustomerPasswordHasher.HashPassword(password);
                customer.IsActive = true;
                customer.CreatedAt = DateTime.Now;

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Account created successfully! You can now login.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Registration failed. Please try again.";
                return View(customer);
            }
        }

        // ==========================================
        // ROOMS - Browse available rooms
        // ==========================================
        public async Task<IActionResult> Rooms()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            var rooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Available)
                .ToListAsync();
            return View(rooms);
        }

        // ==========================================
        // BOOK ROOM - Show booking form with automatic check-in
        // ==========================================
        public async Task<IActionResult> BookRoom(int roomId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || room.Status != RoomStatus.Available)
            {
                TempData["Error"] = "Room is not available for booking.";
                return RedirectToAction("Rooms");
            }

            return View(room);
        }

        // ==========================================
        // CREATE RESERVATION - Uses automatic current time for check-in
        // Customer only selects duration, guests fixed at 1
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            int roomId,
            string duration,
            int numberOfGuests,
            string specialRequests)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            try
            {
                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null || room.Status != RoomStatus.Available)
                {
                    TempData["Error"] = "Room is not available for booking.";
                    return RedirectToAction("Rooms");
                }

                // Parse duration and calculate amount
                DurationOption durationOption;
                decimal totalAmount;
                int hours;

                switch (duration)
                {
                    case "0": // 3 Hours
                        durationOption = DurationOption.ThreeHours;
                        totalAmount = room.Price3Hours;
                        hours = 3;
                        break;
                    case "1": // 12 Hours
                        durationOption = DurationOption.TwelveHours;
                        totalAmount = room.Price12Hours;
                        hours = 12;
                        break;
                    case "2": // 24 Hours
                        durationOption = DurationOption.TwentyFourHours;
                        totalAmount = room.Price24Hours;
                        hours = 24;
                        break;
                    default:
                        TempData["Error"] = "Invalid duration selected.";
                        return RedirectToAction("BookRoom", new { roomId });
                }

                // AUTOMATIC: Use current time for check-in
                DateTime checkInDateTime = DateTime.Now;
                DateTime checkOutDateTime = checkInDateTime.AddHours(hours);

                // Create reservation with Pending status (waiting for payment)
                var reservation = new Reservation
                {
                    CustomerId = customerId.Value,
                    RoomId = roomId,
                    CheckInDateTime = checkInDateTime,
                    CheckOutDateTime = checkOutDateTime,
                    Duration = durationOption,
                    NumberOfGuests = numberOfGuests, // Fixed at 1
                    SpecialRequests = specialRequests,
                    Status = ReservationStatus.Pending, // Pending until payment confirmed
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.Now
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                // Redirect to Payment page
                return RedirectToAction("Payment", new { reservationId = reservation.ReservationId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Reservation failed. Please try again.";
                return RedirectToAction("BookRoom", new { roomId });
            }
        }

        // ==========================================
        // PAYMENT - Show GCash payment page with QR code
        // ==========================================
        public async Task<IActionResult> Payment(int reservationId)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId && r.CustomerId == customerId);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("Rooms");
            }

            return View(reservation);
        }

        // ==========================================
        // CONFIRM PAYMENT - Complete booking with GCash reference
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> ConfirmPayment(
            int reservationId,
            string gcashReferenceNumber,
            string gcashPhoneNumber)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Room)
                    .FirstOrDefaultAsync(r => r.ReservationId == reservationId && r.CustomerId == customerId);

                if (reservation == null)
                {
                    TempData["Error"] = "Reservation not found.";
                    return RedirectToAction("Rooms");
                }

                // Validate GCash reference number (13 digits)
                if (string.IsNullOrWhiteSpace(gcashReferenceNumber) ||
                    gcashReferenceNumber.Length != 13 ||
                    !gcashReferenceNumber.All(char.IsDigit))
                {
                    TempData["Error"] = "Invalid GCash reference number. Must be exactly 13 digits.";
                    return RedirectToAction("Payment", new { reservationId });
                }

                // Validate GCash phone number (11 digits starting with 09)
                if (string.IsNullOrWhiteSpace(gcashPhoneNumber) ||
                    gcashPhoneNumber.Length != 11 ||
                    !gcashPhoneNumber.StartsWith("09"))
                {
                    TempData["Error"] = "Invalid GCash phone number. Must be 11 digits starting with 09.";
                    return RedirectToAction("Payment", new { reservationId });
                }

                // Update reservation status to Confirmed
                reservation.Status = ReservationStatus.Confirmed;

                // Update room status to Occupied
                reservation.Room.Status = RoomStatus.Occupied;

                // Create payment record with GCash details
                var payment = new Payment
                {
                    ReservationId = reservation.ReservationId,
                    Amount = reservation.TotalAmount,
                    PaymentMethod = "GCash",
                    GCashReferenceNumber = gcashReferenceNumber,
                    Status = PaymentStatus.Completed,
                    TransactionReference = $"GCASH-{gcashReferenceNumber}",
                    CreatedAt = DateTime.Now,
                    PaidAt = DateTime.Now
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                TempData["Success"] = $"✅ Booking Confirmed! Reservation #{reservation.ReservationId} | GCash Ref: {gcashReferenceNumber}";
                return RedirectToAction("MyReservations");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Payment confirmation failed. Please try again.";
                return RedirectToAction("Payment", new { reservationId });
            }
        }

        // ==========================================
        // MY RESERVATIONS - View all customer reservations
        // ==========================================
        public async Task<IActionResult> MyReservations()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Payment)
                .Where(r => r.CustomerId == customerId.Value)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reservations);
        }

        // ==========================================
        // PAYMENT HISTORY - View all customer payments
        // ==========================================
        public async Task<IActionResult> PaymentHistory()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");

            var payments = await _context.Payments
                .Include(p => p.Reservation)
                    .ThenInclude(r => r.Room)
                .Where(p => p.Reservation.CustomerId == customerId.Value)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();

            return View(payments);
        }

        // ==========================================
        // LOGOUT
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}