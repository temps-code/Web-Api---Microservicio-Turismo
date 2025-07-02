using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Data;
using MicroServicioTurismo.DTOs;
using MicroServicioTurismo.Models;

namespace MicroServicioTurismo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToursController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ToursController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/tours
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tours = await _context.Tours
                                          .Where(t => t.IsActive)
                                          .ToListAsync();
                return Ok(tours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tours.", detail = ex.Message });
            }
        }

        // GET: api/tours/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var activeTours = await _context.Tours
                                                .Where(t => t.IsActive)
                                                .ToListAsync();
                return Ok(activeTours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving active tours.", detail = ex.Message });
            }
        }

        // GET: api/tours/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactive()
        {
            try
            {
                var inactiveTours = await _context.Tours
                                                  .Where(t => !t.IsActive)
                                                  .ToListAsync();
                return Ok(inactiveTours);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving inactive tours.", detail = ex.Message });
            }
        }

        // GET: api/tours/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(id);
                if (tour == null || !tour.IsActive)
                    return NotFound(new { message = "Tour not found or inactive." });

                return Ok(tour);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving tour.", detail = ex.Message });
            }
        }

        // POST: api/tours
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] TourCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tour = new Tour
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Capacity = dto.Capacity,
                    Price = dto.Price,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Tours.Add(tour);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = tour.TourId }, new { message = "Tour created.", tourId = tour.TourId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating tour.", detail = ex.Message });
            }
        }

        // PUT: api/tours/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TourUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var tour = await _context.Tours.FindAsync(id);
                if (tour == null || !tour.IsActive)
                    return NotFound(new { message = "Tour not found or inactive." });

                if (!string.IsNullOrWhiteSpace(dto.Title))
                    tour.Title = dto.Title;
                if (dto.Description != null)
                    tour.Description = dto.Description;
                if (dto.StartDate.HasValue)
                    tour.StartDate = dto.StartDate.Value;
                if (dto.EndDate.HasValue)
                    tour.EndDate = dto.EndDate.Value;
                if (dto.Capacity.HasValue)
                    tour.Capacity = dto.Capacity.Value;
                if (dto.Price.HasValue)
                    tour.Price = dto.Price.Value;

                tour.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tour updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating tour.", detail = ex.Message });
            }
        }

        // DELETE: api/tours/{id} (logical)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogical(int id)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(id);
                if (tour == null)
                    return NotFound(new { message = "Tour not found." });

                if (!tour.IsActive)
                    return BadRequest(new { message = "Tour is already inactive." });

                tour.IsActive = false;
                tour.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tour logically deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting tour.", detail = ex.Message });
            }
        }

        // PATCH: api/tours/{id}/reactivate
        [HttpPatch("{id:int}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(id);
                if (tour == null)
                    return NotFound(new { message = "Tour not found." });

                if (tour.IsActive)
                    return BadRequest(new { message = "Tour is already active." });

                tour.IsActive = true;
                tour.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tour reactivated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reactivating tour.", detail = ex.Message });
            }
        }

        // DELETE: api/tours/{id}/physical
        [HttpDelete("{id:int}/physical")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            try
            {
                var tour = await _context.Tours.FindAsync(id);
                if (tour == null)
                    return NotFound(new { message = "Tour not found." });

                _context.Tours.Remove(tour);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Tour permanently deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error physically deleting tour.", detail = ex.Message });
            }
        }
    }
}
