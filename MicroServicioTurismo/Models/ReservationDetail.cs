using System;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.Models
{
    public class ReservationDetail
    {
        [Key]
        public int ReservationDetailId { get; set; }

        [Required]
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [Required]
        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}