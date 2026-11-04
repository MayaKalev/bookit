using System.ComponentModel.DataAnnotations;

namespace BookIt.Web.Models.Entities;

// Something a business offers, e.g. "Haircut — 30 min — 80₪".
public class Service
{
    public int Id { get; set; }

    // Foreign key: which business this service belongs to.
    // EF Core links "BusinessId" + "Business" by naming convention.
    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;   // "null!" = EF Core fills it in when loaded

    [Required, MaxLength(100)]
    public string Name { get; set; } = "";

    [MaxLength(500)]
    public string? Description { get; set; }

    // Length of one appointment for this service. Slots are calculated from this.
    [Range(5, 480)]
    public int DurationMinutes { get; set; }

    // decimal (not double) for money: it stores exact values like 79.90.
    [Range(0, 100000)]
    public decimal Price { get; set; }

    // Instead of deleting a service that has past appointments, the owner hides it.
    public bool IsActive { get; set; } = true;
}
