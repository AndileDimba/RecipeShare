using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RecipeShare.Api.Data;
using RecipeShare.Api.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RecipeShare.Tests
{
    public class RecipesApiTests : IDisposable
    {
        private readonly SqliteConnection _connection;

        public RecipesApiTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
            _connection.Open();
        }

        public void Dispose()
        {
            _connection?.Close();
            _connection?.Dispose();
        }

        [Fact]
        public async Task GetAll_ReturnsSeededRecipes()
        {
            using var client = await CreateTestClientAsync();

            var resp = await client.GetAsync("/api/recipes");
            resp.EnsureSuccessStatusCode();
            var recipes = await resp.Content.ReadFromJsonAsync<List<Recipe>>();
            recipes.Should().NotBeNull();
            recipes!.Count.Should().Be(3);
        }

        [Fact]
        public async Task Post_InvalidRecipe_MissingTitle_ReturnsBadRequest()
        {
            using var client = await CreateTestClientAsync();
            var payload = new
            {
                ingredients = "x,y",
                steps = "do it",
                cookingTimeMinutes = 5,
                dietaryTags = "vegan"
            };

            var resp = await client.PostAsJsonAsync("/api/recipes", payload);
            resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Post_ValidRecipe_ReturnsCreated_AndAppearsInList()
        {
            using var client = await CreateTestClientAsync();

            var newRecipe = new
            {
                title = "Test Recipe",
                ingredients = "a,b,c",
                steps = "Mix and serve",
                cookingTimeMinutes = 7,
                dietaryTags = "test"
            };

            var resp = await client.PostAsJsonAsync("/api/recipes", newRecipe);
            resp.StatusCode.Should().Be(HttpStatusCode.Created);
            var created = await resp.Content.ReadFromJsonAsync<Recipe>();
            created.Should().NotBeNull();
            created!.Id.Should().BeGreaterThan(0);
            created.Title.Should().Be("Test Recipe");

            var listResp = await client.GetAsync("/api/recipes");
            listResp.EnsureSuccessStatusCode();
            var list = await listResp.Content.ReadFromJsonAsync<List<Recipe>>();
            list.Should().HaveCount(4);
            list.Should().Contain(r => r.Title == "Test Recipe");
        }

        [Fact]
        public async Task Put_NonExistentId_ReturnsNotFound()
        {
            using var client = await CreateTestClientAsync();

            var fake = new Recipe
            {
                Id = 9999,
                Title = "Non Exist",
                Ingredients = "x",
                Steps = "y",
                CookingTimeMinutes = 5,
                DietaryTags = "none"
            };

            var resp = await client.PutAsJsonAsync($"/api/recipes/{fake.Id}", fake);
            resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<HttpClient> CreateTestClientAsync()
        {
            var factory = new CustomWebApplicationFactoryForTest(_connection);

            var client = factory.CreateClient();

            await SeedDatabaseAsync(factory);

            return client;
        }

        private async Task SeedDatabaseAsync(CustomWebApplicationFactoryForTest factory)
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<RecipeDbContext>();

            db.Database.EnsureCreated();

            db.Recipes.RemoveRange(db.Recipes);
            await db.SaveChangesAsync();

            db.Recipes.AddRange(new[]
            {
        new Recipe {
            Title = "Simple Tomato Pasta",
            Ingredients = "pasta, tomato sauce, garlic, olive oil, salt",
            Steps = "Boil pasta. Heat sauce. Combine and serve.",
            CookingTimeMinutes = 20,
            DietaryTags = "vegetarian"
        },
        new Recipe {
            Title = "Greek Salad",
            Ingredients = "cucumber, tomato, red onion, feta, olive oil, lemon",
            Steps = "Chop ingredients, toss with dressing.",
            CookingTimeMinutes = 10,
            DietaryTags = "vegetarian,gluten-free"
        },
        new Recipe {
            Title = "Avocado Toast",
            Ingredients = "bread, avocado, lemon, salt, pepper",
            Steps = "Toast bread, mash avocado, season, spread on toast.",
            CookingTimeMinutes = 5,
            DietaryTags = "vegetarian"
        }
    });
            await db.SaveChangesAsync();
        }
    }

    public class CustomWebApplicationFactoryForTest : WebApplicationFactory<Program>
    {
        private readonly SqliteConnection _connection;

        public CustomWebApplicationFactoryForTest(SqliteConnection connection)
        {
            _connection = connection;
        }

        protected override void ConfigureWebHost(Microsoft.AspNetCore.Hosting.IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<RecipeDbContext>));
                if (descriptor != null)
                    services.Remove(descriptor);

                services.AddDbContext<RecipeDbContext>(options =>
                {
                    options.UseSqlite(_connection);
                    options.ConfigureWarnings(warnings =>
                        warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
                });
            });
        }
    }
}