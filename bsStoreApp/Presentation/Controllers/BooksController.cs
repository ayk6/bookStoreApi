using System.Text.Json;
using Entities.DataTransferObjects;
using Entities.RequestFeatures;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Services.Contracts;

namespace Presentation.Controllers
{
	//[ApiVersion("1.0")]
	[ServiceFilter(typeof(LogFilterAttribute))]
	[ApiController]
	[Route("api/books")]
	//[ResponseCache(CacheProfileName ="5mins")]
	[HttpCacheExpiration(CacheLocation = CacheLocation.Public, MaxAge = 60)]
	[ApiExplorerSettings(GroupName = "v1")]
	public class BooksController : ControllerBase
	{
		private readonly IServiceManager _manager;

		public BooksController(IServiceManager manager)
		{
			_manager = manager;
		}

		[Authorize]
		[HttpHead]
		[HttpGet]
		//[ResponseCache(Duration = 60)]
		public async Task<IActionResult> GetAllBooksAsync([FromQuery]BookParams bookParams)
		{
			var pagedResult = await _manager.BookService.GetAllBooksAsync(bookParams, false);
			Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(pagedResult.metaData));
			return Ok(pagedResult.books);
		}

		[HttpGet("{id:int}")]
		public async Task<IActionResult> GetBookByIdAsync([FromRoute] int id)
		{
			var book = await _manager.BookService.GetBookByIdAsync(id, false);
			return Ok(book);
		}

		[Authorize(Roles = "Admin, Editor")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		[HttpPost]
		public async Task<IActionResult> AddBookAsync([FromBody] BookDtoForInsertion bookDto)
		{
			var book = await _manager.BookService.CreateBookAsync(bookDto);
			return StatusCode(201, book);
		}

		[Authorize(Roles = "Admin")]
		[ServiceFilter(typeof(ValidationFilterAttribute))]
		[HttpPut("{id:int}")]
		public async Task<IActionResult> UpdateBookAsync(int id, [FromBody] BookDtoForUpdate bookDto)
		{
			await _manager.BookService.UpdateBookAsync(id, bookDto, false);
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpDelete("{id:int}")]
		public async Task<IActionResult> DeleteBookAsync([FromRoute] int id)
		{
			await _manager.BookService.DeleteBookAsync(id, false);
			return NoContent();
		}

		[Authorize(Roles = "Admin")]
		[HttpPatch("{id:int}")]
		public async Task<IActionResult> PatchBookAsync([FromRoute] int id,
			[FromBody] JsonPatchDocument<BookDTO> bookPatch)
		{
			if (bookPatch is null) return BadRequest();
			if (!ModelState.IsValid)
			{
				return UnprocessableEntity(ModelState);
			}
			var bookDTO = await _manager.BookService.GetBookByIdAsync(id, false);
			bookPatch.ApplyTo(bookDTO, ModelState);
			TryValidateModel(bookDTO);
			await _manager.BookService.UpdateBookAsync(id, 
				new BookDtoForUpdate()
				{
					Id = bookDTO.Id,
					Title = bookDTO.Title,
					Price = bookDTO.Price,
				}, 
				true);
			return NoContent();
		}

		[Authorize]
		[HttpGet("details")]
		public async Task<IActionResult> GetAllBooksWithDetailsAsync()
		{
			return Ok(_manager.BookService.GetAllBooksWithDetailsAsync(false));
		}

		[Authorize]
		[HttpOptions]
		public IActionResult GetBooksOptions()
		{
			Response.Headers.Add("Allow", "GET, PUT, POST, PATCH, DELETE, HEAD, OPTİONS");
			return NoContent();
		}
	}

}
