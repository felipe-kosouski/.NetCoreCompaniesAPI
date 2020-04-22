using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CompanyEmployees.ActionFilters;
using CompanyEmployees.Contracts;
using CompanyEmployees.Dtos;
using CompanyEmployees.Models;
using Contracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
//using CompanyEmployees.Models;

namespace CompanyEmployees.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class AuthenticationController : ControllerBase
	{
		private readonly ILoggerManager _logger;
		private readonly IMapper _mapper;
		private UserManager<User> _userManager;
		private readonly IAuthenticationManager _authManager;

		public AuthenticationController(ILoggerManager logger, IMapper mapper, UserManager<User> userManager, IAuthenticationManager authManager)
		{
			_logger = logger;
			_mapper = mapper;
			_userManager = userManager;
			_authManager = authManager;
		}

		[HttpPost]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		public async Task<IActionResult> RegisterUser(UserForRegistrationDto userForRegistration)
		{
			var user = _mapper.Map<User>(userForRegistration);
			var result = await _userManager.CreateAsync(user, userForRegistration.Password);

			if (!result.Succeeded)
			{
				foreach (var error in result.Errors)
				{
					ModelState.TryAddModelError(error.Code, error.Description);
				}
				return BadRequest();
			}
			await _userManager.AddToRolesAsync(user, userForRegistration.Roles);
			return StatusCode(201);
		}

		[HttpPost("login")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		public async Task<IActionResult> Authenticate(UserForAuthenticationDto user)
		{
			if (!await _authManager.ValidateUser(user))
			{
				_logger.LogWarn($"{nameof(Authenticate)}: Authentication failed. Wrong user name or password.");
				return Unauthorized();
			}

			return Ok(new { Token = await _authManager.CreateToken() });
		}
	}
}