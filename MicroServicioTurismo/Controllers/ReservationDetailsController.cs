using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Data;
using MicroServicioTurismo.DTOs;
using MicroServicioTurismo.Models;

namespace MicroServicioTurismo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationDetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationDetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reservationdetails
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var details = await _context.ReservationDetails
                    .Include(rd => rd.Reservation)
                    .Include(rd => rd.Tour)
                    .Where(rd => rd.IsActive)
                    .ToListAsync();
                return Ok(details);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving reservation details.", detail = ex.Message });
            }
        }

        // GET: api/reservationdetails/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var detail = await _context.ReservationDetails
                    .Include(rd => rd.Reservation)
                    .Include(rd => rd.Tour)
                    .FirstOrDefaultAsync(rd => rd.ReservationDetailId == id && rd.IsActive);

                if (detail == null)
                    return NotFound(new { message = "Reservation detail not found." });

                return Ok(detail);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving reservation detail.", detail = ex.Message });
            }
        }

        // POST: api/reservationdetails
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ReservationDetailCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                // Validate reservation
                var reservation = await _context.Reservations.FindAsync(dto.ReservationId);
                if (reservation == null || !reservation.IsActive)
                    return BadRequest(new { message = "Invalid ReservationId." });

                // Validate tour
                var tour = await _context.Tours.FindAsync(dto.TourId);
                if (tour == null || !tour.IsActive)
                    return BadRequest(new { message = "Invalid TourId." });

                var detail = new ReservationDetail
                {
                    ReservationId = dto.ReservationId,
                    TourId = dto.TourId,
                    Description = dto.Description,
                    Price = dto.Price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ReservationDetails.Add(detail);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = detail.ReservationDetailId }, new { message = "Reservation detail created.", id = detail.ReservationDetailId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating reservation detail.", detail = ex.Message });
            }
        }

        // PUT: api/reservationdetails/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] ReservationDetailUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var detail = await _context.ReservationDetails.FindAsync(id);
                if (detail == null || !detail.IsActive)
                    return NotFound(new { message = "Reservation detail not found." });

                if (dto.TourId.HasValue)
                {
                    var tour = await _context.Tours.FindAsync(dto.TourId.Value);
                    if (tour == null || !tour.IsActive)
                        return BadRequest(new { message = "Invalid TourId." });
                    detail.TourId = dto.TourId.Value;
                }

                if (dto.Description != null)
                    detail.Description = dto.Description;
                if (dto.Price.HasValue)
                    detail.Price = dto.Price.Value;

                detail.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation detail updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating reservation detail.", detail = ex.Message });
            }
        }

        // DELETE: api/reservationdetails/{id} (logical)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogical(int id)
        {
            try
            {
                var detail = await _context.ReservationDetails.FindAsync(id);
                if (detail == null || !detail.IsActive)
                    return NotFound(new { message = "Reservation detail not found." });

                detail.IsActive = false;
                detail.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation detail logically deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting reservation detail.", detail = ex.Message });
            }
        }

        // DELETE: api/reservationdetails/{id}/physical
        [HttpDelete("{id:int}/physical")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            try
            {
                var detail = await _context.ReservationDetails.FindAsync(id);
                if (detail == null)
                    return NotFound(new { message = "Reservation detail not found." });

                _context.ReservationDetails.Remove(detail);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Reservation detail permanently deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error physically deleting reservation detail.", detail = ex.Message });
            }
        }
    }
}