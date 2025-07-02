using System;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.DTOs
{
    public class ReservationDetailCreateDto
    {
        [Required]
        public int ReservationId { get; set; }

        [Required]
        public int TourId { get; set; }

        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal Price { get; set; }
    }

    public class ReservationDetailUpdateDto
    {
        public int? TourId { get; set; }
        public string? Description { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be positive.")]
        public decimal? Price { get; set; }
    }
}