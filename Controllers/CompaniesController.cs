using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.Dtos;
using Contracts;
using Microsoft.AspNetCore.Mvc;
//using CompanyEmployees.Models;

namespace CompanyEmployees.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CompaniesController : ControllerBase
	{
		private readonly IRepositoryManager _repository;
		private readonly ILoggerManager _logger;
		private readonly IMapper _mapper;

		public CompaniesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
		{
			_repository = repository;
			_logger = logger;
			_mapper = mapper;
		}

		// GET api/companies
		[HttpGet]
		public ActionResult GetCompanies()
		{
			var companies = _repository.Company.GetAllCompanies(trackChanges: false);
			var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
			return Ok(companiesDto);
		}

		// GET api/companies/5
		[HttpGet("{id}")]
		public ActionResult GetCompany(Guid id)
		{
			var company = _repository.Company.GetCompany(id, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {id} does not exist in the database");
				return NotFound();
			}
			else
			{
				var companyDto = _mapper.Map<CompanyDto>(company);
				return Ok(companyDto);
			}
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