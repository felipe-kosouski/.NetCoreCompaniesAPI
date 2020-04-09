using System.Linq;
using System.Collections.Generic;
using CompanyEmployees.Data.Context;
using CompanyEmployees.Models;
using Contracts;
using System;

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

		public IEnumerable<Company> GetAllCompanies(bool trackChanges)
		{
			return FindAll(trackChanges).OrderBy(x => x.Name).ToList();
		}

		public IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges)
		{
			return FindByCondition(x => ids.Contains(x.Id), trackChanges).ToList();
		}

		public Company GetCompany(Guid companyId, bool trackChanges)
		{
			return FindByCondition(c => c.Id.Equals(companyId), trackChanges).SingleOrDefault();
		}
	}
}