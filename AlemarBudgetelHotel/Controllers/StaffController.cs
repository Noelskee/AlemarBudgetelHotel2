using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemarBudgetelHotel.Data;
using AlemarBudgetelHotel.Models;
using AlemarBudgetelHotel.Helpers;
using System.Linq;

namespace AlemarBudgetelHotel.Controllers
{
    public class StaffController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StaffController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // DASHBOARD
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            // Dashboard statistics
            ViewBag.TodayReservations = await _context.Reservations
                .Where(r => r.CreatedAt.Date == DateTime.Today)
                .CountAsync();

            ViewBag.PendingReservations = await _context.Reservations
                .Where(r => r.Status == ReservationStatus.Pending)
                .CountAsync();

            ViewBag.AvailableRooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Available)
                .CountAsync();

            ViewBag.OccupiedRooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Occupied)
                .CountAsync();

            // Recent reservations
            var recentReservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Include(r => r.Payment)
                .OrderByDescending(r => r.CreatedAt)
                .Take(10)
                .ToListAsync();

            return View(recentReservations);
        }

        // ==========================================
        // LOGIN
        // ==========================================
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var staff = await _context.Staffs
                .FirstOrDefaultAsync(s => s.Username == username && s.IsActive);

            if (staff != null && StaffPasswordHasher.VerifyPassword(password, staff.PasswordHash))
            {
                HttpContext.Session.SetInt32("StaffId", staff.StaffId);
                HttpContext.Session.SetString("StaffName", staff.FullName);
                HttpContext.Session.SetString("StaffRole", staff.Role.ToString());
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid username or password.";
            return View();
        }

        // ==========================================
        // RESERVATIONS - View all reservations
        // ==========================================
        public async Task<IActionResult> Reservations()
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var reservations = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Include(r => r.Payment)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reservations);
        }

        // ==========================================
        // RESERVATION DETAILS
        // ==========================================
        public async Task<IActionResult> ReservationDetails(int reservationId)
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
            {
                TempData["Error"] = "Reservation not found.";
                return RedirectToAction("Reservations");
            }

            return View(reservation);
        }

        // ==========================================
        // ROOMS - View all rooms and status
        // ==========================================
        public async Task<IActionResult> Rooms()
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var rooms = await _context.Rooms.ToListAsync();
            return View(rooms);
        }

        // ==========================================
        // CUSTOMERS - View all customers
        // ==========================================
        public async Task<IActionResult> Customers()
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var customers = await _context.Customers
                .Include(c => c.Reservations)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(customers);
        }

        // ==========================================
        // CHECK IN - Mark reservation as checked in
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CheckIn(int reservationId)
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation != null)
            {
                reservation.Status = ReservationStatus.CheckedIn;
                reservation.ActualCheckInTime = DateTime.Now;
                reservation.Room.Status = RoomStatus.Occupied;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Reservation #{reservationId} checked in successfully.";
            }

            return RedirectToAction("Reservations");
        }

        // ==========================================
        // CHECK OUT - Mark reservation as checked out
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CheckOut(int reservationId)
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation != null)
            {
                reservation.Status = ReservationStatus.CheckedOut;
                reservation.ActualCheckOutTime = DateTime.Now;
                reservation.Room.Status = RoomStatus.Available;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Reservation #{reservationId} checked out successfully.";
            }

            return RedirectToAction("Reservations");
        }

        // ==========================================
        // CONFIRM PAYMENT - Staff verifies GCash payment
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> VerifyPayment(int reservationId)
        {
            var staffId = HttpContext.Session.GetInt32("StaffId");
            if (staffId == null)
                return RedirectToAction("Login");

            var reservation = await _context.Reservations
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation?.Payment != null)
            {
                reservation.Payment.Status = PaymentStatus.Completed;
                reservation.Status = ReservationStatus.Confirmed;
                await _context.SaveChangesAsync();
                TempData["Success"] = $"Payment for Reservation #{reservationId} verified.";
            }

            return RedirectToAction("Reservations");
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
