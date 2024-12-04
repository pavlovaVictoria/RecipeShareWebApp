using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;

namespace RecipeShare.Web.Areas.Administrator.Controllers
{
    [Authorize(Policy = "CanManageEverything")]
    public class AdministratorController : Controller
    {
        private readonly IAdministratorService administratorService;
        public AdministratorController(IAdministratorService _administratorService)
        {
            administratorService = _administratorService;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
