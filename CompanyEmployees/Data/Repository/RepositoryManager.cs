using System.Threading.Tasks;
using CompanyEmployees.Data.Context;
using Contracts;

namespace CompanyEmployees.Data.Repository
{
	public class RepositoryManager : IRepositoryManager
	{
		private RepositoryContext _context;
		private ICompanyRepository _companyRepository;
		private IEmployeeRepository _employeeRepository;

		public RepositoryManager(RepositoryContext context)
		{
			_context = context;
		}

		public ICompanyRepository Company
		{
			get
			{
				if (_companyRepository == null)
				{
					_companyRepository = new CompanyRepository(_context);
				}
				return _companyRepository;
			}
		}

		public IEmployeeRepository Employee
		{
			get
			{
				if (_employeeRepository == null)
				{
					_employeeRepository = new EmployeeRepository(_context);
				}
				return _employeeRepository;
			}
		}

		public Task SaveAsync()
		{
			return _context.SaveChangesAsync();
		}
	}
}