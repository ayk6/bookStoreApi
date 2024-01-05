using System.Collections.Generic;
using System.Dynamic;
using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Exceptions;
using Entities.Models;
using Entities.RequestFeatures;
using Repositories.Contracts;
using Services.Contracts;

namespace Services
{
	public class BookManager : IBookService
	{
		private readonly IRepositoryManager _manager;
		private readonly ICategoryService _categoryService;
		private readonly ILoggerService _logger;
		private readonly IMapper _mapper;
		private readonly IDataShaper<BookDTO> _shaper;
		public BookManager(IRepositoryManager manager, ILoggerService logger, IMapper mapper, IDataShaper<BookDTO> shaper, ICategoryService categoryService)
		{
			_manager = manager;
			_logger = logger;
			_mapper = mapper;
			_shaper = shaper;
			_categoryService = categoryService;
		}

		public async Task<BookDTO> CreateBookAsync(BookDtoForInsertion bookDto)
		{
			await _categoryService.GetCategoryByIdAsync(bookDto.CategoryId, false);
			var book = _mapper.Map<Book>(bookDto);
			_manager.BookRepository.Create(book);
			await _manager.SaveAsync();
			return _mapper.Map<BookDTO>(book);
		}

		public async Task DeleteBookAsync(int id, bool trackChanges)
		{
			var book = await CheckAndGetBook(id, trackChanges);
			_manager.BookRepository.Delete(book);
			await _manager.SaveAsync();
		}

		public async Task<(IEnumerable<ExpandoObject> books,MetaData metaData)> GetAllBooksAsync(BookParams bookParams, bool trackChanges)
		{
			if (!bookParams.ValidPriceRange) 
			{
				throw new PriceOutOfRangeException();
			}
			var booksWithMetaData = await _manager.BookRepository.GetAllBooksAsync(bookParams, trackChanges);
			var booksDTO = _mapper.Map<IEnumerable<BookDTO>>(booksWithMetaData);
			var shapedData = _shaper.ShapeData(booksDTO, bookParams.Fields);
			return (books: shapedData, booksWithMetaData.MetaData);

		}

		public async Task<List<BookDTO>> GetAllBooksAsync(bool trackChanges)
		{
			var books = await _manager.BookRepository.GetAllBooksAsync(trackChanges);
			var bookDTO = _mapper.Map<IEnumerable<BookDTO>>(books);
			return bookDTO.ToList();
		}

		public async Task<IEnumerable<Book>> GetAllBooksWithDetailsAsync(bool trackChanges)
		{
			return await _manager.BookRepository.GetAllBooksWithDetailsAsync(trackChanges);
		}

		public async Task<BookDTO> GetBookByIdAsync(int id, bool trackChanges)
		{
			var book = await CheckAndGetBook(id, trackChanges);
			return _mapper.Map<BookDTO>(book);
		}

		public async Task<(BookDtoForUpdate bookDtoForUpdate, Book book)> PatchBookAsync(int id, bool trackChanges)
		{
			var book = await CheckAndGetBook(id, trackChanges);
			var bookDtoForUpdate = _mapper.Map<BookDtoForUpdate>(book);
			return (bookDtoForUpdate, book);
		}

		public async Task SaveChangesForPatchAsync(BookDtoForUpdate bookDtoForUpdate, Book book)
		{
			_mapper.Map(bookDtoForUpdate, book);
			await _manager.SaveAsync();
		}

		public async Task UpdateBookAsync(int id, BookDtoForUpdate bookDto, bool trackChanges)
		{
			await _categoryService.GetCategoryByIdAsync(bookDto.CategoryId, false);
			var book = await CheckAndGetBook(id, trackChanges);
			book = _mapper.Map<Book>(bookDto);
			_manager.BookRepository.Update(book);
			await _manager.SaveAsync();
		}

		private async Task<Book> CheckAndGetBook(int id, bool trackChanges)
		{
			var book = await _manager.BookRepository.GetBookByIdAsync(id, trackChanges);
			if (book == null)
				throw new BookNotFoundException(id);
			return book;
		}
	}
}
