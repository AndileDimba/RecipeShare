using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace RecipeShare.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Recipes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Ingredients = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Steps = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CookingTimeMinutes = table.Column<int>(type: "int", nullable: false),
                    DietaryTags = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recipes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Recipes",
                columns: new[] { "Id", "CookingTimeMinutes", "DietaryTags", "Ingredients", "Steps", "Title" },
                values: new object[,]
                {
                    { 1, 20, "vegetarian", "pasta, tomato sauce, garlic, olive oil, salt", "Boil pasta. Heat sauce. Combine and serve.", "Simple Tomato Pasta" },
                    { 2, 10, "vegetarian,gluten-free", "cucumber, tomato, red onion, feta, olive oil, lemon", "Chop ingredients, toss with dressing.", "Greek Salad" },
                    { 3, 5, "vegetarian", "bread, avocado, lemon, salt, pepper", "Toast bread, mash avocado, season, spread on toast.", "Avocado Toast" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Recipes");
        }
    }
}
