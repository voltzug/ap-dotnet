using Microsoft.EntityFrameworkCore;
using Lab5.Models;
using System.Linq;

namespace Lab5.Data
{
    public class MoviesDbContext : DbContext
    {
        public DbSet<Movie> Movies { get; set; }

        public MoviesDbContext(DbContextOptions<MoviesDbContext> options)
            : base(options)
        {
            try
            {
                // Ensure database and Movies table exist
                Database.EnsureCreated();
                if (Database.CanConnect() && this.Database.ExecuteSqlRaw("SELECT 1 FROM sqlite_master WHERE type='table' AND name='Movies';") == 1)
                {
                    DbState.IsDbUp = true;
                }
            }
            catch
            {
                DbState.IsDbUp = false;
            }
        }
    }
}