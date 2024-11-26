using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeShare.Web.ViewModels.RecipeViewModels
{
    public class ProductsViewModel
    {
        public required Guid ProductId { get; set; }
        public required string ProductName { get; set; }
        public required int ProductType { get; set; }
    }
}
