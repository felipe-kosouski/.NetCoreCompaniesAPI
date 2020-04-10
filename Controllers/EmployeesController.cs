using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.Dtos;
using CompanyEmployees.Models;
using Contracts;
using Microsoft.AspNetCore.JsonPatch;
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
		public async Task<ActionResult> GetEmployeesForCompany(Guid companyId)
		{
			var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges: false);
			var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
			return Ok(employeesDto);
		}

		// GET api/employees/5
		[HttpGet("{id}", Name = "GetEmployeeForCompany")]
		public async Task<ActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
		{
			var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeFromDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
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
		public async Task<ActionResult> CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employee)
		{
			if (employee == null)
			{
				_logger.LogError("EmployeeForCreationDto object sent from client is null.");
				return BadRequest("EmployeeForCreationDto object is null");
			}

			if (!ModelState.IsValid)
			{
				_logger.LogError("Invalid model state for the EmployeeForCreationDto object");
				return UnprocessableEntity(ModelState);
			}

			var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeEntity = _mapper.Map<Employee>(employee);

			_repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
			await _repository.SaveAsync();

			var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);

			return CreatedAtRoute("GetEmployeeForCompany", new { companyId, id = employeeToReturn.Id }, employeeToReturn);
		}

		// PUT api/employees/5
		[HttpPut("{id}")]
		public async Task<ActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employee)
		{
			if (employee == null)
			{
				_logger.LogError("EmployeeForUpdateDto object sent from client is null.");
				return BadRequest("EmployeeForUpdateDto object is null");
			}

			if (!ModelState.IsValid)
			{
				_logger.LogError("Invalid model state for the EmployeeForUpdateDto object");
				return UnprocessableEntity(ModelState);
			}

			var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
			if (employeeEntity == null)
			{
				_logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			_mapper.Map(employee, employeeEntity);
			await _repository.SaveAsync();

			return NoContent();
		}

		// DELETE api/employees/5
		[HttpDelete("{id}")]
		public async Task<ActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
		{
			var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}

			var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: false);
			if (employeeForCompany == null)
			{
				_logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			_repository.Employee.DeleteEmployee(employeeForCompany);
			await _repository.SaveAsync();
			return NoContent();
		}

		[HttpPatch("{id}")]
		public async Task<ActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
		{
			if (patchDoc == null)
			{
				_logger.LogError("patchDoc object sent from client is null.");
				return BadRequest("patchDoc object is null");
			}
			var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges: false);

			if (company == null)
			{
				_logger.LogInfo($"Company with id: {companyId} doesn't exist in the database.");
				return NotFound();
			}
			var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges: true);
			if (employeeEntity == null)
			{
				_logger.LogInfo($"Employee with id: {id} doesn't exist in the database.");
				return NotFound();
			}
			var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
			patchDoc.ApplyTo(employeeToPatch, ModelState);
			TryValidateModel(employeeToPatch);
			if (!ModelState.IsValid)
			{
				_logger.LogError("Invalid model state for the patch document");
				return UnprocessableEntity(ModelState);
			}
			_mapper.Map(employeeToPatch, employeeEntity);
			await _repository.SaveAsync();
			return NoContent();
		}
	}
}