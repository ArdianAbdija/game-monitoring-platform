using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameMonitoring.Business.DTOs;
using GameMonitoring.Data.Models;

namespace WebAPI.Controllers;


/// Controller responsible for listing, viewing, creating, editing, and deleting genre records.

[ApiController]
[Route("api/[controller]")]
public class GenresController : ControllerBase
{
    private readonly GameMonitoringContext _context;
    private readonly IMapper _mapper;

    
    /// Initializes the GenresController and receives required services through dependency injection.
   
    public GenresController(GameMonitoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    /// Returns all records for this resource from the database/API.
   
    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetAll()
    {
        var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        return Ok(_mapper.Map<List<LookupDto>>(genres));
    }

    
    /// Returns one record by ID, or a not-found response if the record does not exist.
   
    [HttpGet("{id}")]
    public async Task<ActionResult<LookupDto>> GetById(int id)
    {
        var genre = await _context.Genres.FindAsync(id);
        if (genre == null)
            return NotFound($"Genre with id={id} was not found.");
        return Ok(_mapper.Map<LookupDto>(genre));
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    public async Task<IActionResult> Create(LookupDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Genre name is required.");

        if (await _context.Genres.AnyAsync(g => g.Name == dto.Name))
            return BadRequest($"Genre with name '{dto.Name}' already exists.");

        var genre = new Genre { Name = dto.Name };
        _context.Genres.Add(genre);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = genre.Id }, _mapper.Map<LookupDto>(genre));
    }

    
    /// Updates an existing record after checking that it exists and that the submitted data is valid.
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LookupDto dto)
    {
        var existingGenre = await _context.Genres.FindAsync(id);
        if (existingGenre == null)
            return NotFound($"Genre with id={id} was not found.");

        if (await _context.Genres.AnyAsync(g => g.Name == dto.Name && g.Id != id))
            return BadRequest($"Another Genre with name '{dto.Name}' already exists.");

        existingGenre.Name = dto.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var genre = await _context.Genres.Include(g => g.Games).FirstOrDefaultAsync(g => g.Id == id);
        if (genre == null)
            return NotFound($"Genre with id={id} was not found.");

        if (genre.Games.Any())
            return BadRequest("Cannot delete this Genre because it is assigned to one or more games.");

        _context.Genres.Remove(genre);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
