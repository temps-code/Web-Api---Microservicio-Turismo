using System;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.DTOs
{
    public class PaymentCreateDto
    {
        [Required]
        public int ReservationId { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string Method { get; set; } = null!; // e.g. CreditCard, Cash, Transfer

        [Required]
        public string Status { get; set; } = null!; // e.g. Pending, Completed, Refunded
    }

    public class PaymentUpdateDto
    {
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be positive.")]
        public decimal? Amount { get; set; }

        public DateTime? PaymentDate { get; set; }
        public string? Method { get; set; }
        public string? Status { get; set; }
    }
}