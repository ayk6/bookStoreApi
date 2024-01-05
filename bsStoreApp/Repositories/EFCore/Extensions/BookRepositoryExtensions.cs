using System.Reflection;
using System.Text;
using Entities.Models;
using System.Linq.Dynamic.Core;


namespace Repositories.EFCore.Extensions
{
    public static class BookRepositoryExtensions
    {
        public static IQueryable<Book> FilterBooks(this IQueryable<Book> books, uint minPrice, uint maxPrice) =>
            books.Where(book => book.Price >= minPrice && book.Price <= maxPrice);

        public static IQueryable<Book> Search(this IQueryable<Book> books, string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm)) return books;

            return books.Where(book => book.Title.Contains(searchTerm));
		}

        public static IQueryable<Book> SortBooks(this IQueryable<Book> books, string orderByQueryString) 
        {
			if (string.IsNullOrWhiteSpace(orderByQueryString)) return books.OrderBy(b => b.Id);

            var orderParams = orderByQueryString.Trim().Split(',');

            var orderQuery = OrderQueryBuilder.CreateOrderQuery<Book>(orderByQueryString);

            if (orderQuery is null) return books.OrderBy(b => b.Id);

            return books.OrderBy(orderQuery);
		}
        
			
	}
}
