using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.Dtos;
using CompanyEmployees.ModelBinders;
using CompanyEmployees.Models;
using Contracts;
using Microsoft.AspNetCore.JsonPatch;
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
		[HttpGet("{id}", Name = "CompanyById")]
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

		[HttpGet("collection/({ids})", Name = "CompanyCollection")]
		public IActionResult GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
		{
			if (ids == null)
			{
				_logger.LogError("Parameter ids is null");
				return BadRequest("Parameter ids is null");
			}

			var companyEntities = _repository.Company.GetByIds(ids, trackChanges: false);

			if (ids.Count() != companyEntities.Count())
			{
				_logger.LogError("Some ids are not valid in a collection");
				return NotFound();
			}

			var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
			return Ok(companiesToReturn);
		}

		// POST api/companies
		[HttpPost]
		public IActionResult PostCompany(CompanyForCreationDto company)
		{
			if (company == null)
			{
				_logger.LogError("CompanyForCreationDto object sent from client is null");
				return BadRequest("Company is null");
			}

			var companyEntity = _mapper.Map<Company>(company);
			_repository.Company.CreateCompany(companyEntity);
			_repository.Save();

			var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

			return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
		}

		[HttpPost("collection")]
		public IActionResult CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection)
		{
			if (companyCollection == null)
			{
				_logger.LogError("Company collection sent from client is null.");
				return BadRequest("Company collection is null");
			}

			var companyEntities = _mapper.Map<IEnumerable<Company>>(companyCollection);
			foreach (var company in companyEntities)
			{
				_repository.Company.CreateCompany(company);
			}

			_repository.Save();

			var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
			var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
			return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
		}

		// PUT api/companies/5
		[HttpPut("{id}")]
		public IActionResult UpdateCompany(Guid id, CompanyForUpdateDto company)
		{
			if (company == null)
			{
				_logger.LogError("CompanyForUpdateDto object sent from client is null.");
				return BadRequest("CompanyForUpdateDto object is null");
			}

			var companyEntity = _repository.Company.GetCompany(id, trackChanges: true);
			if (companyEntity == null)
			{
				_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			_mapper.Map(company, companyEntity);
			_repository.Save();

			return NoContent();
		}

		// DELETE api/companies/5
		[HttpDelete("{id}")]
		public IActionResult DeleteCompany(Guid id)
		{
			var company = _repository.Company.GetCompany(id, trackChanges: false);
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			_repository.Company.DeleteCompany(company);
			_repository.Save();

			return NoContent();
		}

		[HttpPatch("{id}")]
		public IActionResult PartiallyUpdateEmployeeForCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDoc)
		{
			if (patchDoc == null)
			{
				_logger.LogError("patchDoc object sent from client is null.");
				return BadRequest("patchDoc object is null");
			}
			var company = _repository.Company.GetCompany(id, trackChanges: false);
			
			if (company == null)
			{
				_logger.LogInfo($"Company with id: {id} doesn't exist in the database.");
				return NotFound();
			}

			var companyToPatch = _mapper.Map<CompanyForUpdateDto>(company);
			patchDoc.ApplyTo(companyToPatch);
			_mapper.Map(companyToPatch, company);
			_repository.Save();
			return NoContent();
		}
	}
}