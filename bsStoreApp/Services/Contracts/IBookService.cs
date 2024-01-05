using System.Dynamic;
using Entities.DataTransferObjects;
using Entities.Models;
using Entities.RequestFeatures;

namespace Services.Contracts
{
	public interface IBookService
	{
		Task<(IEnumerable<ExpandoObject> books, MetaData metaData)> GetAllBooksAsync(BookParams bookParams, bool trackChanges);
		Task<BookDTO> GetBookByIdAsync(int id, bool trackChanges);
		Task<BookDTO> CreateBookAsync(BookDtoForInsertion bookDto);
		Task UpdateBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges);
		Task DeleteBookAsync(int id, bool trackChanges);
		Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> PatchBookAsync(int id, bool trackChanges);
		Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book);
		Task<List<BookDTO>> GetAllBooksAsync(bool trackChanges);
		Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges);
	}
}
