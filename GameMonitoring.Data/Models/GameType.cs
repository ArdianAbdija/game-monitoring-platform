using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents a type/category of game, such as single-player, multiplayer, or another game grouping.

public partial class GameType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    
    /// Performs the Game operation for this class.
   
    public virtual ICollection<Game> Games { get; set; } = new List<Game>();
}
