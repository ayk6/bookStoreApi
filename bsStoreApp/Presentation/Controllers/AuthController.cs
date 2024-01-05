using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.DataTransferObjects;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;

namespace Presentation.Controllers
{
	[ApiController]
	[Route("api/auth")]
	[ApiExplorerSettings(GroupName = "v1")]
	public class AuthController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;

		public AuthController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[HttpPost]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		public async Task<IActionResult> RegisterUser([FromBody]UserForRegisterDTO userForRegisterDTO)
		{
			var result = await _serviceManager.AuthService.RegisterUserAsync(userForRegisterDTO);
			
			if (!result.Succeeded)
			{
				foreach( var error in result.Errors )
				{
					ModelState.TryAddModelError(error.Code, error.Description);
				}
				return BadRequest(ModelState);
			}
			return StatusCode(201);
		}

		[HttpPost("login")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		public async Task<IActionResult> Authenticate([FromBody]UserForAuthDTO userForAuthDTO)
		{
			if (!await _serviceManager.AuthService.ValidateUser(userForAuthDTO)) {
				return Unauthorized();
			}
			var tokenDTO = await _serviceManager.AuthService.CreateToken(populateExp: true);
			return Ok(tokenDTO);
		}

		[HttpPost("refresh")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		public async Task<IActionResult> Refrest([FromBody]TokenDTO tokenDTO)
		{
			var tokenDtoReturn = await _serviceManager.AuthService.RefreshToken(tokenDTO);
			return Ok(tokenDtoReturn);
		}
	}
}
