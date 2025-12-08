using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Lab7.Models;

namespace Lab7.Data
{
    public static class ChinookDbHelper
    {
        public static IServiceCollection AddChinookDb(this IServiceCollection services, string? connectionString = null)
        {
            var cs = string.IsNullOrWhiteSpace(connectionString) ? "Data Source=chinook.db" : connectionString;
            services.AddDbContext<ChinookDbContext>(options => options.UseSqlite(cs));
            return services;
        }

        public static async Task ImportChinookUsersAsync(this WebApplication app)
        {
            if (app == null) return;

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var chinook = services.GetService<ChinookDbContext>();
            var userManager = services.GetService<UserManager<ApplicationUser>>();

            if (chinook == null)
            {
                Console.WriteLine("ChinookDbContext service is not available. Skipping import.");
                return;
            }

            if (userManager == null)
            {
                Console.WriteLine("UserManager<ApplicationUser> service is not available. Skipping import.");
                return;
            }

            // Defensive: ensure database is accessible and Customers table exists
            try
            {
                if (!await chinook.Database.CanConnectAsync())
                {
                    Console.WriteLine("Cannot connect to Chinook database. Skipping import.");
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception while checking Chinook database connectivity: {0}", ex.Message);
                return;
            }

            // Find any customer with an email to determine if there is data to import
            var firstCustomerEmail = await chinook.Customers
                .OrderBy(c => c.CustomerId)
                .Select(c => c.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrWhiteSpace(firstCustomerEmail))
            {
                Console.WriteLine("No customers with email found in Chinook. Nothing to import.");
                return;
            }

            // If the first customer email is already present in Identity, assume import already done
            var existing = await userManager.FindByEmailAsync(firstCustomerEmail);
            if (existing != null)
            {
                Console.WriteLine("Chinook customers appear already imported (found {0}). Skipping import.", firstCustomerEmail);
                return;
            }

            // Perform import in a safe loop
            await foreach (var customer in chinook.Customers.AsAsyncEnumerable())
            {
                try
                {
                    // Defensive: skip null entries entirely
                    if (customer == null) continue;

                    var email = customer.Email;
                    if (string.IsNullOrWhiteSpace(email))
                    {
                        // Skip customers without email
                        continue;
                    }

                    // Skip existing user
                    if (await userManager.FindByEmailAsync(email) != null) continue;

                    var user = new ApplicationUser
                    {
                        UserName = email,
                        NormalizedUserName = email.ToUpperInvariant(),
                        Email = email,
                        NormalizedEmail = email.ToUpperInvariant(),
                        EmailConfirmed = true,
                        LockoutEnabled = false,
                        SecurityStamp = Guid.NewGuid().ToString(),
                        CustomerId = Convert.ToInt64(customer.CustomerId)
                    };

                    var result = await userManager.CreateAsync(user, "P@ssw0rd");
                    if (result.Succeeded)
                    {
                        Console.WriteLine("Imported customer {0} as user.", email);
                    }
                    else
                    {
                        var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                        Console.WriteLine("Failed to create user for {0}: {1}", email, errors);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception while importing customer with id {0}: {1}", customer?.CustomerId, ex.Message);
                }
            }

            Console.WriteLine("Chinook import completed.");
        }
    }
}
