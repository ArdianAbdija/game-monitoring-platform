using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;


/// Controller responsible for listing, viewing, creating, editing, and deleting genre records.

public class GenresController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the GenresController and receives required services through dependency injection.
   
    public GenresController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Displays the main page for this controller by loading the required data and passing it to the view.
   
    public async Task<IActionResult> Index()
    {
        var genres = await _api.GetAsync<List<LookupDto>>("genres") ?? new List<LookupDto>();
        return View(genres.Select(g => g.ToViewModel()).ToList());
    }

    
    /// Displays the details page for a single record after loading it by ID.
   
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var genre = await _api.GetAsync<LookupDto>($"genres/{id.Value}");
        if (genre == null)
            return NotFound();

        return View(genre.ToViewModel());
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    public IActionResult Create() => View();

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LookupViewModel genre)
    {
        if (!ModelState.IsValid)
            return View(genre);

        var result = await _api.PostAsync("genres", genre.ToDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Genre could not be created.");
            return View(genre);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var genre = await _api.GetAsync<LookupDto>($"genres/{id.Value}");
        if (genre == null)
            return NotFound();

        return View(genre.ToViewModel());
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LookupViewModel genre)
    {
        if (id != genre.Id)
            return NotFound();

        if (!ModelState.IsValid)
            return View(genre);

        var result = await _api.PutAsync($"genres/{id}", genre.ToDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Genre could not be updated.");
            return View(genre);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var genre = await _api.GetAsync<LookupDto>($"genres/{id.Value}");
        if (genre == null)
            return NotFound();

        return View(genre.ToViewModel());
    }

    
    /// Processes the confirmed delete form submission and removes the selected record through the API.
   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _api.DeleteAsync($"genres/{id}");
        if (!result.Success)
            TempData["ErrorMessage"] = result.Error;

        return RedirectToAction(nameof(Index));
    }
}
