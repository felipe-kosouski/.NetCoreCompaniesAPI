using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.Dtos;
using CompanyEmployees.Models;
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
		[HttpGet("{id}", Name = "GetEmployeeForCompany")]
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

			var employee = _mapper.Map<EmployeeDto>(employeeFromDb);
			return Ok(employee);
		}

		// POST api/employees
		[HttpPost]
		public IActionResult CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employee)
		{
			if (employee == null)
			{
				_logger.LogError("EmployeeForCreationDto object sent from client is null.");
				return BadRequest("EmployeeForCreationDto object is null");
			}

			var company = _repository.Company.GetCompany(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeEntity = _mapper.Map<Employee>(employee);

			_repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
			_repository.Save();

			var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

			return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
		}

		// PUT api/employees/5
		[HttpPut("{id}")]
		public IActionResult UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employee)
		{
			if (employee == null)
			{
				_logger.LogError("EmployeeForUpdateDto object sent from client is null.");
				return BadRequest("EmployeeForUpdateDto object is null");
			}

			var company = _repository.Company.GetCompany(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeEntity = _repository.Employee.GetEmployee(companyId, id, trackChanges: true);
			if (employeeEntity == null)
			{
				_logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			_mapper.Map(employee, employeeEntity);
			_repository.Save();

			return NoContent();
		}

		// DELETE api/employees/5
		[HttpDelete("{id}")]
		public IActionResult DeleteEmployeeForCompany(Guid companyId, Guid id)
		{
			var company = _repository.Company.GetCompany(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeForCompany = _repository.Employee.GetEmployee(companyId, id, trackChanges: false);
			if (employeeForCompany == null)
			{
				_logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			_repository.Employee.DeleteEmployee(employeeForCompany);
			_repository.Save();
			return NoContent();
		}
	}
}