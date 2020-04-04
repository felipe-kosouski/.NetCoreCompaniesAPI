using CompanyEmployees.Data.Context;
using CompanyEmployees.Models;
using Contracts;

namespace CompanyEmployees.Data.Repository
{
	public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
	{
		public EmployeeRepository(RepositoryContext context) : base(context)
		{
		}
	}
}