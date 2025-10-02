using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RecipeShare.Api.Data;
using RecipeShare.Api.Models;

namespace RecipeShare.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecipesController : ControllerBase
    {
        private readonly RecipeDbContext _db;
        private readonly ILogger<RecipesController> _logger;

        public RecipesController(RecipeDbContext db, ILogger<RecipesController> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Return the list of recipes. Optionally filter by a dietary tag (e.g. "vegan").
        /// </summary>
        /// <param name="tag">Optional dietary tag to filter results.</param>
        /// <returns>List of matching recipes.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<Recipe>>> GetAll([FromQuery] string? tag = null)
        {
            var query = _db.Recipes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(tag))
            {
                query = query.Where(r => r.DietaryTags != null && r.DietaryTags.Contains(tag));
            }
            var list = await query.ToListAsync();
            return Ok(list);
        }

        /// <summary>
        /// Get a single recipe by id.
        /// </summary>
        /// <param name="id">Recipe id.</param>
        /// <returns>The requested recipe or 404 if not found.</returns>
        [HttpGet("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<Recipe>> Get(int id)
        {
            var r = await _db.Recipes.FindAsync(id);
            if (r == null) return NotFound();
            return Ok(r);
        }

        /// <summary>
        /// Create a new recipe. The server assigns the Id.
        /// </summary>
        /// <param name="recipe">Recipe payload (omit Id).</param>
        /// <returns>Created recipe with assigned Id.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Recipe>> Post([FromBody] Recipe recipe)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            _db.Recipes.Add(recipe);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = recipe.Id }, recipe);
        }

        /// <summary>
        /// Update an existing recipe. Id in the URL must match the Id in the body.
        /// </summary>
        /// <param name="id">Id of the recipe to update.</param>
        /// <param name="recipe">Updated recipe payload.</param>
        /// <returns>No content on success, 404 if the recipe doesn't exist.</returns>
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Put(int id, [FromBody] Recipe recipe)
        {
            if (id != recipe.Id) return BadRequest("ID mismatch.");
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var exists = await _db.Recipes.AnyAsync(r => r.Id == id);
            if (!exists) return NotFound();

            _db.Entry(recipe).State = EntityState.Modified;
            await _db.SaveChangesAsync();
            return NoContent();
        }

        /// <summary>
        /// Remove a recipe by id.
        /// </summary>
        /// <param name="id">Id of the recipe to delete.</param>
        /// <returns>No content on success, 404 if not found.</returns>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var r = await _db.Recipes.FindAsync(id);
            if (r == null) return NotFound();
            _db.Recipes.Remove(r);
            await _db.SaveChangesAsync();
            return NoContent();
        }
    }
}