using Catalog.Entities;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Context
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<User> Users { get; set; }

        // Sample of manual defining relationship
        // protected override void OnModelCreating(ModelBuilder modelBuilder)
        // {
        //     modelBuilder.Entity<Post>()
        //         .HasOne(p => p.Blog)
        //         .WithMany(b => b.Posts);
        // }
    }
}