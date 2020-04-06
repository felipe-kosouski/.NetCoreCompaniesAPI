using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
//using CompanyEmployees.Models;

namespace CompanyEmployees.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CompaniesController : ControllerBase
	{
		public CompaniesController()
		{
		}

		// GET api/companies
		[HttpGet("")]
		public ActionResult<IEnumerable<string>> Getstrings()
		{
			return new List<string> { };
		}

		// GET api/companies/5
		[HttpGet("{id}")]
		public ActionResult<string> GetstringById(int id)
		{
			return null;
		}

		// POST api/companies
		[HttpPost("")]
		public void Poststring(string value)
		{
		}

		// PUT api/companies/5
		[HttpPut("{id}")]
		public void Putstring(int id, string value)
		{
		}

		// DELETE api/companies/5
		[HttpDelete("{id}")]
		public void DeletestringById(int id)
		{
		}
	}
}