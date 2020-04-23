using System.Linq;
using System.Collections.Generic;
using CompanyEmployees.Data.Context;
using CompanyEmployees.Models;
using Contracts;
using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CompanyEmployees.Data.Repository
{
	public class CompanyRepository : RepositoryBase<Company>, ICompanyRepository
	{
		public CompanyRepository(RepositoryContext context) : base(context)
		{
		}

		public void CreateCompany(Company company)
		{
			Create(company);
		}

		public void DeleteCompany(Company company)
		{
			Delete(company);
		}

		public async Task<IEnumerable<Company>> GetAllCompaniesAsync(bool trackChanges)
		{
			return await FindAll(trackChanges).OrderBy(x => x.Name).ToListAsync();
		}

		public async Task<IEnumerable<Company>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges)
		{
			return await FindByCondition(x => ids.Contains(x.Id), trackChanges).ToListAsync();
		}

		public async Task<Company> GetCompanyAsync(Guid companyId, bool trackChanges)
		{
			return await FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefaultAsync();
		}
	}
}