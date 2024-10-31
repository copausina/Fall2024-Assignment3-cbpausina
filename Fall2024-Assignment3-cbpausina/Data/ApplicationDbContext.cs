using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Fall2024_Assignment3_cbpausina.Models;
using Fall2024_Assignment3_cbpausina.Models.Entities;

namespace Fall2024_Assignment3_cbpausina.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Movie> Movie { get; set; } = default!;
        public DbSet<Actor> Actors { get; set; } = default!;
        public DbSet<MovieActor> MovieActor { get; set; } = default!;
    }
}
