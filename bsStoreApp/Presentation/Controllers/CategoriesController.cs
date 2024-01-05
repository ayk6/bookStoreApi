using System.Collections;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;

namespace Presentation.Controllers
{
	[ApiController]
	[Route("api/categories")]
	public class CategoriesController : ControllerBase
	{
		private readonly IServiceManager _serviceManager;
		public CategoriesController(IServiceManager serviceManager)
		{
			_serviceManager = serviceManager;
		}

		[HttpGet]
		[HttpHead]
		public async Task<IActionResult> GetAllCategoriesAsync()
		{
			return Ok(await _serviceManager.CategoryService.GetAllCategoriesAsync(false));
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetCategoryByIdAsync([FromRoute] int id)
		{
			return Ok(await _serviceManager.CategoryService.GetCategoryByIdAsync(id,false));
		}

		[Authorize(Roles = "Admin, Editor")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		[HttpPost]
		public async Task<IActionResult> AddCategoryAsync([FromBody] Category category)
		{
			return StatusCode(201, await _serviceManager.CategoryService.CreateCategoryAsync(category));
		}

		[Authorize(Roles = "Admin")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		[HttpPut("{id:int}")]
		public async Task<IActionResult> UpdateCategoryAsync([FromRoute] int id, [FromBody] Category category)
		{
			await _serviceManager.CategoryService.UpdateCategoryAsync(id, category, false);
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteCategoryAsync([FromRoute] int id)
		{
			await _serviceManager.CategoryService.DeleteCategoryAsync(id, false);
			return NoContent();
		}


		[Authorize]
		[HttpOptions]
		public IActionResult GetCategoryOptions()
		{
			Response.Headers.Add("Allow", "GET, PUT, POST, DELETE, HEAD, OPTİONS");
			return NoContent();
		}
	}
}

