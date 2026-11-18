using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameMonitoring.Business.DTOs;
using GameMonitoring.Data.Models;

namespace WebAPI.Controllers;


/// Controller responsible for listing, viewing, creating, editing, and deleting game type records.

[ApiController]
[Route("api/[controller]")]
public class GameTypesController : ControllerBase
{
    private readonly GameMonitoringContext _context;
    private readonly IMapper _mapper;

    
    /// Initializes the GameTypesController and receives required services through dependency injection.
   
    public GameTypesController(GameMonitoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    /// Returns all records for this resource from the database/API.
   
    [HttpGet]
    public async Task<ActionResult<List<LookupDto>>> GetAll()
    {
        var gameTypes = await _context.GameTypes.OrderBy(gt => gt.Name).ToListAsync();
        return Ok(_mapper.Map<List<LookupDto>>(gameTypes));
    }

    
    /// Returns one record by ID, or a not-found response if the record does not exist.
   
    [HttpGet("{id}")]
    public async Task<ActionResult<LookupDto>> GetById(int id)
    {
        var gameType = await _context.GameTypes.FindAsync(id);
        if (gameType == null)
            return NotFound($"GameType with id={id} was not found.");
        return Ok(_mapper.Map<LookupDto>(gameType));
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    public async Task<IActionResult> Create(LookupDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest("Game type name is required.");

        if (await _context.GameTypes.AnyAsync(gt => gt.Name == dto.Name))
            return BadRequest($"GameType with name '{dto.Name}' already exists.");

        var gameType = new GameType { Name = dto.Name };
        _context.GameTypes.Add(gameType);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = gameType.Id }, _mapper.Map<LookupDto>(gameType));
    }

    
    /// Updates an existing record after checking that it exists and that the submitted data is valid.
   
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, LookupDto dto)
    {
        var existingGameType = await _context.GameTypes.FindAsync(id);
        if (existingGameType == null)
            return NotFound($"GameType with id={id} was not found.");

        if (await _context.GameTypes.AnyAsync(gt => gt.Name == dto.Name && gt.Id != id))
            return BadRequest($"Another GameType with name '{dto.Name}' already exists.");

        existingGameType.Name = dto.Name;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var gameType = await _context.GameTypes.FindAsync(id);
        if (gameType == null)
            return NotFound($"GameType with id={id} was not found.");

        if (await _context.Games.AnyAsync(g => g.GameTypeId == id))
            return BadRequest("Cannot delete this GameType because it is assigned to one or more games.");

        _context.GameTypes.Remove(gameType);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
