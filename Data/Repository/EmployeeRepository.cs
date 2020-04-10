using System.Linq;
using System;
using System.Collections.Generic;
using CompanyEmployees.Data.Context;
using CompanyEmployees.Models;
using Contracts;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Data.Repository
{
	public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
	{
		public EmployeeRepository(RepositoryContext context) : base(context)
		{
		}

		public void CreateEmployeeForCompany(Guid companyId, Employee employee)
		{
			employee.CompanyId = companyId;
			Create(employee);
		}

		public void DeleteEmployee(Employee employee)
		{
			Delete(employee);
		}

		public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
		{
			return await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges).SingleOrDefaultAsync();
		}

		public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges)
		{
			return await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges).OrderBy(e => e.Name).ToListAsync();
		}
	}
}