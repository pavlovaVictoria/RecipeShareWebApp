using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;
using RecipeShare.Web.ViewModels.PaginationViewModels;
using RecipeShare.Web.ViewModels.RecipeViewModels;

namespace RecipeShare.Web.Areas.Moderator.Controllers
{
    [Authorize(Policy = "CanApproveRecipes")]
    public class ModeratorController : Controller
    {
        private readonly IModeratorService moderatorService;
        public ModeratorController(IModeratorService _moderatorService)
        {
            moderatorService = _moderatorService;
        }
        public async Task<IActionResult> Index(int page = 1, int pageSize = 4)
        {
            PaginatedList<InfoRecipeViewModel> recipes = await moderatorService.ViewAllUnapprovedRecipes(page, pageSize);
            return View(recipes);
        }
    }
}
