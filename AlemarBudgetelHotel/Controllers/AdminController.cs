using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemarBudgetelHotel.Data;
using AlemarBudgetelHotel.Models;
using AlemarBudgetelHotel.Helpers;
using System;
using System.Linq;

namespace AlemarBudgetelHotel.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================
        // DASHBOARD
        // ==========================================
        public async System.Threading.Tasks.Task<IActionResult> Index()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            // Calculate dashboard statistics
            ViewBag.TodayReservations = await _context.Reservations
                .Where(r => r.CreatedAt.Date == DateTime.Today)
                .CountAsync();

            ViewBag.TodayRevenue = await _context.Payments
                .Where(p => p.PaidAt.HasValue && p.PaidAt.Value.Date == DateTime.Today)
                .SumAsync(p => (decimal?)p.Amount) ?? 0;

            var totalRooms = await _context.Rooms.CountAsync();
            var occupiedRooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Occupied)
                .CountAsync();
            ViewBag.OccupancyRate = totalRooms > 0 ? (int)((occupiedRooms * 100.0) / totalRooms) : 0;

            ViewBag.TotalCustomers = await _context.Customers.CountAsync();

            return View();
        }

        // ==========================================
        // LOGIN GET
        // ==========================================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // ==========================================
        // LOGIN POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Login(string username, string password)
        {
            var admin = await _context.Admins
                .Where(a => a.Username == username && a.IsActive == true)
                .FirstOrDefaultAsync();

            if (admin != null && AdminPasswordHasher.VerifyPassword(password, admin.PasswordHash))
            {
                HttpContext.Session.SetInt32("AdminId", admin.AdminId);
                HttpContext.Session.SetString("AdminName", admin.FullName);
                HttpContext.Session.SetString("AdminRole", admin.Role);

                return RedirectToAction("Index");
            }

            ViewBag.Error = "Invalid credentials";
            return View();
        }

        // ==========================================
        // REGISTER GET
        // ==========================================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // ==========================================
        // REGISTER POST
        // ==========================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> Register(Admin admin, string password, string confirmPassword)
        {
            try
            {
                if (password != confirmPassword)
                {
                    ViewBag.Error = "Passwords do not match!";
                    return View(admin);
                }

                if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                {
                    ViewBag.Error = "Password must be at least 6 characters long!";
                    return View(admin);
                }

                var usernameExists = await _context.Admins
                    .AnyAsync(a => a.Username == admin.Username);

                if (usernameExists)
                {
                    ViewBag.Error = "Username already exists!";
                    return View(admin);
                }

                var emailExists = await _context.Admins
                    .AnyAsync(a => a.Email == admin.Email);

                if (emailExists)
                {
                    ViewBag.Error = "Email already exists!";
                    return View(admin);
                }

                admin.PasswordHash = AdminPasswordHasher.HashPassword(password);
                admin.IsActive = true;
                admin.CreatedAt = DateTime.Now;

                _context.Admins.Add(admin);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Account created successfully! You can now login.";

                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Registration failed. Please try again. Error: " + ex.Message;
                return View(admin);
            }
        }

        // ==========================================
        // LOGOUT
        // ==========================================
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // ==========================================
        // RESERVATIONS
        // ==========================================
        public async System.Threading.Tasks.Task<IActionResult> Reservations()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var reservations = await _context.Reservations
                .Include(r => r.Customer)
                .Include(r => r.Room)
                .Include(r => r.Payment)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reservations);
        }

        // ==========================================
        // ROOMS
        // ==========================================
        public async System.Threading.Tasks.Task<IActionResult> Rooms()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var rooms = await _context.Rooms.ToListAsync();

            return View(rooms);
        }

        // ==========================================
        // SECURITY
        // ==========================================
        public async System.Threading.Tasks.Task<IActionResult> Security()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var logs = await _context.AuditLogs
                .OrderByDescending(l => l.CreatedAt)
                .Take(100)
                .ToListAsync();

            return View(logs);
        }

        // ==========================================
        // HOUSEKEEPING
        // ==========================================
        public async System.Threading.Tasks.Task<IActionResult> Housekeeping()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var tasks = await _context.HousekeepingTasks
                .Include(t => t.Room)
                .Include(t => t.AssignedTo)
                .Include(t => t.CreatedBy)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            return View(tasks);
        }

        // ==========================================
        // CUSTOMERS
        // ==========================================
        public async System.Threading.Tasks.Task<IActionResult> Customers()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            var customers = await _context.Customers
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();

            return View(customers);
        }

        // ==========================================
        // REPORTS
        // ==========================================
        public IActionResult Reports()
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }
    }
}