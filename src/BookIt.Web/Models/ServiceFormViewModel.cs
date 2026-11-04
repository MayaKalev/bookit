using System.ComponentModel.DataAnnotations;

namespace BookIt.Web.Models;

// What the Create/Edit form sends. Deliberately does NOT include BusinessId or IsActive:
// the server decides those, so a user can't change them by editing the HTML ("overposting").
public class ServiceFormViewModel
{
    // 0 when creating, the service's Id when editing.
    public int Id { get; set; }

    [Required(ErrorMessage = "Please enter a name.")]
    [StringLength(100)]
    public string Name { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    // [Display] sets the <label> text.
    [Display(Name = "Duration (minutes)")]
    [Range(5, 480, ErrorMessage = "Duration must be between 5 and 480 minutes.")]
    public int DurationMinutes { get; set; } = 30;

    [Range(0, 100000, ErrorMessage = "Price must be between 0 and 100,000.")]
    [DataType(DataType.Currency)]
    public decimal Price { get; set; }
}
