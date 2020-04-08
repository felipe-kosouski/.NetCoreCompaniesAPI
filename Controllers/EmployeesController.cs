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
	[Route("api/companies/{companyId}/[controller]")]
	[ApiController]
	public class EmployeesController : ControllerBase
	{
		private readonly IRepositoryManager _repository;
		private readonly ILoggerManager _logger;
		private readonly IMapper _mapper;
		public EmployeesController(IRepositoryManager repository, ILoggerManager logger, IMapper mapper)
		{
			_repository = repository;
			_logger = logger;
			_mapper = mapper;
		}

		// GET api/employees
		[HttpGet]
		public ActionResult GetEmployeesForCompany(Guid companyId)
		{
			var company = _repository.Company.GetCompany(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges: false);
			var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
			return Ok(employeesDto);
		}

		// GET api/employees/5
		[HttpGet("{id}")]
		public ActionResult GetEmployeeForCompany(Guid companyId, Guid id)
		{
			var company = _repository.Company.GetCompany(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeFromDb = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
			if (employeeFromDb == null)
			{
				_logger.LogInfo($"Employee with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employee =_mapper.Map<EmployeeDto>(employeeFromDb);
			return Ok(employee);
		}

		// POST api/employees
		[HttpPost("")]
		public void Poststring(string value)
		{
		}

		// PUT api/employees/5
		[HttpPut("{id}")]
		public void Putstring(int id, string value)
		{
		}

		// DELETE api/employees/5
		[HttpDelete("{id}")]
		public void DeletestringById(int id)
		{
		}
	}
}