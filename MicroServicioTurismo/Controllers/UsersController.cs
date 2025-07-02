using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MicroServicioTurismo.Data;
using MicroServicioTurismo.Models;
using MicroServicioTurismo.Utils;
using MicroServicioTurismo.DTOs;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var users = await _context.Users
                                          .Where(u => u.IsActive)
                                          .ToListAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving users.", detail = ex.Message });
            }
        }

        // GET: api/users/active
        [HttpGet("active")]
        public async Task<IActionResult> GetActive()
        {
            try
            {
                var activeUsers = await _context.Users
                                                .Where(u => u.IsActive)
                                                .ToListAsync();
                return Ok(activeUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving active users.", detail = ex.Message });
            }
        }

        // GET: api/users/inactive
        [HttpGet("inactive")]
        public async Task<IActionResult> GetInactive()
        {
            try
            {
                var inactiveUsers = await _context.Users
                                                  .Where(u => !u.IsActive)
                                                  .ToListAsync();
                return Ok(inactiveUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving inactive users.", detail = ex.Message });
            }
        }

        // GET: api/users/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null || !user.IsActive)
                    return NotFound(new { message = "User not found or inactive." });

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error retrieving user.", detail = ex.Message });
            }
        }

        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                    return Conflict(new { message = "Username already taken." });
                if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                    return Conflict(new { message = "Email already registered." });

                PasswordHelper.CreatePasswordHash(dto.Password, out var hash, out var salt);

                var user = new User
                {
                    Username = dto.Username,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Role = dto.Role,
                    PasswordHash = hash,
                    PasswordSalt = salt,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { id = user.UserId }, new { message = "User created.", userId = user.UserId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error creating user.", detail = ex.Message });
            }
        }

        // PUT: api/users/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UserUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null || !user.IsActive)
                    return NotFound(new { message = "User not found or inactive." });

                user.FirstName = dto.FirstName ?? user.FirstName;
                user.LastName = dto.LastName ?? user.LastName;
                user.Email = dto.Email ?? user.Email;
                user.Role = dto.Role ?? user.Role;
                user.UpdatedAt = DateTime.UtcNow;

                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User updated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error updating user.", detail = ex.Message });
            }
        }

        // DELETE: api/users/{id} (logical)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteLogical(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (!user.IsActive)
                    return BadRequest(new { message = "User is already inactive." });

                user.IsActive = false;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User logically deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error deleting user.", detail = ex.Message });
            }
        }

        // PATCH: api/users/{id}/reactivate
        [HttpPatch("{id:int}/reactivate")]
        public async Task<IActionResult> Reactivate(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                if (user.IsActive)
                    return BadRequest(new { message = "User is already active." });

                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                return Ok(new { message = "User reactivated." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error reactivating user.", detail = ex.Message });
            }
        }

        // DELETE: api/users/{id}/physical
        [HttpDelete("{id:int}/physical")]
        public async Task<IActionResult> DeletePhysical(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new { message = "User not found." });

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "User permanently deleted." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error physically deleting user.", detail = ex.Message });
            }
        }
    }
}
