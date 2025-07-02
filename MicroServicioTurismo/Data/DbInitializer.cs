using MicroServicioTurismo.Models;
using MicroServicioTurismo.Utils;
using Microsoft.EntityFrameworkCore;

namespace MicroServicioTurismo.Data
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            // Aplicar migraciones y crear la base de datos si no existe
            context.Database.Migrate();

            // 1. Usuarios
            if (!context.Users.Any())
            {
                var usuarios = new List<User>
                {
                    new User { Username = "juanp", FirstName = "Juan", LastName = "Pérez", Email = "juan.perez@turismo.com", Role = "Customer", IsActive = true },
                    new User { Username = "mariar", FirstName = "María", LastName = "Rodríguez", Email = "maria.rodriguez@turismo.com", Role = "Customer", IsActive = true },
                    new User { Username = "empleado1", FirstName = "Ana", LastName = "Gómez", Email = "ana.gomez@turismo.com", Role = "Employee", IsActive = true }
                };

                foreach (var u in usuarios)
                {
                    PasswordHelper.CreatePasswordHash("Prueba123", out var hash, out var salt);
                    u.PasswordHash = hash;
                    u.PasswordSalt = salt;
                    u.CreatedAt = DateTime.UtcNow;
                }

                context.Users.AddRange(usuarios);
                context.SaveChanges();
            }

            // 2. Tours
            if (!context.Tours.Any())
            {
                var tours = new List<Tour>
                {
                    new Tour { Title = "Recorrido por la Ciudad", Description = "Visita guiada por el centro histórico.", StartDate = DateTime.Today.AddDays(3), EndDate = DateTime.Today.AddDays(3), Capacity = 25, Price = 30m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Tour { Title = "Aventura en la Montaña", Description = "Senderismo en la sierra.", StartDate = DateTime.Today.AddDays(7), EndDate = DateTime.Today.AddDays(7), Capacity = 15, Price = 60m, IsActive = true, CreatedAt = DateTime.UtcNow }
                };

                context.Tours.AddRange(tours);
                context.SaveChanges();
            }

            // 3. Reservaciones
            if (!context.Reservations.Any())
            {
                var usuario = context.Users.First(u => u.Username == "juanp");
                var tourCiudad = context.Tours.First(t => t.Title.Contains("Ciudad"));
                var tourMontana = context.Tours.First(t => t.Title.Contains("Montaña"));

                var reservas = new List<Reservation>
                {
                    new Reservation { UserId = usuario.UserId, TourId = tourCiudad.TourId, ReservedDate = tourCiudad.StartDate, SeatsBooked = 2, Status = "Confirmed", IsActive = true, CreatedAt = DateTime.UtcNow },
                    new Reservation { UserId = usuario.UserId, TourId = tourMontana.TourId, ReservedDate = tourMontana.StartDate, SeatsBooked = 1, Status = "Pending", IsActive = true, CreatedAt = DateTime.UtcNow }
                };

                context.Reservations.AddRange(reservas);
                context.SaveChanges();
            }

            // 4. Detalles de Reservación
            if (!context.ReservationDetails.Any())
            {
                var reservas = context.Reservations.ToList();
                var detalles = new List<ReservationDetail>
                {
                    new ReservationDetail { ReservationId = reservas[0].ReservationId, TourId = reservas[0].TourId, Description = "Guía bilingüe incluido", Price = 5m, IsActive = true, CreatedAt = DateTime.UtcNow },
                    new ReservationDetail { ReservationId = reservas[1].ReservationId, TourId = reservas[1].TourId, Description = "Almuerzo opcional", Price = 10m, IsActive = true, CreatedAt = DateTime.UtcNow }
                };

                context.ReservationDetails.AddRange(detalles);
                context.SaveChanges();
            }

            // 5. Pagos
            if (!context.Payments.Any())
            {
                var detalles = context.ReservationDetails
                    .Include(d => d.Reservation)
                    .ToList();
                var pagos = new List<Payment>();

                foreach (var det in detalles)
                {
                    var reserva = det.Reservation;
                    if (reserva == null) continue;

                    var tour = context.Tours.Find(reserva.TourId);
                    if (tour == null) continue;

                    var montoBase = reserva.SeatsBooked * tour.Price;
                    var total = montoBase + det.Price;

                    pagos.Add(new Payment
                    {
                        ReservationId = det.ReservationId,
                        Amount = total,
                        PaymentDate = DateTime.UtcNow,
                        Method = "Tarjeta",
                        Status = "Completed",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    });
                }

                context.Payments.AddRange(pagos);
                context.SaveChanges();
            }
        }
    }
}
