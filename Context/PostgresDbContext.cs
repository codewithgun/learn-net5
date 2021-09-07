using Catalog.Entities.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Context
{
    public class PostgresDbContext : DbContext
    {
        public PostgresDbContext(DbContextOptions<PostgresDbContext> options) : base(options)
        { }

        public DbSet<Item> Items { get; set; }
    }
}