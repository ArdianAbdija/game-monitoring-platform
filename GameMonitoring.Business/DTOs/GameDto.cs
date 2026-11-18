namespace GameMonitoring.Business.DTOs;


/// DTO that represents a game as it is sent from the API to the MVC application, including related display data.

public class GameDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateOnly ReleaseDate { get; set; }
    public string Developer { get; set; } = null!;
    public int GameTypeId { get; set; }
    public string GameTypeName { get; set; } = null!;

    
    /// Performs the new operation for this class.
   
    public List<LookupDto> Genres { get; set; } = new();
    
    /// Performs the new operation for this class.
   
    public List<ReviewDto> Reviews { get; set; } = new();
    
    /// Performs the new operation for this class.
   
    public List<string> ImageUrls { get; set; } = new();
    public double? AverageRating { get; set; }

}


/// DTO used when creating or updating a game through the API.

public class CreateGameDto
{
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    public DateOnly ReleaseDate { get; set; }
    public string Developer { get; set; } = null!;
    public int GameTypeId { get; set; }
}


/// DTO used to assign one or more genre IDs to a game.

public class AssignGenresDto
{
    
    /// Performs the new operation for this class.
   
    public List<int> GenreIds { get; set; } = new();
}
