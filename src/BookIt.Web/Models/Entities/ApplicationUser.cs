using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace BookIt.Web.Models.Entities;

// A user account. IdentityUser already has Id, Email, UserName, PasswordHash, PhoneNumber, etc.
// We extend it with our own class now — even with one extra field — because switching the
// user type later (after users exist) is painful.
public class ApplicationUser : IdentityUser
{
    [MaxLength(100)]
    public string? FullName { get; set; }
}
