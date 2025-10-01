using Microsoft.EntityFrameworkCore;
using RecipeShare.Api.Models;

namespace RecipeShare.Api.Data
{
    public class RecipeDbContext : DbContext
    {
        public RecipeDbContext(DbContextOptions<RecipeDbContext> options) : base(options) { }

        public DbSet<Recipe> Recipes => Set<Recipe>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipe>().HasData(
                new Recipe
                {
                    Id = 1,
                    Title = "Simple Tomato Pasta",
                    Ingredients = "pasta, tomato sauce, garlic, olive oil, salt",
                    Steps = "Boil pasta. Heat sauce. Combine and serve.",
                    CookingTimeMinutes = 20,
                    DietaryTags = "vegetarian"
                },
                new Recipe
                {
                    Id = 2,
                    Title = "Greek Salad",
                    Ingredients = "cucumber, tomato, red onion, feta, olive oil, lemon",
                    Steps = "Chop ingredients, toss with dressing.",
                    CookingTimeMinutes = 10,
                    DietaryTags = "vegetarian,gluten-free"
                },
                new Recipe
                {
                    Id = 3,
                    Title = "Avocado Toast",
                    Ingredients = "bread, avocado, lemon, salt, pepper",
                    Steps = "Toast bread, mash avocado, season, spread on toast.",
                    CookingTimeMinutes = 5,
                    DietaryTags = "vegetarian"
                }
            );
        }
    }
}