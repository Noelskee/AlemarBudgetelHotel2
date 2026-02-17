using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AlemarBudgetelHotel.Data;
using AlemarBudgetelHotel.Models;

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
        // ROOMS - Browse available rooms (no login needed)
        // ==========================================
        public async Task<IActionResult> Index()
        {
            var rooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Available)
                .ToListAsync();
            return View(rooms);
        }

        public async Task<IActionResult> Rooms()
        {
            var rooms = await _context.Rooms
                .Where(r => r.Status == RoomStatus.Available)
                .ToListAsync();
            return View(rooms);
        }

        // ==========================================
        // BOOK ROOM - Show booking form (no login needed)
        // ==========================================
        public async Task<IActionResult> BookRoom(int roomId)
        {
            var room = await _context.Rooms.FindAsync(roomId);
            if (room == null || room.Status != RoomStatus.Available)
            {
                TempData["Error"] = "Room is not available for booking.";
                return RedirectToAction("Rooms");
            }
            return View(room);
        }

        // ==========================================
        // CREATE RESERVATION - Customer fills name/phone, auto check-in
        // ==========================================
        [HttpPost]
        public async Task<IActionResult> CreateReservation(
            int roomId,
            string duration,
            string guestName,
            string guestPhone,
            string guestEmail,
            string specialRequests)
        {
            try
            {
                var room = await _context.Rooms.FindAsync(roomId);
                if (room == null || room.Status != RoomStatus.Available)
                {
                    TempData["Error"] = "Room is not available for booking.";
                    return RedirectToAction("Rooms");
                }

                // Validate guest info
                if (string.IsNullOrWhiteSpace(guestName) || string.IsNullOrWhiteSpace(guestPhone))
                {
                    ViewBag.Error = "Name and phone number are required.";
                    return View("BookRoom", room);
                }

                // Parse duration
                DurationOption durationOption;
                decimal totalAmount;
                int hours;

                switch (duration)
                {
                    case "0":
                        durationOption = DurationOption.ThreeHours;
                        totalAmount = room.Price3Hours;
                        hours = 3;
                        break;
                    case "1":
                        durationOption = DurationOption.TwelveHours;
                        totalAmount = room.Price12Hours;
                        hours = 12;
                        break;
                    case "2":
                        durationOption = DurationOption.TwentyFourHours;
                        totalAmount = room.Price24Hours;
                        hours = 24;
                        break;
                    default:
                        TempData["Error"] = "Invalid duration selected.";
                        return RedirectToAction("BookRoom", new { roomId });
                }

                // Create or find customer by phone
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.PhoneNumber == guestPhone);

                if (customer == null)
                {
                    customer = new Customer
                    {
                        FullName = guestName,
                        PhoneNumber = guestPhone,
                        Email = guestEmail,
                        CreatedAt = DateTime.Now
                    };
                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                // AUTOMATIC: Use current time for check-in
                DateTime checkInDateTime = DateTime.Now;
                DateTime checkOutDateTime = checkInDateTime.AddHours(hours);

                var reservation = new Reservation
                {
                    CustomerId = customer.CustomerId,
                    RoomId = roomId,
                    CheckInDateTime = checkInDateTime,
                    CheckOutDateTime = checkOutDateTime,
                    Duration = durationOption,
                    NumberOfGuests = 1,
                    SpecialRequests = specialRequests,
                    Status = ReservationStatus.Pending,
                    TotalAmount = totalAmount,
                    CreatedAt = DateTime.Now
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return RedirectToAction("Payment", new { reservationId = reservation.ReservationId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Reservation failed. Please try again.";
                return RedirectToAction("BookRoom", new { roomId });
            }
        }

        // ==========================================
        // PAYMENT - Show GCash payment page
        // ==========================================
        public async Task<IActionResult> Payment(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

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
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.Room)
                    .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

                if (reservation == null)
                {
                    TempData["Error"] = "Reservation not found.";
                    return RedirectToAction("Rooms");
                }

                // Validate GCash reference (13 digits)
                if (string.IsNullOrWhiteSpace(gcashReferenceNumber) ||
                    gcashReferenceNumber.Length != 13 ||
                    !gcashReferenceNumber.All(char.IsDigit))
                {
                    TempData["Error"] = "Invalid GCash reference number. Must be exactly 13 digits.";
                    return RedirectToAction("Payment", new { reservationId });
                }

                // Validate GCash phone (11 digits starting with 09)
                if (string.IsNullOrWhiteSpace(gcashPhoneNumber) ||
                    gcashPhoneNumber.Length != 11 ||
                    !gcashPhoneNumber.StartsWith("09"))
                {
                    TempData["Error"] = "Invalid GCash phone number. Must be 11 digits starting with 09.";
                    return RedirectToAction("Payment", new { reservationId });
                }

                // Update reservation to Confirmed
                reservation.Status = ReservationStatus.Confirmed;
                reservation.Room.Status = RoomStatus.Occupied;

                // Create payment record
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

                TempData["Success"] = $"Booking Confirmed! Reservation #{reservation.ReservationId} | GCash Ref: {gcashReferenceNumber}";
                return RedirectToAction("BookingSuccess", new { reservationId = reservation.ReservationId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Payment confirmation failed. Please try again.";
                return RedirectToAction("Payment", new { reservationId });
            }
        }

        // ==========================================
        // BOOKING SUCCESS - Show confirmation page
        // ==========================================
        public async Task<IActionResult> BookingSuccess(int reservationId)
        {
            var reservation = await _context.Reservations
                .Include(r => r.Room)
                .Include(r => r.Customer)
                .Include(r => r.Payment)
                .FirstOrDefaultAsync(r => r.ReservationId == reservationId);

            if (reservation == null)
                return RedirectToAction("Rooms");

            return View(reservation);
        }
    }
}
