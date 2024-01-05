using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
	public class CategoryManager : ICategoryService
	{
		private readonly IRepositoryManager _manager;
		public CategoryManager(IRepositoryManager manager)
		{
			_manager = manager;
		}
		public async Task<Category> CreateCategoryAsync(Category category)
		{
			_manager.CategoryRepository.Create(category);
			await _manager.SaveAsync();
			return category;
		}

		public async Task DeleteCategoryAsync(int id, bool trackChanges)
		{
			var category = await _manager.CategoryRepository.GetCategoryByIdAsync(id, trackChanges);
			_manager.CategoryRepository.Delete(category);
			await _manager.SaveAsync();
		}

		public async Task<IEnumerable<Category>> GetAllCategoriesAsync(bool trackChanges)
		{
			return await _manager.CategoryRepository.GetAllCategoriesAsync(trackChanges);
		}

		public async Task<Category> GetCategoryByIdAsync(int id, bool trackChanges)
		{
			var category = await _manager.CategoryRepository.GetCategoryByIdAsync(id, trackChanges);
			if (category is null) throw new CategoryNotFoundException(id);
			return category;
		}

		public async Task UpdateCategoryAsync(int id, Category newCategory, bool trackChanges)
		{
			var category = await _manager.CategoryRepository.GetCategoryByIdAsync(id, trackChanges);
			category = newCategory;
			_manager.CategoryRepository.Update(category);
			await _manager.SaveAsync();
		}
	}
}
