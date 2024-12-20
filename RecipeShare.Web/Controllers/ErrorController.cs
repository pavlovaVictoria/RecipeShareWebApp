﻿using Microsoft.AspNetCore.Mvc;

namespace RecipeShare.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("Error/500")]
        public IActionResult ServerError()
        {
            return View("ServerError");
        }

        [Route("Error/{statusCode}")]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            Response.StatusCode = statusCode;
            if (statusCode == 404)
            {
                return View("NotFound");
            }
            else if (statusCode == 403)
            {
                return View("Forbidden");
            }
            else
            {
                return View("ServerError");
            }
        }
    }
}
