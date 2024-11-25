using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;
using System.Diagnostics;

namespace RecipeShare.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHomeService homeService;

        public HomeController(ILogger<HomeController> logger, IHomeService _homeService)
        {
            _logger = logger;
            homeService = _homeService;
        }

        public async Task<IActionResult> Index()
        {
            List<InfoRecipeViewModel> topRecipes = await homeService.Top3Recipes();
            return View(topRecipes);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
