using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Web.Controllers
{
    [Authorize]
    public class RecipeController : Controller
    {
        private readonly IRecipeService recipeService;

        public RecipeController(IRecipeService _recipeService)
        {
            recipeService = _recipeService;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            RecipesIndexViewModel model = await recipeService.IndexPageOfRecipes();
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> AllRecipesByCategory(Guid categoryId)
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RecipeDetails(Guid recipeId)
        {
            return View();
        }
    }
}
