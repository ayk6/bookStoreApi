using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
	public class ServiceManager : IServiceManager
	{
        private readonly IBookService _bookService;
        private readonly ICategoryService _categoryService;
        private readonly IAuthService _authService;

		public ServiceManager(IBookService bookService, ICategoryService categoryService, IAuthService authService)
		{
			_bookService = bookService;
			_categoryService = categoryService;
			_authService = authService;
		}

		public IBookService BookService => _bookService;

		public IAuthService AuthService => _authService;

		public ICategoryService CategoryService => _categoryService;
	}
}
