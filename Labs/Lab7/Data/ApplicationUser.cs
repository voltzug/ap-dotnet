using Microsoft.AspNetCore.Identity;

namespace Lab7.Data;

public class ApplicationUser : IdentityUser
{
    // Nullable to avoid migration and null-reference issues for existing users
    public long? CustomerId { get; set; }
}