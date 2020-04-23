using System.Threading.Tasks;
using CompanyEmployees.Dtos;

namespace CompanyEmployees.Contracts
{
	public interface IAuthenticationManager
	{
		Task<bool> ValidateUser(UserForAuthenticationDto userForAuth);
		Task<string> CreateToken();
	}
}