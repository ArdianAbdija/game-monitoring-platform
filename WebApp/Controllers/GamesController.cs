using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;


/// Controller responsible for game browsing/admin CRUD operations and communication with the API.

public class GamesController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the GamesController and receives required services through dependency injection.
   
    public GamesController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Displays the main page for this controller by loading the required data and passing it to the view.
   
    public async Task<IActionResult> Index(string? search, int? gameTypeId, int page = 1)
    {
        const int pageSize = 10;
        var url = $"games/search?name={Uri.EscapeDataString(search ?? string.Empty)}&gameTypeId={gameTypeId}&page={page}&count={pageSize}";
        var result = await _api.GetAsync<PagedResultDto<GameDto>>(url) ?? new PagedResultDto<GameDto>();
        var gameTypes = await _api.GetAsync<List<LookupDto>>("gametypes") ?? new List<LookupDto>();

        ViewBag.Search = search;
        ViewBag.GameTypeId = gameTypeId;
        ViewBag.Page = page;
        ViewBag.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);
        ViewData["GameTypeId"] = new SelectList(gameTypes, "Id", "Name", gameTypeId);

        return View(result.Items.Select(g => g.ToViewModel()).ToList());
    }

    
    /// Displays the details page for a single record after loading it by ID.
   
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var game = await _api.GetAsync<GameDto>($"games/{id.Value}");
        if (game == null)
            return NotFound();

        return View(game.ToViewModel());
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    public async Task<IActionResult> Create()
    {
        await PopulateGameTypes();
        return View();
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(GameViewModel game)
    {
        if (!ModelState.IsValid)
        {
            await PopulateGameTypes(game.GameTypeId);
            return View(game);
        }

        var result = await _api.PostAsync("games", game.ToCreateDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Game could not be created.");
            await PopulateGameTypes(game.GameTypeId);
            return View(game);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var game = await _api.GetAsync<GameDto>($"games/{id.Value}");
        if (game == null)
            return NotFound();

        await PopulateGameTypes(game.GameTypeId);
        return View(game.ToViewModel());
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, GameViewModel game)
    {
        if (id != game.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateGameTypes(game.GameTypeId);
            return View(game);
        }

        var result = await _api.PutAsync($"games/{id}", game.ToCreateDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Game could not be updated.");
            await PopulateGameTypes(game.GameTypeId);
            return View(game);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var game = await _api.GetAsync<GameDto>($"games/{id.Value}");
        if (game == null)
            return NotFound();

        return View(game.ToViewModel());
    }

    
    /// Processes the confirmed delete form submission and removes the selected record through the API.
   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _api.DeleteAsync($"games/{id}");
        if (!result.Success)
            TempData["ErrorMessage"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    
    /// Loads game type lookup values and stores them in ViewData so the game form dropdown can be rendered.
   
    private async Task PopulateGameTypes(int? selectedId = null)
    {
        var gameTypes = await _api.GetAsync<List<LookupDto>>("gametypes") ?? new List<LookupDto>();
        ViewData["GameTypeId"] = new SelectList(gameTypes, "Id", "Name", selectedId);
    }
}
