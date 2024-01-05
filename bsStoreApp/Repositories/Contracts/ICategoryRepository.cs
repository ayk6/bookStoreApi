using Entities.Models;

namespace Repositories.Contracts
{
	public interface ICategoryRepository : IRepositoryBase<Category>
	{
		Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges);
		Task<Category> GetCategoryByIdAsync(int id, bool trackChanges);
		void CreateCategory(Category category);
		void UpdateCategory(Category category);
		void DeleteCategory(Category category);
	}
}
