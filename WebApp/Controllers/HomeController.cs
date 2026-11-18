using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using WebApp.Models;

namespace WebApp.Controllers
{
    
    /// MVC controller responsible for basic static pages and the default error page.
   
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        
        /// Initializes the HomeController and receives required services through dependency injection.
       
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        
        /// Displays the main page for this controller by loading the required data and passing it to the view.
       
        public IActionResult Index()
        {
            return View();
        }

        
        /// Displays the privacy page.
       
        public IActionResult Privacy()
        {
            return View();
        }

        
        /// Displays the error page with the current request ID for debugging support.
       
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
