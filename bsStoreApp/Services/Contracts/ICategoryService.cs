using Entities.DataTransferObjects;
using Entities.Models;

namespace Services.Contracts
{
	public interface ICategoryService
	{
		Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges);
		Task<Category> GetCategoryByIdAsync(int id, bool trackChanges);
		Task<Category> CreateCategoryAsync(Category category);
		Task UpdateCategoryAsync(int id, Category category, bool trackChanges);
		Task DeleteCategoryAsync(int id, bool trackChanges);
	}
}
