using Microsoft.AspNetCore.Mvc;

namespace RecipeShare.Web.Controllers
{
    public class CommentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
