namespace BookIt.Web.Models.Entities;

// One open period on one weekday, e.g. Sunday 09:00–17:00.
// - A day with no rows = closed that day.
// - Two rows on the same day = a break, e.g. 09:00–13:00 and 14:00–18:00.
public class WorkingHours
{
    public int Id { get; set; }

    public int BusinessId { get; set; }
    public Business Business { get; set; } = null!;

    // Built-in .NET enum: Sunday = 0 ... Saturday = 6.
    public DayOfWeek DayOfWeek { get; set; }

    // TimeOnly = a time of day with no date (stored as SQL "time").
    // Local time in the business's TimeZoneId.
    public TimeOnly OpenTime { get; set; }
    public TimeOnly CloseTime { get; set; }
}
