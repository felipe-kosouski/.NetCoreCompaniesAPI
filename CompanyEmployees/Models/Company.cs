using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CompanyEmployees.Models
{
    public class Company
    {
		[Column("CompanyId")]
        public Guid Id { get; set; }

		[Required(ErrorMessage = "Company Name is a Required Field")]
		[MaxLength(60, ErrorMessage = "Maximum length for the {0} field is {1} characters")]
		public string Name { get; set; }

		[Required(ErrorMessage = "Company Address is a Required Field")]
		[MaxLength(60, ErrorMessage = "Maximum length for the {0} field is {1} characters")]
		public string Address { get; set; }
		public string Country { get; set; }
		public ICollection<Employee> Employees { get; set; }
    }
}