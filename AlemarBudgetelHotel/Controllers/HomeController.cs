using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemarBudgetelHotel.Data;
using AlemarBudgetelHotel.Models;
using AlemarBudgetelHotel.Helpers;

namespace AlemarBudgetelHotel.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
                return RedirectToAction("Login");
            return View();
        }

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

        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(Customer customer, string password)
        {
            customer.PasswordHash = CustomerPasswordHasher.HashPassword(password);
            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction("Login");
        }

        public async Task<IActionResult> Rooms()
        {
            var rooms = await _context.Rooms.Where(r => r.Status == RoomStatus.Available).ToListAsync();
            return View(rooms);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
