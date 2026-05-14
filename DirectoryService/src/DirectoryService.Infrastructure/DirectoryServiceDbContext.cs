using Microsoft.EntityFrameworkCore;
using DirectoryService.Domain;

namespace DirectoryService.Infrastructure
{
    public class DirectoryServiceDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<Department> Departments { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Position> Positions { get; set; }


        public DirectoryServiceDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
        }
    }
}
