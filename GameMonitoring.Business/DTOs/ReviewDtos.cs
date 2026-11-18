namespace GameMonitoring.Business.DTOs;


/// DTO that represents a review as it is sent from the API to the MVC application.

public class ReviewDto
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public string Username { get; set; } = null!;
    public int GameId { get; set; }
    public string GameName { get; set; } = null!;
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
    public bool Approved { get; set; }
    public DateTime CreatedAt { get; set; }
    
    /// Performs the new operation for this class.
   
    public List<ReviewImageDto> ReviewImages { get; set; } = new();
}


/// DTO that represents one uploaded image connected to a review.

public class ReviewImageDto
{
    public int Id { get; set; }
    public string Url { get; set; } = null!;
}


/// DTO used when creating or updating a review.

public class CreateReviewDto
{
    public int UserId { get; set; }
    public int GameId { get; set; }
    public int Rating { get; set; }
    public string? ReviewText { get; set; }
}


/// DTO used when adding an image URL to an existing review.

public class CreateReviewImageDto
{
    public int ReviewId { get; set; }
    public string Url { get; set; } = null!;
}
