using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameMonitoring.Business.DTOs;
using GameMonitoring.Data.Models;

namespace WebAPI.Controllers;


/// Controller responsible for listing, creating, editing, deleting, approving, and rejecting game reviews.

[ApiController]
[Route("api/[controller]")]
public class ReviewsController : ControllerBase
{
    private readonly GameMonitoringContext _context;
    private readonly IMapper _mapper;

    
    /// Initializes the ReviewsController and receives required services through dependency injection.
   
    public ReviewsController(GameMonitoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    /// Returns all records for this resource from the database/API.
   
    [HttpGet]
    public async Task<ActionResult<List<ReviewDto>>> GetAll()
    {
        var reviews = await ReviewQuery().OrderByDescending(r => r.CreatedAt).ToListAsync();
        return Ok(_mapper.Map<List<ReviewDto>>(reviews));
    }

    
    /// Returns one record by ID, or a not-found response if the record does not exist.
   
    [HttpGet("{id}")]
    public async Task<ActionResult<ReviewDto>> GetById(int id)
    {
        var review = await ReviewQuery().FirstOrDefaultAsync(r => r.Id == id);
        if (review == null)
            return NotFound($"Review with id={id} was not found.");

        return Ok(_mapper.Map<ReviewDto>(review));
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    public async Task<IActionResult> Create(CreateReviewDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 10)
            return BadRequest("Rating must be between 1 and 10.");

        if (!await _context.Users.AnyAsync(u => u.Id == dto.UserId))
            return BadRequest($"User with id={dto.UserId} does not exist.");

        if (!await _context.Games.AnyAsync(g => g.Id == dto.GameId))
            return BadRequest($"Game with id={dto.GameId} does not exist.");

        if (await _context.Reviews.AnyAsync(r => r.UserId == dto.UserId && r.GameId == dto.GameId))
            return BadRequest("This user has already reviewed this game.");

        var review = new Review
        {
            UserId = dto.UserId,
            GameId = dto.GameId,
            Rating = dto.Rating,
            ReviewText = dto.ReviewText,
            Approved = false,
            CreatedAt = DateTime.Now
        };

        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();

        var created = await ReviewQuery().FirstAsync(r => r.Id == review.Id);
        return CreatedAtAction(nameof(GetById), new { id = review.Id }, _mapper.Map<ReviewDto>(created));
    }

    
    /// Adds a review image URL to an existing review after validating the review ID.
   
    [HttpPost("{id}/images")]
    public async Task<IActionResult> AddImage(int id, CreateReviewImageDto dto)
    {
        if (id != dto.ReviewId)
            return BadRequest("Route id and review id do not match.");

        if (!await _context.Reviews.AnyAsync(r => r.Id == id))
            return NotFound($"Review with id={id} was not found.");

        var image = new ReviewImage
        {
            ReviewId = id,
            Url = dto.Url
        };

        _context.ReviewImages.Add(image);
        await _context.SaveChangesAsync();

        return Ok(new ReviewImageDto { Id = image.Id, Url = image.Url });
    }

    
    /// Updates an existing record after checking that it exists and that the submitted data is valid.
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateReviewDto dto)
    {
        if (dto.Rating < 1 || dto.Rating > 10)
            return BadRequest("Rating must be between 1 and 10.");

        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound($"Review with id={id} was not found.");

        if (await _context.Reviews.AnyAsync(r => r.UserId == dto.UserId && r.GameId == dto.GameId && r.Id != id))
            return BadRequest("This user has already reviewed this game.");

        review.UserId = dto.UserId;
        review.GameId = dto.GameId;
        review.Rating = dto.Rating;
        review.ReviewText = dto.ReviewText;
        review.Approved = false;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Marks the selected review as approved so it can be shown publicly.
   
    [HttpPut("{id}/approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound($"Review with id={id} was not found.");

        review.Approved = true;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Marks the selected review as not approved so it is hidden from public display.
   
    [HttpPut("{id}/reject")]
    public async Task<IActionResult> Reject(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound($"Review with id={id} was not found.");

        review.Approved = false;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var review = await _context.Reviews.FindAsync(id);
        if (review == null)
            return NotFound($"Review with id={id} was not found.");

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Builds the reusable review query with related user, game, and review image data included.
   
    private IQueryable<Review> ReviewQuery()
    {
        return _context.Reviews
            .Include(r => r.User)
            .Include(r => r.Game)
            .Include(r => r.ReviewImages)
            .AsQueryable();
    }
}
