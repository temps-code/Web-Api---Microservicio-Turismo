using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Data;
using MicroServicioTurismo.DTOs;
using MicroServicioTurismo.Models;

namespace MicroServicioTurismo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var reservations = await _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Tour)
                    .Where(r => r.IsActive)
                    .ToListAsync();
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving reservations.", detail = ex.Message });
            }
        }

        // GET: api/reservations/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var reservation = await _context.Reservations
                    .Include(r => r.User)
                    .Include(r => r.Tour)
                    .FirstOrDefaultAsync(r => r.ReservationId == id && r.IsActive);

                if (reservation == null)
                    return NotFound(new { message = "Reservation not found." });

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving reservation.", detail = ex.Message });
            }
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validate user
                var user = await _context.Users.FindAsync(dto.UserId);
                if (user == null || !user.IsActive)
                    return BadRequest(new { message = "Invalid UserId." });

                // Validate tour
                var tour = await _context.Tours.FindAsync(dto.TourId);
                if (tour == null || !tour.IsActive)
                    return BadRequest(new { message = "Invalid TourId." });

                // Validate capacity
                var totalBooked = await _context.Reservations
                    .Where(r => r.TourId == dto.TourId && r.IsActive)
                    .SumAsync(r => r.SeatsBooked);

                if (totalBooked + dto.SeatsBooked > tour.Capacity)
                    return BadRequest(new { message = "Not enough available seats for this tour." });

                var reservation = new Reservation
                {
                    UserId = dto.UserId,
                    TourId = dto.TourId,
                    ReservedDate = dto.ReservedDate,
                    SeatsBooked = dto.SeatsBooked,
                    Status = "Pending",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = reservation.ReservationId }, new { message = "Reservation created.", reservationId = reservation.ReservationId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating reservation.", detail = ex.Message });
            }
        }

        // PUT: api/reservations/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null || !reservation.IsActive)
                    return NotFound(new { message = "Reservation not found." });

                // Validate capacity if seats updated
                if (dto.SeatsBooked.HasValue)
                {
                    var tour = await _context.Tours.FindAsync(reservation.TourId);
                    var otherBooked = await _context.Reservations
                        .Where(r => r.TourId == reservation.TourId && r.IsActive && r.ReservationId != id)
                        .SumAsync(r => r.SeatsBooked);

                    if (otherBooked + dto.SeatsBooked.Value > tour!.Capacity)
                        return BadRequest(new { message = "Not enough available seats for this tour." });

                    reservation.SeatsBooked = dto.SeatsBooked.Value;
                }

                if (dto.ReservedDate.HasValue)
                    reservation.ReservedDate = dto.ReservedDate.Value;
                if (!string.IsNullOrWhiteSpace(dto.Status))
                    reservation.Status = dto.Status!;

                reservation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating reservation.", detail = ex.Message });
            }
        }

        // DELETE: api/reservations/{id} (logical)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogical(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null || !reservation.IsActive)
                    return NotFound(new { message = "Reservation not found." });

                reservation.IsActive = false;
                reservation.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation logically deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting reservation.", detail = ex.Message });
            }
        }

        // DELETE: api/reservations/{id}/physical
        [HttpDelete("{id:int}/physical")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            try
            {
                var reservation = await _context.Reservations.FindAsync(id);
                if (reservation == null)
                    return NotFound(new { message = "Reservation not found." });

                _context.Reservations.Remove(reservation);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation permanently deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error physically deleting reservation.", detail = ex.Message });
            }
        }
    }
}
