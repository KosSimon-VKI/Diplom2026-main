using GardenNookApi.Entities;
using GardenNookApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using TransferModels.Menu;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/menu")]
    [Authorize]
    public class MenuController : Controller
    {
        private const string AdminRole = "Администратор";
        private const int UnitGramsId = 2;
        private const int UnitMillilitersId = 3;
        private const int UnitPiecesId = 4;
        private const int UnitKilogramsId = 5;
        private const int UnitLitersId = 6;
        private const int IngredientsGraphMaxDepth = 16;
        private const string InactiveCategoryName = "Неактивные";
        private const int InactiveDishCategoryId = 12;
        private const string DishesCategoryType = "dishes";
        private const string DrinksCategoryType = "drinks";
        private const string ToppingsCategoryType = "toppings";
        private const int DefaultMenuItemsTake = 100;
        private const int MaxMenuItemsTake = 200;
        private static readonly string InactiveCategoryNameLower = InactiveCategoryName.ToLower();
        private static readonly CultureInfo RussianCulture = CultureInfo.GetCultureInfo("ru-RU");
        private static readonly int[] MilkModifierIngredientIds = [106, 107, 108, 110, 113, 115, 118];
        private static readonly int[] CoffeeModifierIngredientIds = [6, 8];
        private static readonly HashSet<string> IngredientDescriptorTokens =
        [
            "очищенный", "очищенная", "очищенное", "очищенные", "очищ",
            "свежий", "свежая", "свежее", "свежие",
            "свежемороженый", "свежемороженая", "свежемороженое", "свежемороженые",
            "жареный", "жареная", "жареное", "жареные",
            "вареный", "вареная", "вареное", "вареные",
            "отварной", "отварная", "отварное", "отварные",
            "замороженный", "замороженная", "замороженное", "замороженные",
            "размороженный", "размороженная", "размороженное", "размороженные",
            "сушеный", "сушеная", "сушеное", "сушеные",
            "бланшированный", "бланшированная", "бланшированное", "бланшированные",
            "маринованный", "маринованная", "маринованное", "маринованные",
            "соленый", "соленая", "соленое", "соленые",
            "копченый", "копченая", "копченое", "копченые",
            "тушеный", "тушеная", "тушеное", "тушеные",
            "запеченный", "запеченная", "запеченное", "запеченные",
            "резаный", "резаная", "резаное", "резаные",
            "нарезанный", "нарезанная", "нарезанное", "нарезанные",
            "рубленый", "рубленая", "рубленое", "рубленые",
            "тертый", "тертая", "тертое", "тертые",
            "измельченный", "измельченная", "измельченное", "измельченные"
        ];
        private static readonly char[] IngredientNameSeparators =
        [
            ' ', '\t', '\r', '\n',
            ',', '.', ';', ':',
            '(', ')', '[', ']', '{', '}',
            '"', '\'',
            '/', '\\',
            '+', '-', '_'
        ];

        private readonly AppDbContext database;
        private readonly IPreparationStockService stockService;

        public MenuController(AppDbContext db, IPreparationStockService stockService)
        {
            database = db;
            this.stockService = stockService;
        }

        [HttpGet]
        public async Task<IActionResult> GetFullMenu()
        {
            await stockService.RefreshMenuAvailabilityAsync();

            // Блюда
            var dishRows = await database.Dishes
                .AsNoTracking()
                .Include(q => q.Category)
                .Where(d =>
                    d.CategoryId != InactiveDishCategoryId &&
                    (d.Category == null ||
                     d.Category.Name == null ||
                     d.Category.Name.Trim().ToLower() != InactiveCategoryNameLower))
                .OrderByDescending(d => d.IsAvailable)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.PriceRub,
                    CategoryName = d.Category != null ? d.Category.Name : null,
                    d.CaloriesKcal,
                    d.ProteinsG,
                    d.FatsG,
                    d.CarbsG,
                    d.ImageUrl,
                    d.IsAvailable,
                    d.TechnicalCardId
                })
                .ToListAsync();

            var dishCardIds = dishRows
                .Where(d => d.TechnicalCardId.HasValue)
                .Select(d => d.TechnicalCardId!.Value)
                .Distinct()
                .ToList();

            var dishWeightByCard = new Dictionary<int, decimal>();
            if (dishCardIds.Count > 0)
            {
                var ingredientRows = await database.TechnicalCardIngredientCompositions
                    .AsNoTracking()
                    .Where(x => x.TechnicalCardId.HasValue && dishCardIds.Contains(x.TechnicalCardId.Value))
                    .Select(x => new
                    {
                        TechnicalCardId = x.TechnicalCardId!.Value,
                        x.OutputWeight,
                        x.NetWeight,
                        x.GrossWeight,
                        x.UnitOfMeasureId
                    })
                    .ToListAsync();

                foreach (var row in ingredientRows)
                {
                    AddDishWeight(
                        dishWeightByCard,
                        row.TechnicalCardId,
                        ConvertToGrams(PickWeight(row.OutputWeight, row.NetWeight, row.GrossWeight), row.UnitOfMeasureId));
                }

                var semiFinishedRows = await database.TechnicalCardSemiFinishedCompositions
                    .AsNoTracking()
                    .Where(x => x.TechnicalCardId.HasValue && dishCardIds.Contains(x.TechnicalCardId.Value))
                    .Select(x => new
                    {
                        TechnicalCardId = x.TechnicalCardId!.Value,
                        x.OutputWeight,
                        x.NetWeight,
                        x.GrossWeight,
                        x.UnitOfMeasureId
                    })
                    .ToListAsync();

                foreach (var row in semiFinishedRows)
                {
                    AddDishWeight(
                        dishWeightByCard,
                        row.TechnicalCardId,
                        ConvertToGrams(PickWeight(row.OutputWeight, row.NetWeight, row.GrossWeight), row.UnitOfMeasureId));
                }
            }

            // Напитки
            var drinkRows = await database.Drinks
                .AsNoTracking()
                .Include(q => q.Category)
                .Include(q => q.UnitOfMeasure)
                .Where(d =>
                    d.Category == null ||
                    d.Category.Name == null ||
                    d.Category.Name.Trim().ToLower() != InactiveCategoryNameLower)
                .OrderByDescending(d => d.IsAvailable)
                .ThenBy(d => d.Name)
                .Select(d => new
                {
                    Id = d.Id,
                    Name = d.Name,
                    d.Quantity,
                    UnitName = d.UnitOfMeasure != null ? d.UnitOfMeasure.Name : null,
                    Price = Convert.ToInt32(d.PriceRub),
                    d.CategoryId,
                    CategoryName = d.Category != null ? d.Category.Name : null,
                    Calories = Convert.ToInt32(d.CaloriesKcal),
                    Proteins = Convert.ToInt32(d.ProteinsG),
                    Fats = Convert.ToInt32(d.FatsG),
                    Carbs = Convert.ToInt32(d.CarbsG),
                    ImageUrl = string.IsNullOrWhiteSpace(d.ImageUrl)
                        ? "/Images/placeholder.png"
                        : "/Images/" + d.ImageUrl,
                    IsAvailable = d.IsAvailable,
                    d.TechnicalCardId
                })
                .ToListAsync();

            var menuCardIds = dishRows
                .Where(d => d.TechnicalCardId.HasValue)
                .Select(d => d.TechnicalCardId!.Value)
                .Concat(drinkRows
                    .Where(d => d.TechnicalCardId.HasValue)
                    .Select(d => d.TechnicalCardId!.Value))
                .Distinct()
                .ToList();

            var ingredientsByCard = await BuildIngredientsByTechnicalCardAsync(
                menuCardIds,
                HttpContext.RequestAborted);

            var dishes = dishRows
                .Select(d =>
                {
                    var categoryName = d.CategoryName ?? string.Empty;

                    return new DishDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        WeightLabel = BuildDishWeightLabel(d.TechnicalCardId, dishWeightByCard),
                        Ingredients = BuildIngredientsText(d.TechnicalCardId, ingredientsByCard),
                        Price = Convert.ToInt32(d.PriceRub),
                        Category = categoryName,
                        Calories = Convert.ToInt32(d.CaloriesKcal),
                        Proteins = Convert.ToInt32(d.ProteinsG),
                        Fats = Convert.ToInt32(d.FatsG),
                        Carbs = Convert.ToInt32(d.CarbsG),
                        ImageUrl = string.IsNullOrWhiteSpace(d.ImageUrl)
                            ? "/Images/placeholder.png"
                            : "/Images/" + d.ImageUrl,
                        IsAvailable = d.IsAvailable
                    };
                })
                .ToList();

            var drinks = drinkRows
                .Select(d =>
                {
                    var categoryName = d.CategoryName ?? string.Empty;

                    return new DrinkDto
                    {
                        Id = d.Id,
                        Name = d.Name,
                        VolumeLabel = !d.Quantity.HasValue || string.IsNullOrWhiteSpace(d.UnitName)
                            ? string.Empty
                            : d.Quantity.Value.ToString("0.##", CultureInfo.InvariantCulture) + " " + NormalizeVolumeUnit(d.UnitName),
                        Ingredients = BuildIngredientsText(d.TechnicalCardId, ingredientsByCard),
                        Price = d.Price,
                        CategoryId = d.CategoryId,
                        Category = categoryName,
                        Calories = d.Calories,
                        Proteins = d.Proteins,
                        Fats = d.Fats,
                        Carbs = d.Carbs,
                        ImageUrl = string.IsNullOrWhiteSpace(d.ImageUrl)
                            ? "/Images/placeholder.png"
                            : "/Images/" + d.ImageUrl,
                        IsAvailable = d.IsAvailable
                    };
                })
                .ToList();

            // Добавки
            var toppingRows = await database.ToppingsAndSyrups
                .AsNoTracking()
                .Include(q => q.Category)
                .Include(q => q.UnitOfMeasure)
                .Where(t =>
                    t.Category == null ||
                    t.Category.Name == null ||
                    t.Category.Name.Trim().ToLower() != InactiveCategoryNameLower)
                .OrderByDescending(t => t.IsAvailable)
                .ThenBy(t => t.Name)
                .Select(t => new
                {
                    Id = t.Id,
                    t.Name,
                    t.Quantity,
                    UnitName = t.UnitOfMeasure != null ? t.UnitOfMeasure.Name : null,
                    Price = Convert.ToInt32(t.PriceRub),
                    Calories = t.CaloriesKcal ?? 0m,
                    CategoryName = t.Category != null ? t.Category.Name : null,
                    IsAvailable = t.IsAvailable
                })
                .ToListAsync();

            var toppings = toppingRows
                .Select(t =>
                {
                    var categoryName = t.CategoryName ?? string.Empty;
                    var unitName = (t.UnitName ?? string.Empty)
                        .Replace("Граммы", "гр")
                        .Replace("Миллилитры", "мл")
                        .Replace("Штуки", "шт");

                    return new ToppingDto
                    {
                        Id = t.Id,
                        Name = t.Name + " " + Convert.ToInt32(t.Quantity).ToString() + unitName,
                        Price = t.Price,
                        Calories = Convert.ToInt32(t.Calories),
                        Category = categoryName,
                        IsAvailable = t.IsAvailable
                    };
                })
                .ToList();

            var drinkModifiers = await LoadDrinkModifierCatalogAsync();

            return Ok(new MenuResponse
            {
                Dishes = dishes,
                Drinks = drinks,
                Toppings = toppings,
                DrinkModifiers = drinkModifiers
            });
        }

        [HttpGet("categories")]
        public async Task<IActionResult> GetCategories()
        {
            var dishCategories = await database.DishCategories
                .AsNoTracking()
                .Select(c => new MenuCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Type = DishesCategoryType,
                    ItemsCount = c.Dishes.Count
                })
                .ToListAsync();

            var drinkCategories = await database.DrinkCategories
                .AsNoTracking()
                .Select(c => new MenuCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Type = DrinksCategoryType,
                    ItemsCount = c.Drinks.Count
                })
                .ToListAsync();

            var toppingCategories = await database.ToppingCategories
                .AsNoTracking()
                .Select(c => new MenuCategoryDto
                {
                    Id = c.Id,
                    Name = c.Name ?? string.Empty,
                    Type = ToppingsCategoryType,
                    ItemsCount = c.ToppingsAndSyrups.Count
                })
                .ToListAsync();

            return Ok(dishCategories
                .Concat(drinkCategories)
                .Concat(toppingCategories)
                .OrderBy(x => GetCategoryTypeOrder(x.Type))
                .ThenBy(x => x.Name)
                .ToList());
        }

        [HttpPost("categories/{type}")]
        public async Task<IActionResult> CreateCategory(string type, [FromBody] MenuCategoryRequest request)
        {
            var normalizedType = NormalizeCategoryType(type);
            if (normalizedType == null)
            {
                return BadRequest("Неизвестный тип категории.");
            }

            var name = NormalizeCategoryName(request?.Name);
            var validationError = ValidateCategoryName(name);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (await CategoryNameExistsAsync(normalizedType, name, null))
            {
                return Conflict("Категория с таким названием уже существует.");
            }

            var category = new MenuCategoryDto
            {
                Name = name,
                Type = normalizedType
            };

            switch (normalizedType)
            {
                case DishesCategoryType:
                    var dishCategory = new DishCategory { Name = name };
                    database.DishCategories.Add(dishCategory);
                    await database.SaveChangesAsync();
                    category.Id = dishCategory.Id;
                    break;

                case DrinksCategoryType:
                    var drinkCategory = new DrinkCategory { Name = name };
                    database.DrinkCategories.Add(drinkCategory);
                    await database.SaveChangesAsync();
                    category.Id = drinkCategory.Id;
                    break;

                case ToppingsCategoryType:
                    var toppingCategory = new ToppingCategory { Name = name };
                    database.ToppingCategories.Add(toppingCategory);
                    await database.SaveChangesAsync();
                    category.Id = toppingCategory.Id;
                    break;
            }

            return Ok(category);
        }

        [HttpPut("categories/{type}/{id:int}")]
        public async Task<IActionResult> UpdateCategory(string type, int id, [FromBody] MenuCategoryRequest request)
        {
            var normalizedType = NormalizeCategoryType(type);
            if (normalizedType == null)
            {
                return BadRequest("Неизвестный тип категории.");
            }

            var name = NormalizeCategoryName(request?.Name);
            var validationError = ValidateCategoryName(name);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            if (await CategoryNameExistsAsync(normalizedType, name, id))
            {
                return Conflict("Категория с таким названием уже существует.");
            }

            switch (normalizedType)
            {
                case DishesCategoryType:
                    var dishCategory = await database.DishCategories.FirstOrDefaultAsync(c => c.Id == id);
                    if (dishCategory == null)
                    {
                        return NotFound("Категория не найдена.");
                    }

                    dishCategory.Name = name;
                    break;

                case DrinksCategoryType:
                    var drinkCategory = await database.DrinkCategories.FirstOrDefaultAsync(c => c.Id == id);
                    if (drinkCategory == null)
                    {
                        return NotFound("Категория не найдена.");
                    }

                    drinkCategory.Name = name;
                    break;

                case ToppingsCategoryType:
                    var toppingCategory = await database.ToppingCategories.FirstOrDefaultAsync(c => c.Id == id);
                    if (toppingCategory == null)
                    {
                        return NotFound("Категория не найдена.");
                    }

                    toppingCategory.Name = name;
                    break;
            }

            await database.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("categories/{type}/{id:int}")]
        public async Task<IActionResult> DeleteCategory(string type, int id)
        {
            var normalizedType = NormalizeCategoryType(type);
            if (normalizedType == null)
            {
                return BadRequest("Неизвестный тип категории.");
            }

            switch (normalizedType)
            {
                case DishesCategoryType:
                    var dishCategory = await database.DishCategories
                        .Include(c => c.Dishes)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    if (dishCategory == null)
                    {
                        return NotFound("Категория не найдена.");
                    }

                    if (dishCategory.Dishes.Count > 0)
                    {
                        return Conflict($"Нельзя удалить категорию: к ней привязано позиций: {dishCategory.Dishes.Count}.");
                    }

                    database.DishCategories.Remove(dishCategory);
                    break;

                case DrinksCategoryType:
                    var drinkCategory = await database.DrinkCategories
                        .Include(c => c.Drinks)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    if (drinkCategory == null)
                    {
                        return NotFound("Категория не найдена.");
                    }

                    if (drinkCategory.Drinks.Count > 0)
                    {
                        return Conflict($"Нельзя удалить категорию: к ней привязано позиций: {drinkCategory.Drinks.Count}.");
                    }

                    database.DrinkCategories.Remove(drinkCategory);
                    break;

                case ToppingsCategoryType:
                    var toppingCategory = await database.ToppingCategories
                        .Include(c => c.ToppingsAndSyrups)
                        .FirstOrDefaultAsync(c => c.Id == id);
                    if (toppingCategory == null)
                    {
                        return NotFound("Категория не найдена.");
                    }

                    if (toppingCategory.ToppingsAndSyrups.Count > 0)
                    {
                        return Conflict($"Нельзя удалить категорию: к ней привязано позиций: {toppingCategory.ToppingsAndSyrups.Count}.");
                    }

                    database.ToppingCategories.Remove(toppingCategory);
                    break;
            }

            await database.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet("items")]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> GetItems(
            [FromQuery] string? type,
            [FromQuery] int? categoryId,
            [FromQuery] string? availability,
            [FromQuery] string? search,
            [FromQuery] int skip = 0,
            [FromQuery] int take = DefaultMenuItemsTake)
        {
            var normalizedType = string.IsNullOrWhiteSpace(type) || string.Equals(type, "all", StringComparison.OrdinalIgnoreCase)
                ? null
                : NormalizeCategoryType(type);
            if (!string.IsNullOrWhiteSpace(type) &&
                !string.Equals(type, "all", StringComparison.OrdinalIgnoreCase) &&
                normalizedType == null)
            {
                return BadRequest("Неизвестный тип позиции меню.");
            }

            skip = Math.Max(0, skip);
            take = Math.Clamp(take, 1, MaxMenuItemsTake);

            var availableOnly = availability switch
            {
                "available" => true,
                "unavailable" => false,
                _ => (bool?)null
            };

            var normalizedSearch = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
            var fetchLimit = skip + take;

            if (normalizedType == DishesCategoryType)
            {
                return Ok(await QueryDishItems(categoryId, availableOnly, normalizedSearch, skip, take).ToListAsync());
            }

            if (normalizedType == DrinksCategoryType)
            {
                return Ok(await QueryDrinkItems(categoryId, availableOnly, normalizedSearch, skip, take).ToListAsync());
            }

            if (normalizedType == ToppingsCategoryType)
            {
                return Ok(await QueryToppingItems(categoryId, availableOnly, normalizedSearch, skip, take).ToListAsync());
            }

            var dishes = await QueryDishItems(categoryId, availableOnly, normalizedSearch, 0, fetchLimit).ToListAsync();
            var drinks = await QueryDrinkItems(categoryId, availableOnly, normalizedSearch, 0, fetchLimit).ToListAsync();
            var toppings = await QueryToppingItems(categoryId, availableOnly, normalizedSearch, 0, fetchLimit).ToListAsync();

            return Ok(dishes
                .Concat(drinks)
                .Concat(toppings)
                .OrderBy(x => GetCategoryTypeOrder(x.Type))
                .ThenBy(x => x.Name)
                .Skip(skip)
                .Take(take)
                .ToList());
        }

        private IQueryable<MenuItemManagementDto> QueryDishItems(int? categoryId, bool? availableOnly, string? search, int skip, int take)
        {
            var query = database.Dishes.AsNoTracking();
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (availableOnly.HasValue)
            {
                query = query.Where(x => x.IsAvailable == availableOnly.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(x =>
                    (x.Name != null && EF.Functions.Like(x.Name, pattern)) ||
                    (x.Category != null && x.Category.Name != null && EF.Functions.Like(x.Category.Name, pattern)) ||
                    (x.TechnicalCard != null && x.TechnicalCard.Name != null && EF.Functions.Like(x.TechnicalCard.Name, pattern)));
            }

            return query
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(take)
                .Select(x => new MenuItemManagementDto
                {
                    Type = DishesCategoryType,
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    Quantity = null,
                    PriceRub = x.PriceRub ?? 0m,
                    TechnicalCardId = x.TechnicalCardId,
                    TechnicalCardName = x.TechnicalCard != null ? x.TechnicalCard.Name ?? string.Empty : string.Empty,
                    FatsG = x.FatsG ?? 0m,
                    ProteinsG = x.ProteinsG ?? 0m,
                    CarbsG = x.CarbsG ?? 0m,
                    CaloriesKcal = x.CaloriesKcal ?? 0m,
                    Kilojoules = x.Kilojoules ?? 0m,
                    ImageUrl = x.ImageUrl ?? string.Empty,
                    IsAvailable = x.IsAvailable
                });
        }

        private IQueryable<MenuItemManagementDto> QueryDrinkItems(int? categoryId, bool? availableOnly, string? search, int skip, int take)
        {
            var query = database.Drinks.AsNoTracking();
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (availableOnly.HasValue)
            {
                query = query.Where(x => x.IsAvailable == availableOnly.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(x =>
                    (x.Name != null && EF.Functions.Like(x.Name, pattern)) ||
                    (x.Category != null && x.Category.Name != null && EF.Functions.Like(x.Category.Name, pattern)) ||
                    (x.TechnicalCard != null && x.TechnicalCard.Name != null && EF.Functions.Like(x.TechnicalCard.Name, pattern)));
            }

            return query
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(take)
                .Select(x => new MenuItemManagementDto
                {
                    Type = DrinksCategoryType,
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    Quantity = x.Quantity,
                    PriceRub = x.PriceRub ?? 0m,
                    TechnicalCardId = x.TechnicalCardId,
                    TechnicalCardName = x.TechnicalCard != null ? x.TechnicalCard.Name ?? string.Empty : string.Empty,
                    FatsG = x.FatsG ?? 0m,
                    ProteinsG = x.ProteinsG ?? 0m,
                    CarbsG = x.CarbsG ?? 0m,
                    CaloriesKcal = x.CaloriesKcal ?? 0m,
                    Kilojoules = x.Kilojoules ?? 0m,
                    ImageUrl = x.ImageUrl ?? string.Empty,
                    IsAvailable = x.IsAvailable
                });
        }

        private IQueryable<MenuItemManagementDto> QueryToppingItems(int? categoryId, bool? availableOnly, string? search, int skip, int take)
        {
            var query = database.ToppingsAndSyrups.AsNoTracking();
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.CategoryId == categoryId.Value);
            }

            if (availableOnly.HasValue)
            {
                query = query.Where(x => x.IsAvailable == availableOnly.Value);
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search}%";
                query = query.Where(x =>
                    (x.Name != null && EF.Functions.Like(x.Name, pattern)) ||
                    (x.Category != null && x.Category.Name != null && EF.Functions.Like(x.Category.Name, pattern)) ||
                    (x.TechnicalCard != null && x.TechnicalCard.Name != null && EF.Functions.Like(x.TechnicalCard.Name, pattern)));
            }

            return query
                .OrderBy(x => x.Name)
                .Skip(skip)
                .Take(take)
                .Select(x => new MenuItemManagementDto
                {
                    Type = ToppingsCategoryType,
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    CategoryId = x.CategoryId,
                    CategoryName = x.Category != null ? x.Category.Name ?? string.Empty : string.Empty,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty,
                    Quantity = x.Quantity,
                    PriceRub = x.PriceRub ?? 0m,
                    TechnicalCardId = x.TechnicalCardId,
                    TechnicalCardName = x.TechnicalCard != null ? x.TechnicalCard.Name ?? string.Empty : string.Empty,
                    FatsG = x.FatsG ?? 0m,
                    ProteinsG = x.ProteinsG ?? 0m,
                    CarbsG = x.CarbsG ?? 0m,
                    CaloriesKcal = x.CaloriesKcal ?? 0m,
                    Kilojoules = x.Kilojoules ?? 0m,
                    ImageUrl = string.Empty,
                    IsAvailable = x.IsAvailable
                });
        }

        [HttpGet("items/edit-options")]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> GetItemEditOptions()
        {
            var dishCategories = await database.DishCategories
                .AsNoTracking()
                .Select(x => new MenuItemCategoryOptionDto
                {
                    Type = DishesCategoryType,
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var drinkCategories = await database.DrinkCategories
                .AsNoTracking()
                .Select(x => new MenuItemCategoryOptionDto
                {
                    Type = DrinksCategoryType,
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var toppingCategories = await database.ToppingCategories
                .AsNoTracking()
                .Select(x => new MenuItemCategoryOptionDto
                {
                    Type = ToppingsCategoryType,
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var units = await database.UnitsOfMeasures
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new MenuItemOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var technicalCards = await database.TechnicalCards
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new MenuItemOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            return Ok(new MenuItemEditOptionsResponse
            {
                Categories = dishCategories
                    .Concat(drinkCategories)
                    .Concat(toppingCategories)
                    .OrderBy(x => GetCategoryTypeOrder(x.Type))
                    .ThenBy(x => x.Name)
                    .ToList(),
                UnitsOfMeasure = units,
                TechnicalCards = technicalCards
            });
        }

        [HttpPost("items/{type}")]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> CreateItem(string type, [FromBody] MenuItemUpsertRequest request)
        {
            var normalizedType = NormalizeCategoryType(type);
            if (normalizedType == null)
            {
                return BadRequest("Неизвестный тип позиции меню.");
            }

            var validationError = await ValidateMenuItemRequestAsync(normalizedType, request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            MenuItemManagementDto created;
            switch (normalizedType)
            {
                case DishesCategoryType:
                    var dish = new Dish();
                    ApplyDishRequest(dish, request);
                    database.Dishes.Add(dish);
                    await database.SaveChangesAsync();
                    created = await LoadMenuItemAsync(normalizedType, dish.Id);
                    break;

                case DrinksCategoryType:
                    var drink = new Drink();
                    ApplyDrinkRequest(drink, request);
                    database.Drinks.Add(drink);
                    await database.SaveChangesAsync();
                    created = await LoadMenuItemAsync(normalizedType, drink.Id);
                    break;

                default:
                    var topping = new ToppingsAndSyrup();
                    ApplyToppingRequest(topping, request);
                    database.ToppingsAndSyrups.Add(topping);
                    await database.SaveChangesAsync();
                    created = await LoadMenuItemAsync(normalizedType, topping.Id);
                    break;
            }

            return Ok(created);
        }

        [HttpPut("items/{type}/{id:int}")]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> UpdateItem(string type, int id, [FromBody] MenuItemUpsertRequest request)
        {
            var normalizedType = NormalizeCategoryType(type);
            if (normalizedType == null)
            {
                return BadRequest("Неизвестный тип позиции меню.");
            }

            var validationError = await ValidateMenuItemRequestAsync(normalizedType, request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            switch (normalizedType)
            {
                case DishesCategoryType:
                    var dish = await database.Dishes.FirstOrDefaultAsync(x => x.Id == id);
                    if (dish == null)
                    {
                        return NotFound("Позиция меню не найдена.");
                    }

                    ApplyDishRequest(dish, request);
                    break;

                case DrinksCategoryType:
                    var drink = await database.Drinks.FirstOrDefaultAsync(x => x.Id == id);
                    if (drink == null)
                    {
                        return NotFound("Позиция меню не найдена.");
                    }

                    ApplyDrinkRequest(drink, request);
                    break;

                default:
                    var topping = await database.ToppingsAndSyrups.FirstOrDefaultAsync(x => x.Id == id);
                    if (topping == null)
                    {
                        return NotFound("Позиция меню не найдена.");
                    }

                    ApplyToppingRequest(topping, request);
                    break;
            }

            await database.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("items/{type}/{id:int}")]
        [Authorize(Roles = AdminRole)]
        public async Task<IActionResult> DeleteItem(string type, int id)
        {
            var normalizedType = NormalizeCategoryType(type);
            if (normalizedType == null)
            {
                return BadRequest("Неизвестный тип позиции меню.");
            }

            switch (normalizedType)
            {
                case DishesCategoryType:
                    var dish = await database.Dishes
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (dish == null)
                    {
                        return NotFound("Позиция меню не найдена.");
                    }

                    var dishOrderCount = await database.OrderDishItems.CountAsync(x => x.DishId == id);
                    if (dishOrderCount > 0)
                    {
                        return Conflict($"Нельзя удалить блюдо: оно используется в заказах ({dishOrderCount}).");
                    }

                    var dishPortionLimits = await database.MenuItemPortionLimits
                        .Where(x => x.ItemType == DishesCategoryType && x.ItemId == id)
                        .ToListAsync();
                    database.MenuItemPortionLimits.RemoveRange(dishPortionLimits);
                    database.Dishes.Remove(dish);
                    break;

                case DrinksCategoryType:
                    var drink = await database.Drinks
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (drink == null)
                    {
                        return NotFound("Позиция меню не найдена.");
                    }

                    var drinkOrderCount = await database.OrderDrinkItems.CountAsync(x => x.DrinkId == id);
                    if (drinkOrderCount > 0)
                    {
                        return Conflict($"Нельзя удалить напиток: он используется в заказах ({drinkOrderCount}).");
                    }

                    var drinkPortionLimits = await database.MenuItemPortionLimits
                        .Where(x => x.ItemType == DrinksCategoryType && x.ItemId == id)
                        .ToListAsync();
                    database.MenuItemPortionLimits.RemoveRange(drinkPortionLimits);
                    database.Drinks.Remove(drink);
                    break;

                default:
                    var topping = await database.ToppingsAndSyrups
                        .FirstOrDefaultAsync(x => x.Id == id);
                    if (topping == null)
                    {
                        return NotFound("Позиция меню не найдена.");
                    }

                    var orderToppingCount = await database.OrderToppingItems.CountAsync(x => x.ToppingId == id);
                    var dishToppingCount = await database.DishToppings.CountAsync(x => x.ToppingId == id);
                    var drinkToppingCount = await database.DrinkToppings.CountAsync(x => x.ToppingId == id);
                    var linkCount = orderToppingCount + dishToppingCount + drinkToppingCount;
                    if (linkCount > 0)
                    {
                        return Conflict($"Нельзя удалить добавку: она используется в заказах или связях ({linkCount}).");
                    }

                    var toppingPortionLimits = await database.MenuItemPortionLimits
                        .Where(x => x.ItemType == ToppingsCategoryType && x.ItemId == id)
                        .ToListAsync();
                    database.MenuItemPortionLimits.RemoveRange(toppingPortionLimits);
                    database.ToppingsAndSyrups.Remove(topping);
                    break;
            }

            try
            {
                await database.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить позицию меню: она используется в связанных данных.");
            }
        }

        private async Task<MenuItemManagementDto> LoadMenuItemAsync(string type, int id)
        {
            switch (type)
            {
                case DishesCategoryType:
                    var dish = await database.Dishes
                        .AsNoTracking()
                        .Include(x => x.Category)
                        .Include(x => x.UnitOfMeasure)
                        .Include(x => x.TechnicalCard)
                        .FirstAsync(x => x.Id == id);
                    return new MenuItemManagementDto
                    {
                        Type = DishesCategoryType,
                        Id = dish.Id,
                        Name = dish.Name ?? string.Empty,
                        CategoryId = dish.CategoryId,
                        CategoryName = dish.Category?.Name ?? string.Empty,
                        UnitOfMeasureId = dish.UnitOfMeasureId,
                        UnitName = dish.UnitOfMeasure?.Name ?? string.Empty,
                        PriceRub = dish.PriceRub ?? 0m,
                        TechnicalCardId = dish.TechnicalCardId,
                        TechnicalCardName = dish.TechnicalCard?.Name ?? string.Empty,
                        FatsG = dish.FatsG ?? 0m,
                        ProteinsG = dish.ProteinsG ?? 0m,
                        CarbsG = dish.CarbsG ?? 0m,
                        CaloriesKcal = dish.CaloriesKcal ?? 0m,
                        Kilojoules = dish.Kilojoules ?? 0m,
                        ImageUrl = dish.ImageUrl ?? string.Empty,
                        IsAvailable = dish.IsAvailable
                    };

                case DrinksCategoryType:
                    var drink = await database.Drinks
                        .AsNoTracking()
                        .Include(x => x.Category)
                        .Include(x => x.UnitOfMeasure)
                        .Include(x => x.TechnicalCard)
                        .FirstAsync(x => x.Id == id);
                    return new MenuItemManagementDto
                    {
                        Type = DrinksCategoryType,
                        Id = drink.Id,
                        Name = drink.Name ?? string.Empty,
                        CategoryId = drink.CategoryId,
                        CategoryName = drink.Category?.Name ?? string.Empty,
                        UnitOfMeasureId = drink.UnitOfMeasureId,
                        UnitName = drink.UnitOfMeasure?.Name ?? string.Empty,
                        Quantity = drink.Quantity,
                        PriceRub = drink.PriceRub ?? 0m,
                        TechnicalCardId = drink.TechnicalCardId,
                        TechnicalCardName = drink.TechnicalCard?.Name ?? string.Empty,
                        FatsG = drink.FatsG ?? 0m,
                        ProteinsG = drink.ProteinsG ?? 0m,
                        CarbsG = drink.CarbsG ?? 0m,
                        CaloriesKcal = drink.CaloriesKcal ?? 0m,
                        Kilojoules = drink.Kilojoules ?? 0m,
                        ImageUrl = drink.ImageUrl ?? string.Empty,
                        IsAvailable = drink.IsAvailable
                    };

                default:
                    var topping = await database.ToppingsAndSyrups
                        .AsNoTracking()
                        .Include(x => x.Category)
                        .Include(x => x.UnitOfMeasure)
                        .Include(x => x.TechnicalCard)
                        .FirstAsync(x => x.Id == id);
                    return new MenuItemManagementDto
                    {
                        Type = ToppingsCategoryType,
                        Id = topping.Id,
                        Name = topping.Name ?? string.Empty,
                        CategoryId = topping.CategoryId,
                        CategoryName = topping.Category?.Name ?? string.Empty,
                        UnitOfMeasureId = topping.UnitOfMeasureId,
                        UnitName = topping.UnitOfMeasure?.Name ?? string.Empty,
                        Quantity = topping.Quantity,
                        PriceRub = topping.PriceRub ?? 0m,
                        TechnicalCardId = topping.TechnicalCardId,
                        TechnicalCardName = topping.TechnicalCard?.Name ?? string.Empty,
                        FatsG = topping.FatsG ?? 0m,
                        ProteinsG = topping.ProteinsG ?? 0m,
                        CarbsG = topping.CarbsG ?? 0m,
                        CaloriesKcal = topping.CaloriesKcal ?? 0m,
                        Kilojoules = topping.Kilojoules ?? 0m,
                        IsAvailable = topping.IsAvailable
                    };
            }
        }

        private async Task<string?> ValidateMenuItemRequestAsync(string type, MenuItemUpsertRequest? request)
        {
            if (request == null)
            {
                return "Передайте данные позиции меню.";
            }

            if (string.IsNullOrWhiteSpace(NormalizeMenuItemName(request.Name)))
            {
                return "Введите название позиции меню.";
            }

            if (NormalizeMenuItemName(request.Name).Length > 255)
            {
                return "Название позиции меню не должно быть длиннее 255 символов.";
            }

            if (request.PriceRub < 0)
            {
                return "Цена не может быть отрицательной.";
            }

            if (request.Quantity.HasValue && request.Quantity.Value < 0)
            {
                return "Количество не может быть отрицательным.";
            }

            if (request.FatsG < 0 || request.ProteinsG < 0 || request.CarbsG < 0 || request.CaloriesKcal < 0 || request.Kilojoules < 0)
            {
                return "КБЖУ не может содержать отрицательные значения.";
            }

            if (request.CategoryId.HasValue && !await CategoryIdExistsAsync(type, request.CategoryId.Value))
            {
                return "Выбранная категория не найдена.";
            }

            if (request.UnitOfMeasureId.HasValue &&
                !await database.UnitsOfMeasures.AsNoTracking().AnyAsync(x => x.Id == request.UnitOfMeasureId.Value))
            {
                return "Выбранная единица измерения не найдена.";
            }

            if (request.TechnicalCardId.HasValue &&
                !await database.TechnicalCards.AsNoTracking().AnyAsync(x => x.Id == request.TechnicalCardId.Value))
            {
                return "Выбранная техкарта не найдена.";
            }

            return null;
        }

        private async Task<bool> CategoryIdExistsAsync(string type, int id)
        {
            return type switch
            {
                DishesCategoryType => await database.DishCategories.AsNoTracking().AnyAsync(x => x.Id == id),
                DrinksCategoryType => await database.DrinkCategories.AsNoTracking().AnyAsync(x => x.Id == id),
                ToppingsCategoryType => await database.ToppingCategories.AsNoTracking().AnyAsync(x => x.Id == id),
                _ => false
            };
        }

        private static void ApplyDishRequest(Dish dish, MenuItemUpsertRequest request)
        {
            dish.Name = NormalizeMenuItemName(request.Name);
            dish.CategoryId = request.CategoryId;
            dish.UnitOfMeasureId = request.UnitOfMeasureId;
            dish.PriceRub = request.PriceRub;
            dish.TechnicalCardId = request.TechnicalCardId;
            dish.FatsG = request.FatsG;
            dish.ProteinsG = request.ProteinsG;
            dish.CarbsG = request.CarbsG;
            dish.CaloriesKcal = request.CaloriesKcal;
            dish.Kilojoules = request.Kilojoules;
            dish.ImageUrl = NormalizeNullableText(request.ImageUrl);
            dish.IsAvailable = request.IsAvailable;
        }

        private static void ApplyDrinkRequest(Drink drink, MenuItemUpsertRequest request)
        {
            drink.Name = NormalizeMenuItemName(request.Name);
            drink.CategoryId = request.CategoryId;
            drink.UnitOfMeasureId = request.UnitOfMeasureId;
            drink.Quantity = request.Quantity;
            drink.PriceRub = request.PriceRub;
            drink.TechnicalCardId = request.TechnicalCardId;
            drink.FatsG = request.FatsG;
            drink.ProteinsG = request.ProteinsG;
            drink.CarbsG = request.CarbsG;
            drink.CaloriesKcal = request.CaloriesKcal;
            drink.Kilojoules = request.Kilojoules;
            drink.ImageUrl = NormalizeNullableText(request.ImageUrl);
            drink.IsAvailable = request.IsAvailable;
        }

        private static void ApplyToppingRequest(ToppingsAndSyrup topping, MenuItemUpsertRequest request)
        {
            topping.Name = NormalizeMenuItemName(request.Name);
            topping.CategoryId = request.CategoryId;
            topping.UnitOfMeasureId = request.UnitOfMeasureId;
            topping.Quantity = request.Quantity;
            topping.PriceRub = request.PriceRub;
            topping.TechnicalCardId = request.TechnicalCardId;
            topping.FatsG = request.FatsG;
            topping.ProteinsG = request.ProteinsG;
            topping.CarbsG = request.CarbsG;
            topping.CaloriesKcal = request.CaloriesKcal;
            topping.Kilojoules = request.Kilojoules;
            topping.IsAvailable = request.IsAvailable;
        }

        private static string NormalizeMenuItemName(string? name)
        {
            return string.Join(" ", (name ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string? NormalizeNullableText(string? value)
        {
            var normalized = (value ?? string.Empty).Trim();
            return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
        }

        private async Task<DrinkModifierCatalogDto> LoadDrinkModifierCatalogAsync()
        {
            var allModifierIds = MilkModifierIngredientIds
                .Concat(CoffeeModifierIngredientIds)
                .Distinct()
                .ToList();

            var ingredients = await database.Ingredients
                .AsNoTracking()
                .Where(i => allModifierIds.Contains(i.Id))
                .Select(i => new
                {
                    i.Id,
                    Name = i.Name ?? $"Ингредиент #{i.Id}"
                })
                .ToDictionaryAsync(i => i.Id, i => i.Name);

            return new DrinkModifierCatalogDto
            {
                MilkOptions = BuildModifierOptions(MilkModifierIngredientIds, ingredients),
                CoffeeOptions = BuildModifierOptions(CoffeeModifierIngredientIds, ingredients)
            };
        }

        private static List<DrinkModifierOptionDto> BuildModifierOptions(
            IReadOnlyCollection<int> orderedIds,
            IReadOnlyDictionary<int, string> ingredientNamesById)
        {
            return orderedIds
                .Where(id => ingredientNamesById.ContainsKey(id))
                .Select(id => new DrinkModifierOptionDto
                {
                    Id = id,
                    Name = ingredientNamesById[id]
                })
                .ToList();
        }

        private static string BuildDishWeightLabel(int? technicalCardId, IReadOnlyDictionary<int, decimal> weightByCard)
        {
            if (!technicalCardId.HasValue || !weightByCard.TryGetValue(technicalCardId.Value, out var grams) || grams <= 0)
            {
                return string.Empty;
            }

            return grams.ToString("0.##", CultureInfo.InvariantCulture) + " гр";
        }

        private static string BuildIngredientsText(int? technicalCardId, IReadOnlyDictionary<int, string> ingredientsByCard)
        {
            if (!technicalCardId.HasValue ||
                !ingredientsByCard.TryGetValue(technicalCardId.Value, out var ingredients) ||
                string.IsNullOrWhiteSpace(ingredients))
            {
                return "уточняется";
            }

            return ingredients;
        }

        private async Task<Dictionary<int, string>> BuildIngredientsByTechnicalCardAsync(
            IReadOnlyCollection<int> rootCardIds,
            CancellationToken cancellationToken)
        {
            if (rootCardIds.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            var uniqueRootCardIds = rootCardIds.Distinct().ToList();
            var discoveredCardIds = new HashSet<int>(uniqueRootCardIds);
            var childCardsByParent = new Dictionary<int, HashSet<int>>();
            var frontier = uniqueRootCardIds;

            for (var depth = 0; depth < IngredientsGraphMaxDepth && frontier.Count > 0; depth++)
            {
                var levelRows = await database.TechnicalCardSemiFinishedCompositions
                    .AsNoTracking()
                    .Where(x =>
                        x.TechnicalCardId.HasValue &&
                        x.SemiFinishedId.HasValue &&
                        frontier.Contains(x.TechnicalCardId.Value) &&
                        x.SemiFinished != null &&
                        x.SemiFinished.TechnicalCardId.HasValue)
                    .Select(x => new
                    {
                        ParentCardId = x.TechnicalCardId!.Value,
                        ChildCardId = x.SemiFinished!.TechnicalCardId!.Value
                    })
                    .ToListAsync(cancellationToken);

                var nextFrontier = new HashSet<int>();

                foreach (var row in levelRows)
                {
                    if (!childCardsByParent.TryGetValue(row.ParentCardId, out var childCards))
                    {
                        childCards = new HashSet<int>();
                        childCardsByParent[row.ParentCardId] = childCards;
                    }

                    childCards.Add(row.ChildCardId);

                    if (discoveredCardIds.Add(row.ChildCardId))
                    {
                        nextFrontier.Add(row.ChildCardId);
                    }
                }

                frontier = nextFrontier.ToList();
            }

            var allDiscoveredCardIds = discoveredCardIds.ToList();

            var ingredientRows = await database.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(x =>
                    x.TechnicalCardId.HasValue &&
                    x.IngredientId.HasValue &&
                    allDiscoveredCardIds.Contains(x.TechnicalCardId.Value))
                .Select(x => new
                {
                    TechnicalCardId = x.TechnicalCardId!.Value,
                    IngredientId = x.IngredientId!.Value
                })
                .Distinct()
                .ToListAsync(cancellationToken);

            var directIngredientIdsByCard = ingredientRows
                .GroupBy(x => x.TechnicalCardId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.IngredientId).ToHashSet());

            var ingredientIds = ingredientRows
                .Select(x => x.IngredientId)
                .Distinct()
                .ToList();

            var ingredientNamesById = ingredientIds.Count == 0
                ? new Dictionary<int, string>()
                : await database.Ingredients
                    .AsNoTracking()
                    .Where(x => ingredientIds.Contains(x.Id))
                    .Select(x => new
                    {
                        x.Id,
                        x.Name
                    })
                    .ToDictionaryAsync(
                        x => x.Id,
                        x => NormalizeIngredientName(x.Name),
                        cancellationToken);

            var ingredientsByCardCache = new Dictionary<int, HashSet<int>>();

            HashSet<int> CollectIngredientIds(int technicalCardId, HashSet<int> visited, int depth)
            {
                if (depth > IngredientsGraphMaxDepth)
                {
                    return new HashSet<int>();
                }

                if (!visited.Add(technicalCardId))
                {
                    return new HashSet<int>();
                }

                if (ingredientsByCardCache.TryGetValue(technicalCardId, out var cached))
                {
                    visited.Remove(technicalCardId);
                    return new HashSet<int>(cached);
                }

                var collectedIngredientIds = directIngredientIdsByCard.TryGetValue(technicalCardId, out var directIngredientIds)
                    ? new HashSet<int>(directIngredientIds)
                    : new HashSet<int>();

                if (depth < IngredientsGraphMaxDepth &&
                    childCardsByParent.TryGetValue(technicalCardId, out var childCardIds))
                {
                    foreach (var childCardId in childCardIds)
                    {
                        collectedIngredientIds.UnionWith(CollectIngredientIds(childCardId, visited, depth + 1));
                    }
                }

                ingredientsByCardCache[technicalCardId] = new HashSet<int>(collectedIngredientIds);
                visited.Remove(technicalCardId);

                return collectedIngredientIds;
            }

            var ingredientsTextByCard = new Dictionary<int, string>();

            foreach (var rootCardId in uniqueRootCardIds)
            {
                var ingredientIdsForCard = CollectIngredientIds(
                    rootCardId,
                    new HashSet<int>(),
                    0);

                if (ingredientIdsForCard.Count == 0)
                {
                    ingredientsTextByCard[rootCardId] = "уточняется";
                    continue;
                }

                var ingredientNames = ingredientIdsForCard
                    .Select(id => ingredientNamesById.TryGetValue(id, out var name)
                        ? name
                        : $"ингредиент #{id}")
                    .Select(NormalizeIngredientName)
                    .Where(name => !string.IsNullOrWhiteSpace(name))
                    .Distinct(StringComparer.Create(RussianCulture, ignoreCase: false))
                    .OrderBy(name => name, StringComparer.Create(RussianCulture, ignoreCase: false))
                    .ToList();

                ingredientsTextByCard[rootCardId] = ingredientNames.Count == 0
                    ? "уточняется"
                    : string.Join(", ", ingredientNames);
            }

            return ingredientsTextByCard;
        }

        private static void AddDishWeight(IDictionary<int, decimal> target, int technicalCardId, decimal grams)
        {
            if (grams <= 0)
            {
                return;
            }

            if (target.TryGetValue(technicalCardId, out var current))
            {
                target[technicalCardId] = current + grams;
            }
            else
            {
                target[technicalCardId] = grams;
            }
        }

        private static decimal PickWeight(decimal? outputWeight, decimal? netWeight, decimal? grossWeight)
        {
            if (outputWeight.HasValue && outputWeight.Value > 0)
            {
                return outputWeight.Value;
            }

            if (netWeight.HasValue && netWeight.Value > 0)
            {
                return netWeight.Value;
            }

            if (grossWeight.HasValue && grossWeight.Value > 0)
            {
                return grossWeight.Value;
            }

            return 0m;
        }

        private static decimal ConvertToGrams(decimal value, int? unitOfMeasureId)
        {
            if (value <= 0)
            {
                return 0m;
            }

            return unitOfMeasureId switch
            {
                UnitKilogramsId => value * 1000m,
                UnitLitersId => value * 1000m,
                UnitGramsId => value,
                UnitMillilitersId => value,
                UnitPiecesId => value,
                _ => value
            };
        }

        private static string NormalizeIngredientName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalized = value.Trim().ToLower(RussianCulture).Replace('ё', 'е');
            var tokens = normalized
                .Split(IngredientNameSeparators, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Where(token => !IngredientDescriptorTokens.Contains(token))
                .ToList();

            if (tokens.Count == 0)
            {
                return string.Empty;
            }

            return string.Join(" ", tokens);
        }

        private static string NormalizeVolumeUnit(string unitName)
        {
            var normalized = unitName.Trim();

            return normalized switch
            {
                "Миллилитры" => "мл",
                "Литры" => "л",
                _ => normalized
            };
        }

        private static string? NormalizeCategoryType(string? type)
        {
            var normalized = (type ?? string.Empty).Trim().ToLowerInvariant();
            return normalized switch
            {
                DishesCategoryType => DishesCategoryType,
                DrinksCategoryType => DrinksCategoryType,
                ToppingsCategoryType => ToppingsCategoryType,
                _ => null
            };
        }

        private static int GetCategoryTypeOrder(string type)
        {
            return type switch
            {
                DishesCategoryType => 0,
                DrinksCategoryType => 1,
                ToppingsCategoryType => 2,
                _ => 3
            };
        }

        private static string NormalizeCategoryName(string? name)
        {
            return string.Join(" ", (name ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string? ValidateCategoryName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return "Введите название категории.";
            }

            if (name.Length > 50)
            {
                return "Название категории не должно быть длиннее 50 символов.";
            }

            return null;
        }

        private async Task<bool> CategoryNameExistsAsync(string type, string name, int? exceptId)
        {
            IEnumerable<(int Id, string? Name)> rows;
            switch (type)
            {
                case DishesCategoryType:
                    rows = (await database.DishCategories
                            .AsNoTracking()
                            .Select(c => new { c.Id, c.Name })
                            .ToListAsync())
                        .Select(c => (c.Id, c.Name));
                    break;

                case DrinksCategoryType:
                    rows = (await database.DrinkCategories
                            .AsNoTracking()
                            .Select(c => new { c.Id, c.Name })
                            .ToListAsync())
                        .Select(c => (c.Id, c.Name));
                    break;

                case ToppingsCategoryType:
                    rows = (await database.ToppingCategories
                            .AsNoTracking()
                            .Select(c => new { c.Id, c.Name })
                            .ToListAsync())
                        .Select(c => (c.Id, c.Name));
                    break;

                default:
                    return false;
            }

            var normalizedName = NormalizeCategoryName(name);
            return rows.Any(c =>
                (!exceptId.HasValue || c.Id != exceptId.Value) &&
                string.Equals(
                    NormalizeCategoryName(c.Name),
                    normalizedName,
                    StringComparison.OrdinalIgnoreCase));
        }

    }
}
