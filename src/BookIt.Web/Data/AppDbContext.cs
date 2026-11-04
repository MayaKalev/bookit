using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BookIt.Web.Models.Entities;

namespace BookIt.Web.Data;

// The DbContext is your "session" with the database.
// You query and save entities through it; EF Core translates that into SQL.
// IdentityDbContext adds the user/role tables (AspNetUsers, AspNetRoles, ...) to our own.
public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    // The options (which database, connection string) are passed in from Program.cs.
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    // Each DbSet<T> becomes a table. Query with LINQ, e.g. _db.Services.Where(s => s.IsActive).
    public DbSet<Business> Businesses => Set<Business>();
    public DbSet<Service> Services => Set<Service>();
    public DbSet<WorkingHours> WorkingHours => Set<WorkingHours>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    // Extra rules that naming conventions and [attributes] can't express.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // base.OnModelCreating above configures the Identity tables — it must run first.

        // Two businesses can't share a URL slug.
        modelBuilder.Entity<Business>()
            .HasIndex(b => b.Slug)
            .IsUnique();

        // Money: up to 99,999,999.99 exactly (SQL type decimal(10,2)).
        modelBuilder.Entity<Service>()
            .Property(s => s.Price)
            .HasPrecision(10, 2);

        modelBuilder.Entity<Appointment>(appt =>
        {
            // Store the enum as text ("Booked") instead of a number (0) — easier to read in SQL.
            appt.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Speeds up the most common query: "this business's appointments around this time".
            appt.HasIndex(a => new { a.BusinessId, a.StartUtc });

            appt.HasIndex(a => a.CustomerId);

            // Deleting a business or service must NOT silently delete appointment history.
            // (SQL Server also refuses two cascade paths to the same table:
            //  Business → Appointment and Business → Service → Appointment.)
            appt.HasOne(a => a.Business)
                .WithMany(b => b.Appointments)
                .HasForeignKey(a => a.BusinessId)
                .OnDelete(DeleteBehavior.Restrict);

            appt.HasOne(a => a.Service)
                .WithMany()
                .HasForeignKey(a => a.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // CustomerId now really points to a user in AspNetUsers.
            appt.HasOne(a => a.Customer)
                .WithMany()
                .HasForeignKey(a => a.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Demo business, services and working hours (see SeedData.cs).
        SeedData.Apply(modelBuilder);
    }
}
