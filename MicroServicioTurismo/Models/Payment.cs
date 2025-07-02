using System;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        [Required]
        public int ReservationId { get; set; }
        public Reservation? Reservation { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime PaymentDate { get; set; }

        [Required]
        public string Method { get; set; } = null!;

        [Required]
        public string Status { get; set; } = "Pending";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
