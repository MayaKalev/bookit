using Microsoft.AspNetCore.Identity;
using BookIt.Web.Models.Entities;

namespace BookIt.Web.Data;

public static class Roles
{
    public const string Owner = "Owner";
}

// Creates the "Owner" role and the demo owner account when the app starts.
// Passwords must be hashed by Identity, so users can't be seeded with HasData in a migration —
// we use UserManager in code instead. It only creates what's missing, so running it every start is safe.
public static class IdentitySeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config, ILogger logger)
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        if (!await roleManager.RoleExistsAsync(Roles.Owner))
        {
            await roleManager.CreateAsync(new IdentityRole(Roles.Owner));
        }

        // The demo owner's login comes from configuration (user-secrets), never from source code.
        var email = config["Seed:OwnerEmail"];
        var password = config["Seed:OwnerPassword"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            logger.LogWarning("Seed:OwnerEmail / Seed:OwnerPassword not set — skipping demo owner account.");
            return;
        }

        var owner = await userManager.FindByEmailAsync(email);
        if (owner == null)
        {
            owner = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = "Demo Owner"
            };
            var result = await userManager.CreateAsync(owner, password);   // hashes the password
            if (!result.Succeeded)
            {
                logger.LogError("Could not create demo owner: {Errors}", string.Join("; ", result.Errors.Select(e => e.Description)));
                return;
            }
            logger.LogInformation("Created demo owner account {Email}", email);
        }

        if (!await userManager.IsInRoleAsync(owner, Roles.Owner))
        {
            await userManager.AddToRoleAsync(owner, Roles.Owner);
        }
    }
}
