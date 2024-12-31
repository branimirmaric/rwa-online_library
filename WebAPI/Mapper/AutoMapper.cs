using AutoMapper;
using WebAPI.Dtos;
using WebAPI.Models;

namespace WebAPI.Mapper
{
    public class AutoMapper : Profile
    {
        public AutoMapper()
        {
            CreateMap<Book, BookDto>().ReverseMap();
            CreateMap<Genre, GenreDto>().ReverseMap();
            CreateMap<Image, ImageDto>().ReverseMap();
            CreateMap<Location, LocationDto>().ReverseMap();
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<UserReservation, UserReservationDto>().ReverseMap();
        }
    }
}
