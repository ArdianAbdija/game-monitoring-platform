using GameMonitoring.Business.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using WebApp.Services;
using WebApp.ViewModels;

namespace WebApp.Controllers;


/// MVC controller used by normal users to browse games, view game details, and submit reviews with optional images.

public class BrowseController : Controller
{
    private readonly IGameMonitoringApiClient _api;

    
    /// Initializes the BrowseController and receives required services through dependency injection.
   
    public BrowseController(IGameMonitoringApiClient api)
    {
        _api = api;
    }

    
    /// Displays the main page for this controller by loading the required data and passing it to the view.
   
    public async Task<IActionResult> Index(string? search, int? gameTypeId, int page = 1)
    {
        const int pageSize = 6;
        var url = $"games/search?name={Uri.EscapeDataString(search ?? string.Empty)}&gameTypeId={gameTypeId}&page={page}&count={pageSize}";
        var result = await _api.GetAsync<PagedResultDto<GameDto>>(url) ?? new PagedResultDto<GameDto>();
        var gameTypes = await _api.GetAsync<List<LookupDto>>("gametypes") ?? new List<LookupDto>();

        ViewBag.Search = search;
        ViewBag.GameTypeId = gameTypeId;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = (int)Math.Ceiling(result.TotalItems / (double)pageSize);
        ViewData["GameTypes"] = new SelectList(gameTypes, "Id", "Name", gameTypeId);

        return View(result.Items.Select(g => g.ToViewModel()).ToList());
    }

    
    /// Displays the details page for a single record after loading it by ID.
   
    public async Task<IActionResult> Details(int id)
    {
        var game = await _api.GetAsync<GameDto>($"games/{id}");
        if (game == null)
            return NotFound();

        return View(game.ToViewModel());
    }

    
    /// Creates a review for the selected game and uploads any valid image files connected to that review.
   
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SubmitReview(int gameId, int rating, string? reviewText, List<IFormFile>? reviewImages)
    {
        int? userId = HttpContext.Session.GetInt32("UserId");

        if (userId == null)
        {
            TempData["ReviewError"] = "You must be logged in to submit a review.";
            return RedirectToAction("Login", "Account");
        }

        if (rating < 1 || rating > 10)
        {
            TempData["ReviewError"] = "Rating must be between 1 and 10.";
            return RedirectToAction("Details", new { id = gameId });
        }

        var createResult = await _api.PostAsync<CreateReviewDto, ReviewDto>("reviews", new CreateReviewDto
        {
            GameId = gameId,
            UserId = userId.Value,
            Rating = rating,
            ReviewText = reviewText
        });

        if (!createResult.Success || createResult.Data == null)
        {
            TempData["ReviewError"] = createResult.Error ?? "Review could not be submitted.";
            return RedirectToAction("Details", new { id = gameId });
        }

        if (reviewImages != null && reviewImages.Any())
        {
            string uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "reviews");
            Directory.CreateDirectory(uploadFolder);

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

            foreach (var image in reviewImages.Where(image => image.Length > 0))
            {
                var extension = Path.GetExtension(image.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(extension))
                    continue;

                string fileName = Guid.NewGuid() + extension;
                string filePath = Path.Combine(uploadFolder, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                    await image.CopyToAsync(stream);

                await _api.PostAsync($"reviews/{createResult.Data.Id}/images", new CreateReviewImageDto
                {
                    ReviewId = createResult.Data.Id,
                    Url = "/uploads/reviews/" + fileName
                });
            }
        }

        TempData["ReviewSuccess"] = "Your review has been submitted and is waiting for approval.";
        return RedirectToAction("Details", new { id = gameId });
    }
}
