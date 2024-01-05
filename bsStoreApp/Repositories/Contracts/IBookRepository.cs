using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities.Models;
using Entities.RequestFeatures;

namespace Repositories.Contracts
{
	public interface IBookRepository : IRepositoryBase<Book>
	{
		Task<PagedList<Book>> GetAllBooksAsync(BookParams bookParams, bool trackChanges);
		Task<Book> GetBookByIdAsync(int id, bool trackChanges);
		void CreateBook(Book book);
		void UpdateBook(Book book);
		void DeleteBook(Book book);
		Task<List<Book>> GetAllBooksAsync(bool trackChanges);
		Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges);
	}
}
