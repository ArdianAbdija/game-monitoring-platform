using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents a game genre and its many-to-many relationship with games.

public partial class Genre
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    
    /// Performs the Game operation for this class.
   
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
