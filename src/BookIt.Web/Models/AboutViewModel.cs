namespace BookIt.Web.Models;

// A ViewModel holds exactly the data one page needs to display.
// It is not saved to the database — it's just a "data package" from controller to view.
public class AboutViewModel
{
    public string AppName { get; set; } = "";
    public string Tagline { get; set; } = "";
    public List<string> Features { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
}
