using GameMonitoring.Business.DTOs;

namespace WebApp.ViewModels;


/// Static helper class that converts DTOs from the API into MVC view models and converts form view models back into DTOs.

public static class ViewModelMappingExtensions
{
    
    /// Converts an API DTO into the matching MVC view model used by Razor views.
   
    public static GameViewModel ToViewModel(this GameDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name,
        Description = dto.Description,
        ReleaseDate = dto.ReleaseDate,
        Developer = dto.Developer,
        GameTypeId = dto.GameTypeId,
        GameTypeName = dto.GameTypeName,
        Genres = dto.Genres,
        Reviews = dto.Reviews.Select(r => r.ToViewModel()).ToList(),
        ImageUrls = dto.ImageUrls,
        AverageRating = dto.AverageRating
    };

    
    /// Converts an MVC view model into the DTO expected by the API create/update endpoint.
   
    public static CreateGameDto ToCreateDto(this GameViewModel vm) => new()
    {
        Name = vm.Name,
        Description = vm.Description,
        ReleaseDate = vm.ReleaseDate,
        Developer = vm.Developer,
        GameTypeId = vm.GameTypeId
    };

    
    /// Converts an API DTO into the matching MVC view model used by Razor views.
   
    public static LookupViewModel ToViewModel(this LookupDto dto) => new()
    {
        Id = dto.Id,
        Name = dto.Name
    };

    
    /// Converts an MVC lookup view model into the simple lookup DTO expected by the API.
   
    public static LookupDto ToDto(this LookupViewModel vm) => new()
    {
        Id = vm.Id,
        Name = vm.Name
    };

    
    /// Converts an API DTO into the matching MVC view model used by Razor views.
   
    public static ReviewViewModel ToViewModel(this ReviewDto dto) => new()
    {
        Id = dto.Id,
        UserId = dto.UserId,
        Username = dto.Username,
        GameId = dto.GameId,
        GameName = dto.GameName,
        Rating = dto.Rating,
        ReviewText = dto.ReviewText,
        Approved = dto.Approved,
        CreatedAt = dto.CreatedAt,
        ReviewImages = dto.ReviewImages.Select(i => new ReviewImageViewModel { Id = i.Id, Url = i.Url }).ToList()
    };

    
    /// Converts an MVC view model into the DTO expected by the API create/update endpoint.
   
    public static CreateReviewDto ToCreateDto(this ReviewViewModel vm) => new()
    {
        UserId = vm.UserId,
        GameId = vm.GameId,
        Rating = vm.Rating,
        ReviewText = vm.ReviewText
    };

    
    /// Converts an API DTO into the matching MVC view model used by Razor views.
   
    public static UserProfileViewModel ToViewModel(this UserDto dto) => new()
    {
        Id = dto.Id,
        Username = dto.Username,
        Email = dto.Email,
        Role = dto.Role,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        PhoneNumber = dto.PhoneNumber
    };

    
    /// Converts a user profile view model into the DTO expected by the API profile update endpoint.
   
    public static UpdateUserProfileDto ToUpdateDto(this UserProfileViewModel vm) => new()
    {
        Id = vm.Id,
        Email = vm.Email,
        FirstName = vm.FirstName,
        LastName = vm.LastName,
        PhoneNumber = vm.PhoneNumber
    };
}
