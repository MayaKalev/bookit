using Microsoft.EntityFrameworkCore;
using BookIt.Web.Models.Entities;

namespace BookIt.Web.Data;

// Starting data that EF Core puts into the database through a migration.
// HasData rules:
//  - you must set the Id yourself (so EF Core can track the row in later migrations)
//  - you set foreign keys (BusinessId) directly, not navigation properties
//  - values must be fixed (no DateTime.Now), or every migration would see a "change"
public static class SeedData
{
    public const int DemoBusinessId = 1;

    public static void Apply(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Business>().HasData(new Business
        {
            Id = DemoBusinessId,
            Name = "Maya's Hair Studio",
            Slug = "mayas-hair-studio",
            Description = "A small neighbourhood hair studio. One chair, no waiting.",
            TimeZoneId = "Asia/Jerusalem"
        });

        modelBuilder.Entity<Service>().HasData(
            new Service { Id = 1, BusinessId = DemoBusinessId, Name = "Haircut", DurationMinutes = 30, Price = 80m, Description = "Wash, cut and style." },
            new Service { Id = 2, BusinessId = DemoBusinessId, Name = "Hair colouring", DurationMinutes = 90, Price = 250m, Description = "Full colour." }
        );

        // Sunday–Thursday 09:00–17:00, Friday 09:00–13:00, Saturday closed (no row).
        var hours = new List<WorkingHours>();
        var id = 1;
        foreach (var day in new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday })
        {
            hours.Add(new WorkingHours { Id = id++, BusinessId = DemoBusinessId, DayOfWeek = day, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(17, 0) });
        }
        hours.Add(new WorkingHours { Id = id, BusinessId = DemoBusinessId, DayOfWeek = DayOfWeek.Friday, OpenTime = new TimeOnly(9, 0), CloseTime = new TimeOnly(13, 0) });

        modelBuilder.Entity<WorkingHours>().HasData(hours);
    }
}
