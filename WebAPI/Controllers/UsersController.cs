using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GameMonitoring.Business.DTOs;
using GameMonitoring.Data.Models;

namespace WebAPI.Controllers;


/// API controller responsible for retrieving users and updating user profile data.

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly GameMonitoringContext _context;
    private readonly IMapper _mapper;

    
    /// Initializes the UsersController and receives required services through dependency injection.
   
    public UsersController(GameMonitoringContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    
    /// Returns all records for this resource from the database/API.
   
    [HttpGet]
    public async Task<ActionResult<List<UserDto>>> GetAll()
    {
        var users = await _context.Users.OrderBy(u => u.Username).ToListAsync();
        return Ok(_mapper.Map<List<UserDto>>(users));
    }

    
    /// Returns one record by ID, or a not-found response if the record does not exist.
   
    [HttpGet("{id}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound($"User with id={id} was not found.");

        return Ok(_mapper.Map<UserDto>(user));
    }

    
    /// Returns the first user with the Admin role, mainly for admin profile support.
   
    [HttpGet("admin")]
    public async Task<ActionResult<UserDto>> GetFirstAdmin()
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Admin");
        if (user == null)
            return NotFound("Admin user was not found.");

        return Ok(_mapper.Map<UserDto>(user));
    }

    
    /// Updates profile fields for the logged-in user after checking that the request belongs to that same user.
   
    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateProfile(int id, UpdateUserProfileDto dto)
    {
        if (id != dto.Id)
            return BadRequest("Route id and user id do not match.");

        var user = await _context.Users.FindAsync(id);
        if (user == null)
            return NotFound($"User with id={id} was not found.");

        user.Email = dto.Email;
        user.FirstName = dto.FirstName;
        user.LastName = dto.LastName;
        user.PhoneNumber = dto.PhoneNumber;

        await _context.SaveChangesAsync();
        return NoContent();
    }
}
