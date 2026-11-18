using System.ComponentModel.DataAnnotations;
using GameMonitoring.Business.DTOs;

namespace WebApp.ViewModels;


/// View model used by MVC game pages to display and edit game data in a UI-friendly format.

public class GameViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Game name is required.")]
    [StringLength(150, ErrorMessage = "Game name cannot be longer than 150 characters.")]
    [Display(Name = "Game Name")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Description is required.")]
    public string Description { get; set; } = null!;

    [Required(ErrorMessage = "Release date is required.")]
    [Display(Name = "Release Date")]
    public DateOnly ReleaseDate { get; set; }

    [Required(ErrorMessage = "Developer is required.")]
    [StringLength(150, ErrorMessage = "Developer name cannot be longer than 150 characters.")]
    public string Developer { get; set; } = null!;

    [Required(ErrorMessage = "Game type is required.")]
    [Display(Name = "Game Type")]
    public int GameTypeId { get; set; }

    [Display(Name = "Game Type")]
    public string GameTypeName { get; set; } = string.Empty;

    
    /// Performs the new operation for this class.
   
    public List<LookupDto> Genres { get; set; } = new();
    
    /// Performs the new operation for this class.
   
    public List<ReviewViewModel> Reviews { get; set; } = new();
    
    /// Performs the new operation for this class.
   
    public List<string> ImageUrls { get; set; } = new();
    public double? AverageRating { get; set; }
}


/// View model used by the game browsing page to carry games, filters, pagination data, and lookup values.

public class GameListViewModel
{
    
    /// Performs the new operation for this class.
   
    public List<GameViewModel> Games { get; set; } = new();
    public string? Search { get; set; }
    public int? GameTypeId { get; set; }
    public int Page { get; set; } = 1;
    public int TotalPages { get; set; }
    
    /// Performs the new operation for this class.
   
    public List<LookupDto> GameTypes { get; set; } = new();
}
