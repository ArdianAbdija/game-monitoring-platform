using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WebApp.DTOs;
using WebApp.Services;

namespace WebApp.Controllers;


/// MVC controller responsible for user registration, login, logout, and storing login information in the session.

public class AccountController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the AccountController and receives required services through dependency injection.
   
    public AccountController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Handles account registration by validating the submitted data, hashing the password, and creating a new user account.
   
    [HttpGet]
    public IActionResult Register() => View();

    
    /// Handles account registration by validating the submitted data, hashing the password, and creating a new user account.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(MvcRegisterDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            ModelState.AddModelError("Username", "Username is required.");

        if (string.IsNullOrWhiteSpace(dto.Email))
            ModelState.AddModelError("Email", "Email is required.");

        if (string.IsNullOrWhiteSpace(dto.Password))
            ModelState.AddModelError("Password", "Password is required.");

        if (dto.Password != dto.RepeatPassword)
            ModelState.AddModelError("RepeatPassword", "Passwords do not match.");

        if (!ModelState.IsValid)
            return View(dto);

        var result = await _api.PostAsync("auth/register", new RegisterDto
        {
            Username = dto.Username,
            Email = dto.Email,
            Password = dto.Password
        });

        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Registration failed.");
            return View(dto);
        }

        TempData["SuccessMessage"] = "Registration successful. Please log in.";
        return RedirectToAction(nameof(Login));
    }

    
    /// Handles login by validating credentials, creating a JWT token through the API, and storing user information in the MVC session.
   
    [HttpGet]
    public IActionResult Login() => View();

    
    /// Handles login by validating credentials, creating a JWT token through the API, and storing user information in the MVC session.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(MvcLoginDto dto)
    {
        var result = await _api.PostAsync<LoginDto, LoginResponseDto>("auth/login", new LoginDto
        {
            Username = dto.Username,
            Password = dto.Password
        });

        if (!result.Success || result.Data == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid username or password.");
            return View(dto);
        }

        HttpContext.Session.SetInt32("UserId", result.Data.UserId);
        HttpContext.Session.SetString("Username", result.Data.Username);
        HttpContext.Session.SetString("Role", result.Data.Role);
        HttpContext.Session.SetString("Token", result.Data.Token);

        if (result.Data.Role == "Admin")
            return RedirectToAction("Index", "Games");

        return RedirectToAction("Index", "Browse");
    }

    
    /// Clears the current session and redirects the user back to the login page.
   
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Index", "Home");
    }
}
