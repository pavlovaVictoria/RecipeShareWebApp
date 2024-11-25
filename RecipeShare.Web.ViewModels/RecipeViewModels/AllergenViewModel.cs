using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class AllergenViewModel
    {
        public required string AllergenName { get; set; }
        public required string AllergenImage { get; set; }
    }
}
