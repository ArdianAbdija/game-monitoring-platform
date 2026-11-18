using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;


/// Controller responsible for listing, viewing, creating, editing, and deleting game type records.

public class GameTypesController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the GameTypesController and receives required services through dependency injection.
   
    public GameTypesController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Displays the main page for this controller by loading the required data and passing it to the view.
   
    public async Task<IActionResult> Index()
    {
        var gameTypes = await _api.GetAsync<List<LookupDto>>("gametypes") ?? new List<LookupDto>();
        return View(gameTypes.Select(gt => gt.ToViewModel()).ToList());
    }

    
    /// Displays the details page for a single record after loading it by ID.
   
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var gameType = await _api.GetAsync<LookupDto>($"gametypes/{id.Value}");
        if (gameType == null)
            return NotFound();

        return View(gameType.ToViewModel());
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    public IActionResult Create() => View();

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LookupViewModel gameType)
    {
        if (!ModelState.IsValid)
            return View(gameType);

        var result = await _api.PostAsync("gametypes", gameType.ToDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Game type could not be created.");
            return View(gameType);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var gameType = await _api.GetAsync<LookupDto>($"gametypes/{id.Value}");
        if (gameType == null)
            return NotFound();

        return View(gameType.ToViewModel());
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LookupViewModel gameType)
    {
        if (id != gameType.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(gameType);

        var result = await _api.PutAsync($"gametypes/{id}", gameType.ToDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Game type could not be updated.");
            return View(gameType);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var gameType = await _api.GetAsync<LookupDto>($"gametypes/{id.Value}");
        if (gameType == null)
            return NotFound();

        return View(gameType.ToViewModel());
    }

    
    /// Processes the confirmed delete form submission and removes the selected record through the API.
   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _api.DeleteAsync($"gametypes/{id}");
        if (!result.Success)
            TempData["ErrorMessage"] = result.Error;

        return RedirectToAction(nameof(Index));
    }
}
