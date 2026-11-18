using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents the Game table and stores core game data plus relationships to game type, genres, images, and reviews.

public partial class Game
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateOnly ReleaseDate { get; set; }

    public string Developer { get; set; } = null!;

    public int GameTypeId { get; set; }

    public virtual GameType GameType { get; set; } = null!;

    
    /// Performs the Image operation for this class.
   
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    
    /// Performs the Review operation for this class.
   
    public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();

    
    /// Performs the Genre operation for this class.
   
    public virtual ICollection<Genre> Genres { get; set; } = new List<Genre>();
}
