using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Data.Models;

namespace RecipeShare.Data
{
    public class RecipeShareDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public RecipeShareDbContext()
        {

        }
        public RecipeShareDbContext(DbContextOptions<RecipeShareDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Allergen> Allergens { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<LikedRecipe> LikedRecipes { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeAllergen> RecipesAllergens { get; set; }
        public virtual DbSet<RecipeProductDetails> RecipesProductsDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityRole<Guid>>().HasData(
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "User", NormalizedName = "USER"},
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Moderator", NormalizedName = "MODERATOR"},
                new IdentityRole<Guid> { Id = Guid.NewGuid(), Name = "Administrator", NormalizedName = "ADMINISTRATOR"}
            );

            //Composite primary keys
            builder.Entity<LikedRecipe>().HasKey(x => new
            {
                x.UserId,
                x.RecipeId
            });

            builder.Entity<RecipeAllergen>().HasKey(x => new
            {
                x.RecipeId,
                x.AllergenId
            });

            builder.Entity<RecipeProductDetails>().HasKey(x => new
            {
                x.RecipeId,
                x.ProductId
            });

            //Delete Behaviour
            builder.Entity<Comment>()
                .HasOne(c => c.Recipe)
                .WithMany(r => r.Comments)
                .HasForeignKey(c => c.RecipeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Comment>()
                .HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Recipe>()
                .HasOne(r => r.User)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Recipe>()
                .HasOne(r => r.Category)
                .WithMany(c => c.Recipes)
                .HasForeignKey(r => r.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
