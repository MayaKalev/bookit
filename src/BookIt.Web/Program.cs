using Microsoft.EntityFrameworkCore;
using BookIt.Web.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Register AppDbContext with dependency injection.
// The connection string is read from configuration: appsettings.json, then user-secrets (in Development),
// then environment variables — later sources override earlier ones.
var connectionString = builder.Configuration.GetConnectionString("BookIt")
    ?? throw new InvalidOperationException("Connection string 'BookIt' not found. Set it with 'dotnet user-secrets'.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
