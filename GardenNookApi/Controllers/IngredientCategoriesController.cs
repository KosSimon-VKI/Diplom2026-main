using GardenNookApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Inventory;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/ingredient-categories")]
    [Authorize]
    public class IngredientCategoriesController : Controller
    {
        private readonly AppDbContext _db;

        public IngredientCategoriesController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            var categories = await _db.IngredientCategories
                .AsNoTracking()
                .Select(c => new IngredientCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    ItemsCount = c.Ingredients.Count
                })
                .OrderBy(c => c.Name)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] IngredientCategoryRequest request)
        {
            var name = NormalizeName(request?.Name);
            var validationError = ValidateName(name);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (await CategoryNameExistsAsync(name, null))
            {
                return Conflict("Категория сырья с таким названием уже существует.");
            }

            var category = new IngredientCategory
            {
                Name = name
            };

            _db.IngredientCategories.Add(category);
            await _db.SaveChangesAsync();

            return Ok(new IngredientCategoryDto
            {
                Id = category.Id,
                Name = category.Name ?? string.Empty,
                ItemsCount = 0
            });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] IngredientCategoryRequest request)
        {
            var name = NormalizeName(request?.Name);
            var validationError = ValidateName(name);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (await CategoryNameExistsAsync(name, id))
            {
                return Conflict("Категория сырья с таким названием уже существует.");
            }

            var category = await _db.IngredientCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (category == null)
            {
                return NotFound("Категория сырья не найдена.");
            }

            category.Name = name;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _db.IngredientCategories
                .Include(c => c.Ingredients)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound("Категория сырья не найдена.");
            }

            if (category.Ingredients.Count > 0)
            {
                return Conflict($"Нельзя удалить категорию сырья: к ней привязано сырья: {category.Ingredients.Count}.");
            }

            _db.IngredientCategories.Remove(category);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string? ValidateName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Введите название категории сырья.";
            }

            if (name.Length > 50)
            {
                return "Название категории сырья не должно быть длиннее 50 символов.";
            }

            return null;
        }

        private async Task<bool> CategoryNameExistsAsync(string name, int? exceptId)
        {
            var rows = await _db.IngredientCategories
                .AsNoTracking()
                .Select(c => new { c.Id, c.Name })
                .ToListAsync();

            var normalizedName = NormalizeName(name);
            return rows.Any(c =>
                (!exceptId.HasValue || c.Id != exceptId.Value) &&
                string.Equals(
                    NormalizeName(c.Name),
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}
