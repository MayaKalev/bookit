using Microsoft.AspNetCore.Mvc;
using BookIt.Web.Models;

namespace BookIt.Web.Controllers;

// The class name must end in "Controller". The part before it ("About") becomes the URL segment.
public class AboutController : Controller
{
    // Handles GET /About  (and /About/Index)
    public IActionResult Index()
    {
        // 1. Build the data the page needs.
        var model = new AboutViewModel
        {
            AppName = "BookIt",
            Tagline = "Book appointments with local businesses in seconds.",
            Features = new List<string>
            {
                "Businesses manage their services and working hours",
                "Customers browse businesses and book free slots",
                "No double bookings"
            },
            GeneratedAt = DateTime.Now
        };

        // 2. Hand it to the view. MVC looks for Views/About/Index.cshtml.
        return View(model);
    }
}
