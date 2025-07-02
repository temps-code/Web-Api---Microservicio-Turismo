using System;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.DTOs
{
    public class ReservationCreateDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int TourId { get; set; }

        [Required]
        public DateTime ReservedDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "SeatsBooked must be at least 1.")]
        public int SeatsBooked { get; set; }
    }

    public class ReservationUpdateDto
    {
        public DateTime? ReservedDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SeatsBooked must be at least 1.")]
        public int? SeatsBooked { get; set; }

        public string? Status { get; set; }
    }
}