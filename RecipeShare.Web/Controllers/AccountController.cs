using Microsoft.AspNetCore.Mvc;
using RecipeShare.Services.Data.Interfaces;

namespace RecipeShare.Web.Controllers
{
	public class AccountController : Controller
	{
        private readonly IAccountService accountService;

        public AccountController(IAccountService _accountService)
        {
            accountService = _accountService;
        }


    }
}
