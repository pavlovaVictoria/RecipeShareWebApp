using Microsoft.AspNetCore.Mvc;

namespace RecipeShare.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/500")]
        public IActionResult ServerError()
        {
            return View("ServerError");
        }

        // Handles other status codes like 404
        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            else if (statusCode == 403)
            {
                return View("Forbidden");
            }
            return View("GeneralError");
        }

        [Route("Error/General")]
        public IActionResult GeneralError()
        {
            return View("GeneralError");
        }
    }
}
