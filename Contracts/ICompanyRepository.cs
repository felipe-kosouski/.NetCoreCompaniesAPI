using System;
using System.Collections.Generic;
using CompanyEmployees.Models;

namespace Contracts
{
    public interface ICompanyRepository
    {
		IEnumerable<Company> GetAllCompanies(bool trackChanges);
		Company GetCompany(Guid companyId, bool trackChanges);
		IEnumerable<Company> GetByIds(IEnumerable<Guid> ids, bool trackChanges);
		void CreateCompany(Company company);
    }
}