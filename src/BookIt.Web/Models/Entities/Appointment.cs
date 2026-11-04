using System.ComponentModel.DataAnnotations;

namespace BookIt.Web.Models.Entities;

public enum AppointmentStatus
{
    Booked = 0,
    Cancelled = 1,
    Completed = 2
}

// A booking: one customer, one service, at one time, with one business.
public class Appointment
{
    public int Id { get; set; }

    // BusinessId is also reachable through Service, but storing it directly makes
    // "all appointments of business X" queries (and the double-booking check) simple and fast.
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    public int ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    // The customer's user Id. ASP.NET Identity (step 1.8) uses string Ids,
    // so we store a string now and connect it to the user table later.
    [Required, MaxLength(450)]
    public string CustomerId { get; set; } = "";

    // Stored in UTC so times are unambiguous (see step 1.14).
    // EndUtc = StartUtc + service duration, saved so overlap checks are one simple query.
    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    public AppointmentStatus Status { get; set; } = AppointmentStatus.Booked;

    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
