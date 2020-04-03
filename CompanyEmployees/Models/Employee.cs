using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyEmployees.Models
{
	public class Employee
	{
		[Column("EmployeeId")]
		public Guid Id { get; set; }

		[Required(ErrorMessage = "Employee name is a required field.")]
		[MaxLength(30, ErrorMessage = "Maximum length for the {0} is {1} characters.")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Age is a required field.")]
		public int Age { get; set; }

		[Required(ErrorMessage = "Position is a required field.")]
		[MaxLength(20, ErrorMessage = "Maximum length for the {0} is {1} characters.")]
		public string Position { get; set; }

		public Guid CompanyId { get; set; }
		public Company Company { get; set; }
	}
}