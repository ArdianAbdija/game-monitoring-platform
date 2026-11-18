using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents an image URL connected directly to a game.

public partial class Image
{
    public int Id { get; set; }

    public int GameId { get; set; }

    public string Url { get; set; } = null!;

    public virtual Game Game { get; set; } = null!;
}
