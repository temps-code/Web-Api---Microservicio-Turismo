// DTOs/UserDtos.cs
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.DTOs
{
    public class UserCreateDto
    {
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;
    }

    public class UserUpdateDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        public string? Role { get; set; }
    }
}
