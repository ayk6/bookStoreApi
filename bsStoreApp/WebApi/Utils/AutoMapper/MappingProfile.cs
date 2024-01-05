using AutoMapper;
using Entities.DataTransferObjects;
using Entities.Models;

namespace WebApi.Utils.AutoMapper
{
	public class MappingProfile : Profile
	{
        public MappingProfile()
        {
            CreateMap<BookDtoForUpdate, Book>().ReverseMap();
			CreateMap<BookDtoForInsertion, Book>();
			CreateMap<Book, BookDTO>();
            CreateMap<UserForRegisterDTO, User>();
        }
    }
}
