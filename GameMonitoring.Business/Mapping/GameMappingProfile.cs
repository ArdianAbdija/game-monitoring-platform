using AutoMapper;
using GameMonitoring.Business.DTOs;
using GameMonitoring.Data.Models;

namespace GameMonitoring.Business.Mapping;


/// AutoMapper profile that defines how database entities are converted to DTOs and how create DTOs are converted back to entities.

public class GameMappingProfile : Profile
{
    
    /// Registers all AutoMapper mapping rules used by the API.
   
    public GameMappingProfile()
    {
        CreateMap<GameType, LookupDto>();
        CreateMap<Genre, LookupDto>();
        CreateMap<ReviewImage, ReviewImageDto>();

        CreateMap<Review, ReviewDto>()
            .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.User.Username))
            .ForMember(dest => dest.GameName, opt => opt.MapFrom(src => src.Game.Name));

        CreateMap<Game, GameDto>()
            .ForMember(dest => dest.GameTypeName, opt => opt.MapFrom(src => src.GameType.Name))
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.Images.Select(image => image.Url).ToList()))
            .ForMember(dest => dest.AverageRating, opt => opt.MapFrom(src =>
                src.Reviews.Any(r => r.Approved)
                    ? src.Reviews.Where(r => r.Approved).Average(r => r.Rating)
                    : (double?)null));

        CreateMap<CreateGameDto, Game>();
        CreateMap<User, UserDto>();
    }
}
