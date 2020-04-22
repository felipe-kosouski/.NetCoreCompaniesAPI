using CompanyEmployees.Data.Configuration;
using CompanyEmployees.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Data.Context
{
    public class RepositoryContext : IdentityDbContext<User>
    {
        public RepositoryContext(DbContextOptions options) : base(options)
		{
			
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.ApplyConfiguration(new CompanyConfiguration());
			builder.ApplyConfiguration(new EmployeeConfiguration());
			builder.ApplyConfiguration(new RoleConfiguration());
		}

		public DbSet<Company> Companies { get; set; }
		public DbSet<Employee> Employees { get; set; }
    }
}