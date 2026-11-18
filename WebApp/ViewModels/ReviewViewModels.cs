using System.ComponentModel.DataAnnotations;

namespace WebApp.ViewModels;


/// View model used by MVC review pages to display and edit review information.

public class ReviewViewModel
{
    public int Id { get; set; }

    [Display(Name = "User")]
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    [Display(Name = "Game")]
    public int GameId { get; set; }

    public string GameName { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Rating { get; set; }

    [Display(Name = "Review Text")]
    public string? ReviewText { get; set; }

    public bool Approved { get; set; }

    [Display(Name = "Created At")]
    public DateTime CreatedAt { get; set; }

    
    /// Performs the new operation for this class.
   
    public List<ReviewImageViewModel> ReviewImages { get; set; } = new();
}


/// View model used to display uploaded images connected to a review.

public class ReviewImageViewModel
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
}
