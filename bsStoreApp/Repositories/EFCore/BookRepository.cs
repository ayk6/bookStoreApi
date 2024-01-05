using Entities.Models;
using Entities.RequestFeatures;
using Microsoft.EntityFrameworkCore;
using Repositories.Contracts;
using Repositories.EFCore.Extensions;

namespace Repositories.EFCore
{
    public sealed class BookRepository : RepositoryBase<Book>, IBookRepository
	{
		public BookRepository(RepositoryContext context) : base(context)
		{

		}

		public void CreateBook(Book book) =>CreateBook(book);

		public void DeleteBook(Book book) => DeleteBook(book);

		public async Task<PagedList<Book>> GetAllBooksAsync(BookParams bookParams,bool trackChanges)
		{
			var books = await FindAll(trackChanges)
				.FilterBooks(bookParams.MinPrice,bookParams.MaxPrice)
				.Search(bookParams.SearchTerm)
				.SortBooks(bookParams.OrderBy)
				.ToListAsync();

			return PagedList<Book>.ToPagedList(books, bookParams.PageNumber, bookParams.PageSize);
		}

		public async Task<List<Book>> GetAllBooksAsync(bool trackChanges)
		{
				return await FindAll(trackChanges)
				.OrderBy(b=> b.Id)
				.ToListAsync();
		}

		public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges)
		{
			return await _context.Books.Include(b => b.CategoryId).OrderBy(b=>b.Id).ToListAsync();
		}

		public async Task<Book> GetBookByIdAsync(int id, bool trackChanges) =>
			await FindByConditions(b => b.Id.Equals(id), trackChanges).SingleOrDefaultAsync();

		public void UpdateBook(Book book) => UpdateBook(book);
	}
}
