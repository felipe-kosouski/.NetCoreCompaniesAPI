using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CompanyEmployees.Dtos
{
	public class CompanyForManipulationDto
	{
		[Required(ErrorMessage = "Company name is a required field.")]
		[MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Company address is a required field.")]
		[MaxLength(100, ErrorMessage = "Maximum length for the address is 100 characters.")]
		public string Address { get; set; }

		[Required(ErrorMessage = "Company Country is a required field.")]
		[MaxLength(20, ErrorMessage = "Maximum length for the country is 20 characters.")]
		public string Country { get; set; }

		public IEnumerable<EmployeeForCreationDto> Employees { get; set; }
	}
}