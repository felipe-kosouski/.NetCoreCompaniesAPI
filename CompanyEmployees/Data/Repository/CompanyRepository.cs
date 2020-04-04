using CompanyEmployees.Data.Context;
using CompanyEmployees.Models;
using Contracts;

namespace CompanyEmployees.Data.Repository
{
	public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
	{
		public CompanyRepository(RepositoryContext context) : base(context)
		{
		}
	}
}