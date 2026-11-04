using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookIt.Web.Data;
using BookIt.Web.Models;
using BookIt.Web.Models.Entities;

namespace BookIt.Web.Controllers;

// Owner page: edit the weekly opening hours in one form.
// Only logged-in users with the Owner role may use ANY action in this controller.
// Not logged in → redirected to the login page. Logged in but not an owner → "Access denied".
[Authorize(Roles = Roles.Owner)]
public class WorkingHoursController : Controller
{
    private readonly AppDbContext _db;

    public WorkingHoursController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentBusinessId => SeedData.DemoBusinessId;

    // GET /WorkingHours
    public async Task<IActionResult> Index()
    {
        var rows = await _db.WorkingHours
            .Where(w => w.BusinessId == CurrentBusinessId)
            .ToListAsync();

        // Convert database rows (0–2 per day) into exactly 7 form rows.
        var model = new WeekHoursViewModel();
        foreach (var day in Enum.GetValues<DayOfWeek>())   // Sunday ... Saturday
        {
            var periods = rows.Where(r => r.DayOfWeek == day).OrderBy(r => r.OpenTime).ToList();
            var row = new DayHoursRow { DayOfWeek = day, IsOpen = periods.Count > 0 };

            if (periods.Count >= 1)
            {
                row.OpenTime = periods.First().OpenTime;
                row.CloseTime = periods.Last().CloseTime;
            }
            if (periods.Count >= 2)
            {
                // Two periods: the gap between them is the break.
                row.BreakStart = periods[0].CloseTime;
                row.BreakEnd = periods[1].OpenTime;
            }
            model.Days.Add(row);
        }

        return View(model);
    }

    // POST /WorkingHours
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Index(WeekHoursViewModel model)
    {
        // The form must contain each of the 7 days exactly once.
        if (model.Days.Count != 7 || model.Days.Select(d => d.DayOfWeek).Distinct().Count() != 7)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        // Simplest correct approach: replace the whole week.
        // RemoveRange + AddRange + one SaveChangesAsync = one database transaction,
        // so the week is never left half-saved if something fails.
        var existing = await _db.WorkingHours
            .Where(w => w.BusinessId == CurrentBusinessId)
            .ToListAsync();
        _db.WorkingHours.RemoveRange(existing);

        foreach (var day in model.Days.Where(d => d.IsOpen))
        {
            if (day.HasBreak)
            {
                _db.WorkingHours.Add(NewPeriod(day.DayOfWeek, day.OpenTime, day.BreakStart!.Value));
                _db.WorkingHours.Add(NewPeriod(day.DayOfWeek, day.BreakEnd!.Value, day.CloseTime));
            }
            else
            {
                _db.WorkingHours.Add(NewPeriod(day.DayOfWeek, day.OpenTime, day.CloseTime));
            }
        }

        await _db.SaveChangesAsync();

        TempData["Success"] = "Working hours saved.";
        return RedirectToAction(nameof(Index));
    }

    private WorkingHours NewPeriod(DayOfWeek day, TimeOnly open, TimeOnly close) => new()
    {
        BusinessId = CurrentBusinessId,
        DayOfWeek = day,
        OpenTime = open,
        CloseTime = close
    };
}
