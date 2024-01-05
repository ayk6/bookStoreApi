using Entities.RequestFeatures;
using Microsoft.AspNetCore.Mvc;
using Services.Contracts;

namespace Presentation.Controllers
{
	//[ApiVersion("2.0" , Deprecated = true)]
	[ApiController]
	[Route("api/books")]
	[ApiExplorerSettings(GroupName = "v2")]
	internal class BooksControllerV2 : ControllerBase
	{
		private readonly IServiceManager _manager;

		public BooksControllerV2(IServiceManager manager)
		{
			_manager = manager;
		}

		[HttpGet]
		public async Task<IActionResult> GetAllBooksAsync()
		{
			var books = await _manager.BookService.GetAllBooksAsync(false);
			var booksV2 = books.Select(b => new
			{
				Title = b.Title,
				Id = b.Id,
			});
			return Ok(booksV2);
		}
	}
}
