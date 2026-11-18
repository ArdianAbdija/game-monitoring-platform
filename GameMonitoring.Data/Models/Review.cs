using System;
using System.Collections.Generic;

namespace GameMonitoring.Data.Models;


/// Entity model that represents a user review, including rating, text, approval status, and related review images.

public partial class Review
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int GameId { get; set; }

    public int Rating { get; set; }

    public string? ReviewText { get; set; }

    public bool Approved { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Game Game { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    
    /// Performs the ReviewImage operation for this class.
   
    public virtual ICollection<ReviewImage> ReviewImages { get; set; } = new List<ReviewImage>();
}
