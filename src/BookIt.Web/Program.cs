using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BookIt.Web.Data;
using BookIt.Web.Models.Entities;

var builder = WebApplication.CreateBuilder(args);

// ---------- 1. Register services ----------

builder.Services.AddControllersWithViews();

// The login/register pages from Identity's built-in UI are Razor Pages, so we enable those too.
builder.Services.AddRazorPages();

// Register AppDbContext with dependency injection.
// The connection string is read from configuration: appsettings.json, then user-secrets (in Development),
// then environment variables — later sources override earlier ones.
var connectionString = builder.Configuration.GetConnectionString("BookIt")
    ?? throw new InvalidOperationException("Connection string 'BookIt' not found. Set it with 'dotnet user-secrets'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ASP.NET Core Identity: user accounts, password hashing, login cookies, and the
// ready-made Register / Login / Logout / Manage-account pages (under /Identity/Account/...).
builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        // We don't send emails yet, so don't require clicking a confirmation link.
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        // Lock an account for 5 minutes after 5 wrong passwords (slows down password guessing).
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    })
    .AddRoles<IdentityRole>()                       // enables roles like "Owner"
    .AddEntityFrameworkStores<AppDbContext>();      // users and roles are stored via our DbContext

var app = builder.Build();

// ---------- 2. Seed the Owner role and demo owner account ----------

using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    await IdentitySeeder.SeedAsync(scope.ServiceProvider, app.Configuration, logger);
}

// ---------- 3. Request pipeline (order matters!) ----------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();   // WHO are you?  (reads the login cookie → sets User)
app.UseAuthorization();    // ARE YOU ALLOWED? (checks [Authorize] against User)

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages();       // the Identity pages

app.Run();
