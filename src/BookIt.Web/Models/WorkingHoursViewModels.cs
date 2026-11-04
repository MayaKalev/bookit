using System.ComponentModel.DataAnnotations;

namespace BookIt.Web.Models;

// The whole week's form: 7 rows, Sunday (0) to Saturday (6).
public class WeekHoursViewModel
{
    public List<DayHoursRow> Days { get; set; } = new();
}

// One day in the form. The database stores 0, 1 or 2 WorkingHours rows per day;
// this class is the friendlier shape the user edits.
//
// IValidatableObject lets a class check rules that involve several fields at once
// (attributes like [Range] can only look at one field).
public class DayHoursRow : IValidatableObject
{
    public DayOfWeek DayOfWeek { get; set; }

    [Display(Name = "Open")]
    public bool IsOpen { get; set; }

    [Display(Name = "From")]
    public TimeOnly OpenTime { get; set; } = new(9, 0);

    [Display(Name = "To")]
    public TimeOnly CloseTime { get; set; } = new(17, 0);

    // Optional break. Both empty = no break.
    [Display(Name = "Break from")]
    public TimeOnly? BreakStart { get; set; }

    [Display(Name = "Break to")]
    public TimeOnly? BreakEnd { get; set; }

    public bool HasBreak => BreakStart.HasValue && BreakEnd.HasValue;

    // ASP.NET calls this automatically during model validation.
    // Each "yield return" adds one error; the second argument says which field it belongs to.
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (!IsOpen)
        {
            yield break;   // closed day: the times don't matter
        }

        if (CloseTime <= OpenTime)
        {
            yield return new ValidationResult("Closing time must be after opening time.", new[] { nameof(CloseTime) });
            yield break;
        }

        if (BreakStart.HasValue != BreakEnd.HasValue)
        {
            yield return new ValidationResult("Fill in both break times, or leave both empty.", new[] { nameof(BreakEnd) });
            yield break;
        }

        if (HasBreak)
        {
            if (BreakEnd <= BreakStart)
            {
                yield return new ValidationResult("Break must end after it starts.", new[] { nameof(BreakEnd) });
            }
            else if (BreakStart <= OpenTime || BreakEnd >= CloseTime)
            {
                yield return new ValidationResult("Break must be inside opening hours.", new[] { nameof(BreakStart) });
            }
        }
    }
}
