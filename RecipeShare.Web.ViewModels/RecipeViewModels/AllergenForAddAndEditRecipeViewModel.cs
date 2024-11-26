using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class AllergenForAddAndEditRecipeViewModel
    {
        public required Guid AllergenId { get; set; }
        public string? AllergenName { get; set; }
    }
}
