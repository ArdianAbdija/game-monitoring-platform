namespace GameMonitoring.Data.Models;


/// Entity model that represents an image uploaded together with a review.

public partial class ReviewImage
{
    public int Id { get; set; }

    public int ReviewId { get; set; }

    public string Url { get; set; } = null!;

    public virtual Review Review { get; set; } = null!;
}
