using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameMonitoring.Business.DTOs;
using GameMonitoring.Data.Models;

namespace WebAPI.Controllers;


/// API controller responsible for returning, searching, creating, updating, deleting, and genre-linking game records.

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly GameMonitoringContext _context;
    private readonly IMapper _mapper;

    
    /// Initializes the GamesController and receives required services through dependency injection.
   
    public GamesController(GameMonitoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    /// Returns all records for this resource from the database/API.
   
    [HttpGet]
    public async Task<ActionResult<List<GameDto>>> GetAll()
    {
        var games = await BaseGameQuery().OrderBy(g => g.Name).ToListAsync();
        return Ok(_mapper.Map<List<GameDto>>(games));
    }

    
    /// Returns one record by ID, or a not-found response if the record does not exist.
   
    [HttpGet("{id}")]
    public async Task<ActionResult<GameDto>> GetById(int id)
    {
        var game = await BaseGameQuery().FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
            return NotFound($"Game with id={id} was not found.");

        return Ok(_mapper.Map<GameDto>(game));
    }

    
    /// Filters and paginates games by optional name and game type values.
   
    [HttpGet("search")]
    public async Task<ActionResult<PagedResultDto<GameDto>>> Search(string? name, int? gameTypeId, int page = 1, int count = 10)
    {
        if (page < 1 || count < 1)
            return BadRequest("Page and count must be greater than 0.");

        var query = BaseGameQuery();

        if (!string.IsNullOrWhiteSpace(name))
            query = query.Where(g => g.Name.Contains(name));

        if (gameTypeId.HasValue)
            query = query.Where(g => g.GameTypeId == gameTypeId.Value);

        var totalItems = await query.CountAsync();
        var games = await query
            .OrderBy(g => g.Name)
            .Skip((page - 1) * count)
            .Take(count)
            .ToListAsync();

        return Ok(new PagedResultDto<GameDto>
        {
            TotalItems = totalItems,
            Page = page,
            Count = count,
            Items = _mapper.Map<List<GameDto>>(games)
        });
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    public async Task<IActionResult> Create(CreateGameDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (!await _context.GameTypes.AnyAsync(gt => gt.Id == dto.GameTypeId))
            return BadRequest($"GameType with id={dto.GameTypeId} does not exist.");

        if (await _context.Games.AnyAsync(g => g.Name == dto.Name))
            return BadRequest($"Game with name '{dto.Name}' already exists.");

        var game = _mapper.Map<Game>(dto);
        _context.Games.Add(game);
        await _context.SaveChangesAsync();
        await AddLog("Information", $"Game with id={game.Id} has been created.");

        var created = await BaseGameQuery().FirstAsync(g => g.Id == game.Id);
        return CreatedAtAction(nameof(GetById), new { id = game.Id }, _mapper.Map<GameDto>(created));
    }

    
    /// Updates an existing record after checking that it exists and that the submitted data is valid.
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, CreateGameDto dto)
    {
        var existingGame = await _context.Games.FindAsync(id);

        if (existingGame == null)
            return NotFound($"Game with id={id} was not found.");

        if (!await _context.GameTypes.AnyAsync(gt => gt.Id == dto.GameTypeId))
            return BadRequest($"GameType with id={dto.GameTypeId} does not exist.");

        if (await _context.Games.AnyAsync(g => g.Name == dto.Name && g.Id != id))
            return BadRequest($"Another game with name '{dto.Name}' already exists.");

        existingGame.Name = dto.Name;
        existingGame.Description = dto.Description;
        existingGame.ReleaseDate = dto.ReleaseDate;
        existingGame.Developer = dto.Developer;
        existingGame.GameTypeId = dto.GameTypeId;

        await _context.SaveChangesAsync();
        await AddLog("Information", $"Game with id={id} has been updated.");

        return NoContent();
    }

    
    /// Replaces the genres currently connected to a game with the genre IDs submitted by the client.
   
    [HttpPut("{id}/genres")]
    public async Task<IActionResult> AssignGenres(int id, AssignGenresDto dto)
    {
        var game = await _context.Games.Include(g => g.Genres).FirstOrDefaultAsync(g => g.Id == id);

        if (game == null)
            return NotFound($"Game with id={id} was not found.");

        if (dto.GenreIds == null || !dto.GenreIds.Any())
            return BadRequest("At least one genre id must be provided.");

        var genres = await _context.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();

        if (genres.Count != dto.GenreIds.Distinct().Count())
            return BadRequest("One or more genre ids are invalid.");

        game.Genres.Clear();
        foreach (var genre in genres)
            game.Genres.Add(genre);

        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Deletes the selected game record from the database and writes a log entry for the deletion.
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
            return NotFound($"Game with id={id} was not found.");

        _context.Games.Remove(game);
        await _context.SaveChangesAsync();
        await AddLog("Information", $"Game with id={id} has been deleted.");

        return NoContent();
    }

    
    /// Builds the reusable game query with related game type, genres, images, and reviews already included.
   
    private IQueryable<Game> BaseGameQuery()
    {
        return _context.Games
            .Include(g => g.GameType)
            .Include(g => g.Genres)
            .Include(g => g.Images)
            .Include(g => g.Reviews)
                .ThenInclude(r => r.User)
            .Include(g => g.Reviews)
                .ThenInclude(r => r.ReviewImages)
            .AsQueryable();
    }

    
    /// Creates a database log entry so important game changes can be tracked.
   
    private async Task AddLog(string level, string message)
    {
        _context.Logs.Add(new Log
        {
            Timestamp = DateTime.Now,
            Level = level,
            Message = message
        });
        await _context.SaveChangesAsync();
    }
}
