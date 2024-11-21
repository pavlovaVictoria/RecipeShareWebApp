using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RecipeShare.Common.Enums;
using RecipeShare.Data;
using RecipeShare.Data.Models;
using System.Runtime.CompilerServices;

namespace RecipeShare.Services.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using IServiceScope scope = serviceProvider.CreateScope();
            RecipeShareDbContext context = scope.ServiceProvider.GetRequiredService<RecipeShareDbContext>();

            await context.Database.MigrateAsync();
            //Seed all default values
            await SeedAllergensAsync(context);
            await SeedAllCategoriesAsync(context);
            await SeedAllProductsAsync(context);
            await SeedDefaultUserAsync(scope.ServiceProvider);
            await SeedAllRecipesAsync(context, scope.ServiceProvider);
        }

        private static async Task SeedAllergensAsync(RecipeShareDbContext context)
        {
            if (context.Allergens.Any())
            {
                return;
            }
            string jsonFilePath = GetJsonFilePath("allergen.json");
            List<Allergen>? allergensData = JsonConvert.DeserializeObject<List<Allergen>>(await File.ReadAllTextAsync(jsonFilePath));
            if (allergensData != null)
            {
                allergensData = allergensData.OrderBy(x => x.AllergenName).ToList();
                await context.Allergens.AddRangeAsync(allergensData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAllCategoriesAsync(RecipeShareDbContext context)
        {
            if (context.Categories.Any())
            {
                return;
            }
            string jsonFilePath = GetJsonFilePath("category.json");
            List<Category>? categoriesData = JsonConvert.DeserializeObject<List<Category>>(await File.ReadAllTextAsync(jsonFilePath));

            if (categoriesData != null)
            {
                categoriesData = categoriesData.OrderBy(x => x.CategoryName).ToList();
                await context.Categories.AddRangeAsync(categoriesData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedAllProductsAsync(RecipeShareDbContext context)
        {
            if (context.Products.Any())
            {
                return;
            }
            string jsonFilePath = GetJsonFilePath("product.json");
            List<Product>? productsData = JsonConvert.DeserializeObject<List<Product>>(await File.ReadAllTextAsync(jsonFilePath));

            if (productsData != null)
            {
                productsData = productsData.OrderBy(x => x.ProductName).ToList();
                await context.Products.AddRangeAsync(productsData);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDefaultUserAsync(IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser? defaultUser = await userManager.FindByEmailAsync("defaultuser@example.com");
            if (defaultUser == null)
            {
                ApplicationUser user = new ApplicationUser
                {
                    UserName = "DefaultUser",
                    Email = "defaultuser@example.com",
                    IsMale = true,
                    AccountBio = "A default user"
                };

                IdentityResult result = await userManager.CreateAsync(user, "DefaultPassword123");
                if (!result.Succeeded)
                {
                    return;
                }
            }
            else
            {
                if (!await userManager.IsInRoleAsync(defaultUser, "User"))
                {
                    await userManager.AddToRoleAsync(defaultUser, "User");
                }
            }
        }

        private static async Task SeedAllRecipesAsync(RecipeShareDbContext context, IServiceProvider serviceProvider)
        {
            if (context.Recipes.Any())
            {
                return;
            }
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser? defaultUser = await userManager.FindByEmailAsync("defaultuser@example.com");
            if (defaultUser == null)
            {
                return;
            }
            List<Recipe> recipes = new List<Recipe>()
            {
                new Recipe
                {
                    RecipeTitle = "Classic Caesar Salad",
                    NormalizedRecipeTitle = "CLASSIC CAESAR SALAD",
                    UserId = defaultUser.Id,
                    Description = "A classic Caeser salad",
                    Preparation = "Prepare the chicken on the stove. Toss the lettuce with Caesar dressing, croutons, chichen, and Parmesan cheese. Serve chilled.",
                    MinutesForPrep = 10,
                    MealType = MealType.WithMeat,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Salad").Id,
                    Img = "https://assets.bonappetit.com/photos/624215f8a76f02a99b29518f/1:1/w_2800,h_2800,c_limit/0328-ceasar-salad-lede.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Chicken").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 50.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Lettuce").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 200.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Cheese").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 15.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p => p.ProductName == "Bread").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        }
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Wheat").Id
                        }
                    }
                },
                //not seeded
                new Recipe
                {
                    RecipeTitle = "Chicken Alfredo",
                    NormalizedRecipeTitle = "CHICKEN ALFREDO",
                    UserId = defaultUser.Id,
                    Description = "A creamy Alfredo pasta.",
                    Preparation = "Cook the pasta. Grill the chicken. Make the Alfredo sauce with cream and cheese on the stove. Mix the pasta, chicken and the Alfredo sauce. Serve hot.",
                    MinutesForPrep = 30,
                    MealType = MealType.WithMeat,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "MainCourse").Id,
                    Img = "https://www.budgetbytes.com/wp-content/uploads/2022/07/Chicken-Alfredo-above.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Chicken").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Heavy Cream").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 500.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Cheese").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p => p.ProductName == "Pasta").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 300.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p => p.ProductName == "Pasta").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 300.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p => p.ProductName == "Salt").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 10.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p => p.ProductName == "Black Pepper").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 5.00m
                        }
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Wheat").Id
                        },
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Milk").Id
                        }
                    }
                },
                new Recipe
                {
                    RecipeTitle = "Vegetable Stir-Fry",
                    NormalizedRecipeTitle = "VEGETABLE STIR-FRY",
                    UserId = defaultUser.Id,
                    Description = "A quick and healthy stir-fry",
                    Preparation = "Stir-fry the vegetables in a wok with soy sauce and serve with rice.",
                    MinutesForPrep = 25,
                    MealType = MealType.Vegan,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "MainCourse").Id,
                    Img = "https://natashaskitchen.com/wp-content/uploads/2020/08/Vegetable-Stir-Fry-2.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Rice").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Broccoli").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Bell Pepper").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Carrot").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 15.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Corn").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 15.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Mushroom").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 10.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Soy Sauce").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 15.00m
                        }
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Soya").Id
                        }
                    }
                },
            };
            if (recipes != null)
            {
                await context.Recipes.AddRangeAsync(recipes);
                await context.SaveChangesAsync();
            }
        }
        private static string GetJsonFilePath(string jsonName)
        {
            string solutionDir = Directory.GetCurrentDirectory();
            string wwwrootPath = Path.Combine(solutionDir, "..", "RecipeShare.Web", "wwwroot", "data", jsonName);
            string jsonFilePath = Path.GetFullPath(wwwrootPath);
            return jsonFilePath;
        }

        //For testing
        private static async Task AddRoleToUser(IServiceProvider serviceProvider)
        {
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            ApplicationUser? defaultUser = await userManager.FindByEmailAsync("vikiuchi83@gmail.com");
            if (defaultUser != null)
            {
                if (!await userManager.IsInRoleAsync(defaultUser, "User"))
                {
                    await userManager.AddToRoleAsync(defaultUser, "User");
                }
            }
        }

        //For testing
        private static async Task ClearSpecificTableAsync(RecipeShareDbContext context)
        {
            //context.Allergens.RemoveRange(context.Allergens);
            //await context.SaveChangesAsync();
            context.Recipes.RemoveRange(context.Recipes);
            await context.SaveChangesAsync();
            
            context.Products.RemoveRange(context.Products);
            await context.SaveChangesAsync();

            //context.Categories.RemoveRange(context.Categories);
            //await context.SaveChangesAsync();
        }
    }
}

    //new Recipe
    //{
    //    RecipeTitle = "Vegetable Stir-Fry",
    //    NormalizedRecipeTitle = "VEGETABLE STIR-FRY",
    //    UserId = defaultUser.Id,
    //    Description = "A quick and healthy stir-fry with mixed vegetables.",
    //    Preparation = "Stir-fry the vegetables in a wok with soy sauce and serve with rice."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Margarita Pizza",
    //    NormalizedRecipeTitle = "MARGARITA PIZZA",
    //    UserId = defaultUser.Id,
    //    Description = "A simple pizza with tomato sauce, fresh mozzarella, and basil.",
    //    Preparation = "Spread the tomato sauce on the dough, add mozzarella, and basil. Bake until golden."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Chocolate Chip Cookies",
    //    NormalizedRecipeTitle = "CHOCOLATE CHIP COOKIES",
    //    UserId = defaultUser.Id,
    //    Description = "Soft and chewy cookies with chocolate chips.",
    //    Preparation = "Mix the ingredients, form dough balls, bake at 350°F for 10 minutes."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Beef Tacos",
    //    NormalizedRecipeTitle = "BEEF TACOS",
    //    UserId = defaultUser.Id,
    //    Description = "Ground beef, seasoned with taco spices, served in soft tortillas.",
    //    Preparation = "Cook ground beef with taco seasoning. Fill tortillas with beef, lettuce, and cheese."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Lemon Meringue Pie",
    //    NormalizedRecipeTitle = "LEMON MERINGUE PIE",
    //    UserId = defaultUser.Id,
    //    Description = "A tangy lemon filling topped with fluffy meringue.",
    //    Preparation = "Prepare the lemon filling, pour into pie crust, and bake with meringue on top."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Spaghetti Bolognese",
    //    NormalizedRecipeTitle = "SPAGHETTI BOLOGNESE",
    //    UserId = defaultUser.Id,
    //    Description = "A rich meat sauce served over pasta.",
    //    Preparation = "Cook ground beef, onions, and tomatoes. Serve over spaghetti."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Chicken Caesar Wrap",
    //    NormalizedRecipeTitle = "CHICKEN CAESAR WRAP",
    //    UserId = defaultUser.Id,
    //    Description = "A wrap filled with grilled chicken, Caesar salad, and dressing.",
    //    Preparation = "Grill the chicken, toss with Caesar salad, and wrap in a tortilla."
    //},
    //new Recipe
    //{
    //    RecipeTitle = "Vegetarian Chili",
    //    NormalizedRecipeTitle = "VEGETARIAN CHILI",
    //    UserId = defaultUser.Id,
    //    Description = "A hearty chili made with beans, tomatoes, and vegetables.",
    //    Preparation = "Cook the beans, tomatoes, and vegetables with chili spices. Simmer for 30 minutes."
    //}
//};
