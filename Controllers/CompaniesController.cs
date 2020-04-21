using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Dtos;
using CompanyEmployees.ModelBinders;
using CompanyEmployees.Models;
using Contracts;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
//using CompanyEmployees.Models;

namespace CompanyEmployees.Controllers
{
	[ApiVersion("1.0")]
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

		[HttpOptions]
		public IActionResult GetCompaniesOptions()
		{
			Response.Headers.Add("Allow", "GET, OPTIONS, POST");
			return Ok();
		}

		// GET api/companies
		[HttpGet(Name = "GetCompanies")]
		public async Task<ActionResult> GetCompanies()
		{
			var companies = await _repository.Company.GetAllCompaniesAsync(trackChanges: false);
			var companiesDto = _mapper.Map<IEnumerable<CompanyDto>>(companies);
			return Ok(companiesDto);
		}

		// GET api/companies/5
		[HttpGet("{id}", Name = "CompanyById")]
		public async Task<ActionResult> GetCompany(Guid id)
		{
			var company = await _repository.Company.GetCompanyAsync(id, trackChanges: false);
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
		public async Task<ActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
		{
			if (ids == null)
			{
				_logger.LogError("Parameter ids is null");
				return BadRequest("Parameter ids is null");
			}

			var companyEntities = await _repository.Company.GetByIdsAsync(ids, trackChanges: false);

			if (ids.Count() != companyEntities.Count())
			{
				_logger.LogError("Some ids are not valid in a collection");
				return NotFound();
			}

			var companiesToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
			return Ok(companiesToReturn);
		}

		// POST api/companies
		[HttpPost(Name = "CreateCompany")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		public async Task<ActionResult> CreateCompany(CompanyForCreationDto company)
		{
			var companyEntity = _mapper.Map<Company>(company);
			_repository.Company.CreateCompany(companyEntity);
			await _repository.SaveAsync();

			var companyToReturn = _mapper.Map<CompanyDto>(companyEntity);

			return CreatedAtRoute("CompanyById", new { id = companyToReturn.Id }, companyToReturn);
		}

		[HttpPost("collection")]
		public async Task<ActionResult> CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection)
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

			await _repository.SaveAsync();

			var companyCollectionToReturn = _mapper.Map<IEnumerable<CompanyDto>>(companyEntities);
			var ids = string.Join(",", companyCollectionToReturn.Select(c => c.Id));
			return CreatedAtRoute("CompanyCollection", new { ids }, companyCollectionToReturn);
		}

		// PUT api/companies/5
		[HttpPut("{id}")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		[ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
		public async Task<ActionResult> UpdateCompany(Guid id, CompanyForUpdateDto company)
		{
			var companyEntity = HttpContext.Items["company"] as Company;

			_mapper.Map(company, companyEntity);
			await _repository.SaveAsync();

			return NoContent();
		}

		// DELETE api/companies/5
		[HttpDelete("{id}")]
		[ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
		public async Task<ActionResult> DeleteCompany(Guid id)
		{
			var company = HttpContext.Items["company"] as Company;

			_repository.Company.DeleteCompany(company);
			await _repository.SaveAsync();

			return NoContent();
		}

		[HttpPatch("{id}")]
		[ServiceFilter(typeof(ValidateCompanyExistsAttribute))]
		public async Task<ActionResult> PartiallyUpdateEmployeeForCompany(Guid id, [FromBody] JsonPatchDocument<CompanyForUpdateDto> patchDoc)
		{
			if (patchDoc == null)
			{
				_logger.LogError("patchDoc object sent from client is null.");
				return BadRequest("patchDoc object is null");
			}

			var company = HttpContext.Items["company"] as Company;

			var companyToPatch = _mapper.Map<CompanyForUpdateDto>(company);
			patchDoc.ApplyTo(companyToPatch, ModelState);
			TryValidateModel(companyToPatch);
			if (!ModelState.IsValid)
			{
				_logger.LogError("Invalid model state for the patch document");
				return UnprocessableEntity(ModelState);
			}
			_mapper.Map(companyToPatch, company);
			await _repository.SaveAsync();
			return NoContent();
		}
	}
}