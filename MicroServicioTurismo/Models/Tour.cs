// Models/Tour.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.Models
{
    public class Tour
    {
        [Key]
        public int TourId { get; set; }

        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int Capacity { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Reservation>? Reservations { get; set; }
    }
}