using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;


/// Controller responsible for listing, creating, editing, deleting, approving, and rejecting game reviews.

public class ReviewsController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the ReviewsController and receives required services through dependency injection.
   
    public ReviewsController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Displays the main page for this controller by loading the required data and passing it to the view.
   
    public async Task<IActionResult> Index()
    {
        var reviews = await _api.GetAsync<List<ReviewDto>>("reviews") ?? new List<ReviewDto>();
        return View(reviews.Select(r => r.ToViewModel()).ToList());
    }

    
    /// Displays the details page for a single record after loading it by ID.
   
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
            return NotFound();

        var review = await _api.GetAsync<ReviewDto>($"reviews/{id.Value}");
        if (review == null)
            return NotFound();

        return View(review.ToViewModel());
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    public async Task<IActionResult> Create()
    {
        await PopulateDropdowns();
        return View();
    }

    
    /// Handles creation of a new record after validating the submitted data.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ReviewViewModel review)
    {
        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(review.GameId, review.UserId);
            return View(review);
        }

        var result = await _api.PostAsync("reviews", review.ToCreateDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Review could not be created.");
            await PopulateDropdowns(review.GameId, review.UserId);
            return View(review);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
            return NotFound();

        var review = await _api.GetAsync<ReviewDto>($"reviews/{id.Value}");
        if (review == null)
            return NotFound();

        await PopulateDropdowns(review.GameId, review.UserId);
        return View(review.ToViewModel());
    }

    
    /// Shows the edit form or saves the submitted edit form depending on whether the request is GET or POST.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ReviewViewModel review)
    {
        if (id != review.Id)
            return NotFound();

        if (!ModelState.IsValid)
        {
            await PopulateDropdowns(review.GameId, review.UserId);
            return View(review);
        }

        var result = await _api.PutAsync($"reviews/{id}", review.ToCreateDto());
        if (!result.Success)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Review could not be updated.");
            await PopulateDropdowns(review.GameId, review.UserId);
            return View(review);
        }

        return RedirectToAction(nameof(Index));
    }

    
    /// Loads a record for deletion confirmation or deletes it through the API when called from the API controller.
   
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
            return NotFound();

        var review = await _api.GetAsync<ReviewDto>($"reviews/{id.Value}");
        if (review == null)
            return NotFound();

        return View(review.ToViewModel());
    }

    
    /// Processes the confirmed delete form submission and removes the selected record through the API.
   
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var result = await _api.DeleteAsync($"reviews/{id}");
        if (!result.Success)
            TempData["ErrorMessage"] = result.Error;

        return RedirectToAction(nameof(Index));
    }

    
    /// Marks the selected review as approved so it can be shown publicly.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id)
    {
        await _api.PutAsync($"reviews/{id}/approve", new { });
        return RedirectToAction(nameof(Index));
    }

    
    /// Marks the selected review as not approved so it is hidden from public display.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id)
    {
        await _api.PutAsync($"reviews/{id}/reject", new { });
        return RedirectToAction(nameof(Index));
    }

    
    /// Loads games and users and stores them in ViewData so the review form dropdowns can be rendered.
   
    private async Task PopulateDropdowns(int? selectedGameId = null, int? selectedUserId = null)
    {
        var games = await _api.GetAsync<List<GameDto>>("games") ?? new List<GameDto>();
        var users = await _api.GetAsync<List<UserDto>>("users") ?? new List<UserDto>();

        ViewData["GameId"] = new SelectList(games, "Id", "Name", selectedGameId);
        ViewData["UserId"] = new SelectList(users, "Id", "Username", selectedUserId);
    }
}
