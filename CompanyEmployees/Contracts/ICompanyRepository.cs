using System.Collections.Generic;
using CompanyEmployees.Models;

namespace Contracts
{
    public interface ICompanyRepository
    {
		IEnumerable<Company> GetAllCompanies(bool trackChanges);
    }
}