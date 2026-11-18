using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApp.DTOs;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;


/// MVC controller that lets a normal logged-in user view and update their own profile information.

public class UserProfileController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the UserProfileController and receives required services through dependency injection.
   
    public UserProfileController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Displays the main page for this controller by loading the required data and passing it to the view.
   
    public async Task<IActionResult> Index()
    {
        var userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
            return RedirectToAction("Login", "Account");

        var user = await _api.GetAsync<UserDto>($"users/{userId.Value}");
        if (user == null)
            return NotFound();

        return View(user.ToViewModel());
    }

    
    /// Updates profile fields for the logged-in user after checking that the request belongs to that same user.
   
    [HttpPost]
    public async Task<IActionResult> UpdateProfile([FromBody] ProfileDto dto)
    {
        var sessionUserId = HttpContext.Session.GetInt32("UserId");

        if (sessionUserId == null || dto.Id != sessionUserId.Value)
            return Unauthorized();

        var result = await _api.PutAsync($"users/{dto.Id}/profile", new UpdateUserProfileDto
        {
            Id = dto.Id,
            Email = dto.Email,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PhoneNumber = dto.PhoneNumber
        });

        if (!result.Success)
            return BadRequest(result.Error);

        return Ok(new { success = true, message = "Profile updated successfully." });
    }
}
