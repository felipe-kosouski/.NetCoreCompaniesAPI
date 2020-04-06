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
			try
			{
				var companies = _repository.Company.GetAllCompanies(trackChanges: false);
				var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
				return Ok(companiesDto);
			}
			catch (System.Exception ex)
			{
				_logger.LogError($"Something went wrong in the {nameof(GetCompanies)} action {ex}");
				return StatusCode(500, "Internal Server Error");
			}
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