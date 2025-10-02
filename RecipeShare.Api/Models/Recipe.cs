using System.ComponentModel.DataAnnotations;

namespace RecipeShare.Api.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Ingredients { get; set; } = string.Empty;

        [Required]
        public string Steps { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int CookingTimeMinutes { get; set; }
        public string? DietaryTags { get; set; }
    }
}