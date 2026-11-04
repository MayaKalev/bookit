using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookIt.Web.Data;
using BookIt.Web.Models;
using BookIt.Web.Models.Entities;

namespace BookIt.Web.Controllers;

// Owner page: manage the business's services.
// Stage 1 has a single business. In step 1.8 we lock this to logged-in owners,
// and in Stage 2 the business comes from the logged-in owner instead of a constant.
public class ServicesController : Controller
{
    private readonly AppDbContext _db;

    // Dependency injection: ASP.NET creates an AppDbContext for this request and passes it in.
    public ServicesController(AppDbContext db)
    {
        _db = db;
    }

    private int CurrentBusinessId => SeedData.DemoBusinessId;

    // GET /Services
    public async Task<IActionResult> Index()
    {
        // "async/await": while SQL Server works, the thread is free to serve other requests.
        var services = await _db.Services
            .Where(s => s.BusinessId == CurrentBusinessId)
            .OrderByDescending(s => s.IsActive)
            .ThenBy(s => s.Name)
            .ToListAsync();

        return View(services);
    }

    // GET /Services/Create  → show an empty form
    public IActionResult Create()
    {
        return View(new ServiceFormViewModel());
    }

    // POST /Services/Create  → the form was submitted
    [HttpPost]
    [ValidateAntiForgeryToken]   // rejects forms that didn't come from our own page (CSRF protection)
    public async Task<IActionResult> Create(ServiceFormViewModel form)
    {
        // Model binding already filled "form" from the posted fields and ran the [Required]/[Range] checks.
        if (!ModelState.IsValid)
        {
            // Show the same form again, with the user's input and the error messages.
            return View(form);
        }

        var service = new Service
        {
            BusinessId = CurrentBusinessId,   // decided by the server, not the form
            Name = form.Name.Trim(),
            Description = form.Description?.Trim(),
            DurationMinutes = form.DurationMinutes,
            Price = form.Price,
            IsActive = true
        };

        _db.Services.Add(service);
        await _db.SaveChangesAsync();   // ← the INSERT happens here

        // TempData survives exactly one redirect — perfect for "Saved!" messages.
        TempData["Success"] = $"Service \"{service.Name}\" was added.";

        // Post-Redirect-Get: redirect after a successful POST so refreshing the page
        // doesn't submit the form a second time.
        return RedirectToAction(nameof(Index));
    }

    // GET /Services/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var service = await FindServiceAsync(id);
        if (service == null) return NotFound();

        var form = new ServiceFormViewModel
        {
            Id = service.Id,
            Name = service.Name,
            Description = service.Description,
            DurationMinutes = service.DurationMinutes,
            Price = service.Price
        };
        return View(form);
    }

    // POST /Services/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ServiceFormViewModel form)
    {
        if (id != form.Id) return BadRequest();

        var service = await FindServiceAsync(id);
        if (service == null) return NotFound();

        if (!ModelState.IsValid) return View(form);

        // EF Core tracks the loaded entity; changing properties marks it as modified.
        service.Name = form.Name.Trim();
        service.Description = form.Description?.Trim();
        service.DurationMinutes = form.DurationMinutes;
        service.Price = form.Price;

        await _db.SaveChangesAsync();   // ← UPDATE only the changed columns

        TempData["Success"] = $"Service \"{service.Name}\" was updated.";
        return RedirectToAction(nameof(Index));
    }

    // POST /Services/ToggleActive/5  → hide or show a service (we never delete: old appointments point to it)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var service = await FindServiceAsync(id);
        if (service == null) return NotFound();

        service.IsActive = !service.IsActive;
        await _db.SaveChangesAsync();

        TempData["Success"] = service.IsActive
            ? $"\"{service.Name}\" is visible to customers again."
            : $"\"{service.Name}\" is now hidden from customers.";
        return RedirectToAction(nameof(Index));
    }

    // Only find services that belong to the current business.
    // Without the BusinessId check, someone could edit another business's service by changing the URL.
    private Task<Service?> FindServiceAsync(int id) =>
        _db.Services.FirstOrDefaultAsync(s => s.Id == id && s.BusinessId == CurrentBusinessId);
}
