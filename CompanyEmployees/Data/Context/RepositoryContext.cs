using CompanyEmployees.Data.Configuration;
using CompanyEmployees.Models;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Data.Context
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions options) : base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			builder.ApplyConfiguration(new CompanyConfiguration());
			builder.ApplyConfiguration(new EmployeeConfiguration());
		}

		public DbSet<Company> Companies { get; set; }
		public DbSet<Employee> Employees { get; set; }
    }
}