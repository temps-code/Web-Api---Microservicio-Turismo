using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Data;
using MicroServicioTurismo.DTOs;
using MicroServicioTurismo.Models;

namespace MicroServicioTurismo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaymentsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/payments
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var payments = await _context.Payments
                    .Include(p => p.Reservation)
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return Ok(payments);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving payments.", detail = ex.Message });
            }
        }

        // GET: api/payments/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var payment = await _context.Payments
                    .Include(p => p.Reservation)
                    .FirstOrDefaultAsync(p => p.PaymentId == id && p.IsActive);

                if (payment == null)
                    return NotFound(new { message = "Payment not found or inactive." });

                return Ok(payment);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving payment.", detail = ex.Message });
            }
        }

        // POST: api/payments
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] PaymentCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var reservation = await _context.Reservations.FindAsync(dto.ReservationId);
                if (reservation == null || !reservation.IsActive)
                    return BadRequest(new { message = "Invalid ReservationId." });

                var payment = new Payment
                {
                    ReservationId = dto.ReservationId,
                    Amount = dto.Amount,
                    PaymentDate = dto.PaymentDate,
                    Method = dto.Method,
                    Status = dto.Status,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = payment.PaymentId }, new { message = "Payment created.", paymentId = payment.PaymentId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating payment.", detail = ex.Message });
            }
        }

        // PUT: api/payments/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] PaymentUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null || !payment.IsActive)
                    return NotFound(new { message = "Payment not found or inactive." });

                if (dto.Amount.HasValue)
                    payment.Amount = dto.Amount.Value;
                if (dto.PaymentDate.HasValue)
                    payment.PaymentDate = dto.PaymentDate.Value;
                if (!string.IsNullOrWhiteSpace(dto.Method))
                    payment.Method = dto.Method;
                if (!string.IsNullOrWhiteSpace(dto.Status))
                    payment.Status = dto.Status;

                payment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating payment.", detail = ex.Message });
            }
        }

        // GET: api/payments/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var active = await _context.Payments
                    .Include(p => p.Reservation)
                    .Where(p => p.IsActive)
                    .ToListAsync();
                return Ok(active);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving active payments.", detail = ex.Message });
            }
        }

        // GET: api/payments/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactive()
        {
            try
            {
                var inactive = await _context.Payments
                    .Include(p => p.Reservation)
                    .Where(p => !p.IsActive)
                    .ToListAsync();
                return Ok(inactive);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving inactive payments.", detail = ex.Message });
            }
        }

        // DELETE: api/payments/{id} (logical)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogical(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                    return NotFound(new { message = "Payment not found." });

                if (!payment.IsActive)
                    return BadRequest(new { message = "Payment is already inactive." });

                payment.IsActive = false;
                payment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment logically deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting payment.", detail = ex.Message });
            }
        }

        // PATCH: api/payments/{id}/reactivate
        [HttpPatch("{id:int}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                    return NotFound(new { message = "Payment not found." });

                if (payment.IsActive)
                    return BadRequest(new { message = "Payment is already active." });

                payment.IsActive = true;
                payment.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment reactivated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reactivating payment.", detail = ex.Message });
            }
        }

        // DELETE: api/payments/{id}/physical
        [HttpDelete("{id:int}/physical")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            try
            {
                var payment = await _context.Payments.FindAsync(id);
                if (payment == null)
                    return NotFound(new { message = "Payment not found." });

                _context.Payments.Remove(payment);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Payment permanently deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error physically deleting payment.", detail = ex.Message });
            }
        }
    }
}
