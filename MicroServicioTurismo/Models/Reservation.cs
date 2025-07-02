using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MicroServicioTurismo.Models
{
    public class Reservation
    {
        [Key]
        public int ReservationId { get; set; }

        [Required]
        public int UserId { get; set; }
        public User? User { get; set; }

        [Required]
        public int TourId { get; set; }
        public Tour? Tour { get; set; }

        [Required]
        public DateTime ReservedDate { get; set; }

        [Required]
        public int SeatsBooked { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<ReservationDetail>? ReservationDetails { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}