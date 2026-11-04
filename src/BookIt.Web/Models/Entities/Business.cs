using System.ComponentModel.DataAnnotations;

namespace BookIt.Web.Models.Entities;

// A business that customers can book with (e.g. a hair salon).
// In Stage 1 there is only one; in Stage 2 many businesses sign up.
public class Business
{
    // "Id" is recognised by EF Core as the primary key automatically.
    public int Id { get; set; }

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    // URL-friendly name, e.g. "maya-hair-studio" → /b/maya-hair-studio (used in Stage 2).
    [Required, MaxLength(100)]
    public string Slug { get; set; } = "";

    [MaxLength(1000)]
    public string? Description { get; set; }   // "?" = optional (may be null)

    // IANA time zone, e.g. "Asia/Jerusalem". Working hours are in this zone's local time.
    [Required, MaxLength(64)]
    public string TimeZoneId { get; set; } = "UTC";

    // Navigation properties: "one business has many ..."
    // EF Core uses these to understand relationships and to load related data.
    public List<Service> Services { get; set; } = new();
    public List<WorkingHours> WorkingHours { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}
