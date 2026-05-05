using GardenNookApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Inventory;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/inventory")]
    [Authorize(Roles = "Администратор")]
    public class InventoryController : ControllerBase
    {
        private const string AdminRole = "Администратор";

        private readonly AppDbContext _db;

        public InventoryController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("edit-options")]
        public async Task<ActionResult<InventoryEditOptionsResponse>> GetEditOptions()
        {
            var units = await _db.UnitsOfMeasures
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var ingredientCategories = await _db.IngredientCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var semiFinishedCategories = await _db.SemiFinishedCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var technicalCards = await _db.TechnicalCards
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            return Ok(new InventoryEditOptionsResponse
            {
                UnitsOfMeasure = units,
                IngredientCategories = ingredientCategories,
                SemiFinishedCategories = semiFinishedCategories,
                TechnicalCards = technicalCards
            });
        }

        [HttpGet("ingredients")]
        public async Task<ActionResult<List<InventoryIngredientDto>>> GetIngredients([FromQuery] string? search)
        {
            var query = _db.Ingredients.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";
                query = query.Where(x =>
                    (x.Name != null && EF.Functions.Like(x.Name, pattern)) ||
                    (x.Category != null && x.Category.Name != null && EF.Functions.Like(x.Category.Name, pattern)) ||
                    (x.UnitOfMeasure != null && x.UnitOfMeasure.Name != null && EF.Functions.Like(x.UnitOfMeasure.Name, pattern)));
            }

            return Ok(await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new InventoryIngredientDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    Stock = x.Stock ?? 0m,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    CostRub = x.CostRub ?? 0m,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty
                })
                .ToListAsync());
        }

        [HttpPost("ingredients")]
        public async Task<ActionResult<InventoryIngredientDto>> CreateIngredient(InventoryIngredientRequest request)
        {
            var validationError = await ValidateIngredientRequestAsync(request, null);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var ingredient = new Ingredient();
            ApplyIngredientRequest(ingredient, request);

            _db.Ingredients.Add(ingredient);
            await _db.SaveChangesAsync();

            return Ok(await LoadIngredientAsync(ingredient.Id));
        }

        [HttpPost("ingredients/supply")]
        public async Task<IActionResult> SupplyIngredient(InventoryIngredientSupplyRequest request)
        {
            var lines = (request?.Lines ?? new List<InventoryIngredientSupplyLineRequest>())
                .Where(x => x.IngredientId > 0 || x.Quantity != 0)
                .ToList();

            if (lines.Count == 0)
            {
                return BadRequest("Добавьте хотя бы одну позицию поставки.");
            }

            if (lines.Any(x => x.IngredientId <= 0))
            {
                return BadRequest("Выберите сырье для каждой позиции поставки.");
            }

            if (lines.Any(x => x.Quantity <= 0))
            {
                return BadRequest("Количество поставки должно быть больше нуля.");
            }

            var quantitiesByIngredient = lines
                .GroupBy(x => x.IngredientId)
                .ToDictionary(x => x.Key, x => x.Sum(line => line.Quantity));

            var ingredientIds = quantitiesByIngredient.Keys.ToList();
            var ingredients = await _db.Ingredients
                .Where(x => ingredientIds.Contains(x.Id))
                .ToListAsync();

            if (ingredients.Count != ingredientIds.Count)
            {
                return NotFound("Одно или несколько выбранных сырьев не найдены.");
            }

            foreach (var ingredient in ingredients)
            {
                var quantity = quantitiesByIngredient[ingredient.Id];
                ingredient.Stock = (ingredient.Stock ?? 0m) + quantity;
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpPut("ingredients/{id:int}")]
        public async Task<IActionResult> UpdateIngredient(int id, InventoryIngredientRequest request)
        {
            var validationError = await ValidateIngredientRequestAsync(request, id);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var ingredient = await _db.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
            if (ingredient == null)
            {
                return NotFound("Сырье не найдено.");
            }

            ApplyIngredientRequest(ingredient, request);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("ingredients/{id:int}")]
        public async Task<IActionResult> DeleteIngredient(int id)
        {
            var ingredient = await _db.Ingredients.FirstOrDefaultAsync(x => x.Id == id);
            if (ingredient == null)
            {
                return NotFound("Сырье не найдено.");
            }

            var technicalCardCount = await _db.TechnicalCardIngredientCompositions.CountAsync(x => x.IngredientId == id);
            var writeOffCount = await _db.IngredientWriteOffActItems.CountAsync(x => x.IngredientId == id);
            var modifierCount = await _db.OrderDrinkItemModifiers.CountAsync(x => x.CoffeeIngredientId == id || x.MilkIngredientId == id);
            var linkCount = technicalCardCount + writeOffCount + modifierCount;
            if (linkCount > 0)
            {
                return Conflict($"Нельзя удалить сырье: оно используется в связанных данных ({linkCount}).");
            }

            _db.Ingredients.Remove(ingredient);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить сырье: оно используется в связанных данных.");
            }
        }

        [HttpGet("semi-finished")]
        public async Task<ActionResult<List<InventorySemiFinishedDto>>> GetSemiFinished([FromQuery] string? search)
        {
            var query = _db.SemiFinisheds.AsNoTracking();
            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";
                query = query.Where(x =>
                    (x.Name != null && EF.Functions.Like(x.Name, pattern)) ||
                    (x.Category != null && x.Category.Name != null && EF.Functions.Like(x.Category.Name, pattern)) ||
                    (x.UnitOfMeasure != null && x.UnitOfMeasure.Name != null && EF.Functions.Like(x.UnitOfMeasure.Name, pattern)) ||
                    (x.TechnicalCard != null && x.TechnicalCard.Name != null && EF.Functions.Like(x.TechnicalCard.Name, pattern)));
            }

            return Ok(await query
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new InventorySemiFinishedDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    CostRub = x.CostRub ?? 0m,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    TechnicalCardId = x.TechnicalCardId,
                    TechnicalCardName = x.TechnicalCard != null ? x.TechnicalCard.Name ?? string.Empty : string.Empty,
                    FatsG = x.FatsG ?? 0m,
                    ProteinsG = x.ProteinsG ?? 0m,
                    CarbsG = x.CarbsG ?? 0m,
                    CaloriesKcal = x.CaloriesKcal ?? 0m,
                    Kilojoules = x.Kilojoules ?? 0m
                })
                .ToListAsync());
        }

        [HttpPost("semi-finished")]
        public async Task<ActionResult<InventorySemiFinishedDto>> CreateSemiFinished(InventorySemiFinishedRequest request)
        {
            var validationError = await ValidateSemiFinishedRequestAsync(request, null);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var semiFinished = new SemiFinished();
            ApplySemiFinishedRequest(semiFinished, request);

            _db.SemiFinisheds.Add(semiFinished);
            await _db.SaveChangesAsync();

            return Ok(await LoadSemiFinishedAsync(semiFinished.Id));
        }

        [HttpPut("semi-finished/{id:int}")]
        public async Task<IActionResult> UpdateSemiFinished(int id, InventorySemiFinishedRequest request)
        {
            var validationError = await ValidateSemiFinishedRequestAsync(request, id);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var semiFinished = await _db.SemiFinisheds.FirstOrDefaultAsync(x => x.Id == id);
            if (semiFinished == null)
            {
                return NotFound("Полуфабрикат не найден.");
            }

            ApplySemiFinishedRequest(semiFinished, request);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("semi-finished/{id:int}")]
        public async Task<IActionResult> DeleteSemiFinished(int id)
        {
            var semiFinished = await _db.SemiFinisheds.FirstOrDefaultAsync(x => x.Id == id);
            if (semiFinished == null)
            {
                return NotFound("Полуфабрикат не найден.");
            }

            var technicalCardCount = await _db.TechnicalCardSemiFinishedCompositions.CountAsync(x => x.SemiFinishedId == id);
            var writeOffCount = await _db.SemiFinishedWriteOffActItems.CountAsync(x => x.SemiFinishedId == id);
            var preparationCount = await _db.Preparations.CountAsync(x => x.SemiFinishedId == id);
            var taskCount = await _db.PreparationTasks.CountAsync(x => x.SemiFinishedId == id);
            var linkCount = technicalCardCount + writeOffCount + preparationCount + taskCount;
            if (linkCount > 0)
            {
                return Conflict($"Нельзя удалить полуфабрикат: он используется в связанных данных ({linkCount}).");
            }

            _db.SemiFinisheds.Remove(semiFinished);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить полуфабрикат: он используется в связанных данных.");
            }
        }

        private async Task<InventoryIngredientDto> LoadIngredientAsync(int id)
        {
            return await _db.Ingredients
                .AsNoTracking()
                .Select(x => new InventoryIngredientDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    Stock = x.Stock ?? 0m,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    CostRub = x.CostRub ?? 0m,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty
                })
                .FirstAsync(x => x.Id == id);
        }

        private async Task<InventorySemiFinishedDto> LoadSemiFinishedAsync(int id)
        {
            return await _db.SemiFinisheds
                .AsNoTracking()
                .Select(x => new InventorySemiFinishedDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    CostRub = x.CostRub ?? 0m,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    TechnicalCardId = x.TechnicalCardId,
                    TechnicalCardName = x.TechnicalCard != null ? x.TechnicalCard.Name ?? string.Empty : string.Empty,
                    FatsG = x.FatsG ?? 0m,
                    ProteinsG = x.ProteinsG ?? 0m,
                    CarbsG = x.CarbsG ?? 0m,
                    CaloriesKcal = x.CaloriesKcal ?? 0m,
                    Kilojoules = x.Kilojoules ?? 0m
                })
                .FirstAsync(x => x.Id == id);
        }

        private async Task<string?> ValidateIngredientRequestAsync(InventoryIngredientRequest? request, int? exceptId)
        {
            if (request == null)
            {
                return "Заполните данные сырья.";
            }

            var name = NormalizeName(request.Name);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Введите название сырья.";
            }

            if (name.Length > 255)
            {
                return "Название сырья не должно быть длиннее 255 символов.";
            }

            if (request.Stock < 0 || request.CostRub < 0)
            {
                return "Остаток и себестоимость не могут быть отрицательными.";
            }

            if (await IngredientNameExistsAsync(name, exceptId))
            {
                return "Сырье с таким названием уже существует.";
            }

            if (request.UnitOfMeasureId.HasValue &&
                !await _db.UnitsOfMeasures.AsNoTracking().AnyAsync(x => x.Id == request.UnitOfMeasureId.Value))
            {
                return "Выбранная единица измерения не найдена.";
            }

            if (request.CategoryId.HasValue &&
                !await _db.IngredientCategories.AsNoTracking().AnyAsync(x => x.Id == request.CategoryId.Value))
            {
                return "Выбранная категория сырья не найдена.";
            }

            return null;
        }

        private async Task<string?> ValidateSemiFinishedRequestAsync(InventorySemiFinishedRequest? request, int? exceptId)
        {
            if (request == null)
            {
                return "Заполните данные полуфабриката.";
            }

            var name = NormalizeName(request.Name);
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Введите название полуфабриката.";
            }

            if (name.Length > 255)
            {
                return "Название полуфабриката не должно быть длиннее 255 символов.";
            }

            if (request.CostRub < 0 ||
                request.FatsG < 0 ||
                request.ProteinsG < 0 ||
                request.CarbsG < 0 ||
                request.CaloriesKcal < 0 ||
                request.Kilojoules < 0)
            {
                return "Себестоимость и КБЖУ не могут быть отрицательными.";
            }

            if (await SemiFinishedNameExistsAsync(name, exceptId))
            {
                return "Полуфабрикат с таким названием уже существует.";
            }

            if (request.UnitOfMeasureId.HasValue &&
                !await _db.UnitsOfMeasures.AsNoTracking().AnyAsync(x => x.Id == request.UnitOfMeasureId.Value))
            {
                return "Выбранная единица измерения не найдена.";
            }

            if (request.CategoryId.HasValue &&
                !await _db.SemiFinishedCategories.AsNoTracking().AnyAsync(x => x.Id == request.CategoryId.Value))
            {
                return "Выбранная категория полуфабриката не найдена.";
            }

            if (request.TechnicalCardId.HasValue &&
                !await _db.TechnicalCards.AsNoTracking().AnyAsync(x => x.Id == request.TechnicalCardId.Value))
            {
                return "Выбранная техкарта не найдена.";
            }

            return null;
        }

        private async Task<bool> IngredientNameExistsAsync(string name, int? exceptId)
        {
            var normalizedName = NormalizeName(name);
            var rows = await _db.Ingredients
                .AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            return rows.Any(x =>
                (!exceptId.HasValue || x.Id != exceptId.Value) &&
                string.Equals(NormalizeName(x.Name), normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> SemiFinishedNameExistsAsync(string name, int? exceptId)
        {
            var normalizedName = NormalizeName(name);
            var rows = await _db.SemiFinisheds
                .AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            return rows.Any(x =>
                (!exceptId.HasValue || x.Id != exceptId.Value) &&
                string.Equals(NormalizeName(x.Name), normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        private static void ApplyIngredientRequest(Ingredient ingredient, InventoryIngredientRequest request)
        {
            ingredient.Name = NormalizeName(request.Name);
            ingredient.Stock = request.Stock;
            ingredient.UnitOfMeasureId = request.UnitOfMeasureId;
            ingredient.CostRub = request.CostRub;
            ingredient.CategoryId = request.CategoryId;
        }

        private static void ApplySemiFinishedRequest(SemiFinished semiFinished, InventorySemiFinishedRequest request)
        {
            semiFinished.Name = NormalizeName(request.Name);
            semiFinished.CostRub = request.CostRub;
            semiFinished.CategoryId = request.CategoryId;
            semiFinished.UnitOfMeasureId = request.UnitOfMeasureId;
            semiFinished.TechnicalCardId = request.TechnicalCardId;
            semiFinished.FatsG = request.FatsG;
            semiFinished.ProteinsG = request.ProteinsG;
            semiFinished.CarbsG = request.CarbsG;
            semiFinished.CaloriesKcal = request.CaloriesKcal;
            semiFinished.Kilojoules = request.Kilojoules;
        }

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
