using System;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.DTOs
{
    public class TourCreateDto
    {
        [Required]
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
        public int Capacity { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal Price { get; set; }
    }

    public class TourUpdateDto
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Capacity must be greater than zero.")]
        public int? Capacity { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
        public decimal? Price { get; set; }
    }
}