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
                    IsApproved = true,
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
                new Recipe
                {
                    IsApproved = true,
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
                    IsApproved = true,
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
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Fresh Bruschetta",
                    NormalizedRecipeTitle = "FRESH BRUSCHETTA",
                    UserId = defaultUser.Id,
                    Description = "Tomato and Basil Bruschetta",
                    Preparation = "Toast the bread, mix the tomato topping, and spoon it over the bread. Optionally, top with cheese and serve.",
                    MinutesForPrep = 15,
                    MealType = MealType.Vegetarian,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Appetizer").Id,
                    Img = "https://www.recipetineats.com/tachyon/2018/12/Bruschetta_2a.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Bread").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Tomato").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 50.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Basil").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 10.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Cheese").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 30.00m
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
                    IsApproved = true,
                    RecipeTitle = "Lemon Meringue Pie",
                    NormalizedRecipeTitle = "LEMON MERINGUE PIE",
                    UserId = defaultUser.Id,
                    Description = "A lemon pie",
                    Preparation = "Prepare the lemon filling, pour into pie crust, and bake with meringue on top.",
                    MinutesForPrep = 90,
                    MealType = MealType.Vegetarian,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Dessert").Id,
                    Img = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSi6WrcVGMxkO8Bms3S-4PFp8mvjavv8iXgFw&s",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Chicken Egg").Id,
                            UnitType = UnitType.Count,
                            Quantity = 6m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Lemon").Id,
                            UnitType = UnitType.Count,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Butter").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Wheat Flour").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "White Sugar").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 120.00m
                        },
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
                        },
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Egg").Id
                        }
                    }
                },
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Sweet Greek Yogurt",
                    NormalizedRecipeTitle = "SWEET GREEK YOGURT",
                    UserId = defaultUser.Id,
                    Description = "Honey and Almonds Yogurt",
                    Preparation = "Scoop yogurt into a bowl, drizzle with honey, top with sliced almonds, and sprinkle cinnamon if desired.",
                    MinutesForPrep = 5,
                    MealType = MealType.Vegetarian,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Snack").Id,
                    Img = "https://www.modernhoney.com/wp-content/uploads/2016/10/IMG_1210edit-copycrop.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Yogurt").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 30.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Honey").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 10.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Almond").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 10.00m
                        },
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Milk").Id
                        },
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Nut").Id
                        },
                    }
                },
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Pizza Dough",
                    NormalizedRecipeTitle = "PIZZA DOUGH",
                    UserId = defaultUser.Id,
                    Description = "Perfect pizza dough",
                    Preparation = "In a bowl, mix water with olive oil, sugar and yeast and wait 10 minutes. Then we add the salt and flour and knead until it becomes a dough.",
                    MinutesForPrep = 5,
                    MealType = MealType.Vegan,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Dough").Id,
                    Img = "https://www.foodandwine.com/thmb/S4Xys0wnd7Xu9D-41jDVn1b9Iqc=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/homemade-pizza-dough-FT-RECIPE0422-7d3aa2fbd4244e88afaff987753866d6.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Wheat Flour").Id,
                            UnitType = UnitType.Cups,
                            Quantity = 3m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Olive Oil").Id,
                            UnitType = UnitType.Spoons,
                            Quantity = 3m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Water").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 200.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "White Sugar").Id,
                            UnitType = UnitType.TeaSpoon,
                            Quantity = 1m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Salt").Id,
                            UnitType = UnitType.TeaSpoon,
                            Quantity = 1m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Yeast").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 9m
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
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Tomato Soup",
                    NormalizedRecipeTitle = "TOMATO SOUP",
                    UserId = defaultUser.Id,
                    Description = "Classic creamy tomato soup",
                    Preparation = "Bake onions, garlic, tomatoes and bell peppers in a pan. Then blend them and serve the soup with cream.",
                    MinutesForPrep = 5,
                    MealType = MealType.Vegetarian,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Snack").Id,
                    Img = "https://www.modernhoney.com/wp-content/uploads/2016/10/IMG_1210edit-copycrop.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Tomato").Id,
                            UnitType = UnitType.Count,
                            Quantity = 4m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Garlic").Id,
                            UnitType = UnitType.Count,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Onion").Id,
                            UnitType = UnitType.Count,
                            Quantity = 1m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Bell Pepper").Id,
                            UnitType = UnitType.Count,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Sour Cream").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 20.00m
                        },
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Milk").Id
                        }
                    }
                },
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Sangria",
                    NormalizedRecipeTitle = "SANGRIA",
                    UserId = defaultUser.Id,
                    Description = "Spanish drink",
                    Preparation = "Mix the wine, the orange juice and the chopped fruit.",
                    MinutesForPrep = 10,
                    MealType = MealType.Vegan,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Drink").Id,
                    Img = "https://whatmollymade.com/wp-content/uploads/2022/02/winter-sangria-18.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Cherry").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Apple").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Grapes").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 20.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Lime").Id,
                            UnitType = UnitType.Count,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Orange Juice").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 300.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Sparkling Water").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 500.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Red Wine").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 200.00m
                        },
                    },
                },
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Chocolate Cookies",
                    NormalizedRecipeTitle = "CHOCOLATE COOKIES",
                    UserId = defaultUser.Id,
                    Description = "Soft and chewy cookies",
                    Preparation = "Mix the ingredients, form dough balls and bake at 170C for 10 minutes.",
                    MinutesForPrep = 30,
                    MealType = MealType.Vegetarian,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Dessert").Id,
                    Img = "https://assets.bonappetit.com/photos/5ca534485e96521ff23b382b/1:1/w_2560%2Cc_limit/chocolate-chip-cookie.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Chicken Egg").Id,
                            UnitType = UnitType.Count,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Butter").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 200.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Baking Soda").Id,
                            UnitType = UnitType.TeaSpoon,
                            Quantity = 1m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Chocolate Chips").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Wheat Flour").Id,
                            UnitType = UnitType.Cups,
                            Quantity = 2m
                        },
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Wheat").Id
                        }
                    }
                },
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Shrimp pasta",
                    NormalizedRecipeTitle = "SHRIMP PASTA",
                    UserId = defaultUser.Id,
                    Description = "Creamy shrimp pasta",
                    Preparation = "The shrimps are heated in a pan with garlic, then cream and cherry tomatoes are added. After the pasta is cooked, mix it with the sauce.",
                    MinutesForPrep = 30,
                    MealType = MealType.WithMeat,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "MainCourse").Id,
                    Img = "https://newmansown.com/wp-content/uploads/2022/03/shrimp-marinara-with-pasta.png",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Garlic").Id,
                            UnitType = UnitType.Count,
                            Quantity = 1m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Tomato").Id,
                            UnitType = UnitType.Count,
                            Quantity = 1m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Cheese").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 15.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Cream").Id,
                            UnitType = UnitType.Milliliters,
                            Quantity = 200.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Shrimp").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 100.00m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Pasta").Id,
                            UnitType = UnitType.Grams,
                            Quantity = 500.00m
                        }
                    },
                    AllergensRecipes =
                    {
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Milk").Id
                        },
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Egg").Id
                        },
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Wheat").Id
                        },
                        new RecipeAllergen
                        {
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Crab").Id
                        }
                    }
                },
                new Recipe
                {
                    IsApproved = true,
                    RecipeTitle = "Pasta Dough",
                    NormalizedRecipeTitle = "PASTA DOUGH",
                    UserId = defaultUser.Id,
                    Description = "Fresh pasta dough",
                    Preparation = "We make the dough by mixing flour, eggs and salt.",
                    MinutesForPrep = 20,
                    MealType = MealType.Vegetarian,
                    CategoryId = context.Categories.First(c=> c.CategoryName == "Dough").Id,
                    Img = "https://www.foodandwine.com/thmb/71SG19or6KD4Cu_O_3rDjw01dT4=/1500x0/filters:no_upscale():max_bytes(150000):strip_icc()/fresh-pasta-dough-FT-RECIPE0921-5ae4cdc40d3f44ada10db2b2a5fd7de3.jpg",
                    DateOfRelease = DateTime.UtcNow,
                    RecipesProductsDetails =
                    {
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Wheat Flour").Id,
                            UnitType = UnitType.Cups,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Chicken Egg").Id,
                            UnitType = UnitType.Count,
                            Quantity = 2m
                        },
                        new RecipeProductDetails
                        {
                            ProductId = context.Products.First(p=> p.ProductName == "Salt").Id,
                            UnitType = UnitType.TeaSpoon,
                            Quantity = 0.5m
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
                            AllergenId = context.Allergens.First(a => a.AllergenName == "Egg").Id
                        }
                    }
                }
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