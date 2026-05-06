using ClosedXML.Excel;
using GardenNookApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using TransferModels.Reports;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/reports")]
    [Authorize(Roles = "Администратор")]
    public class ReportsController : ControllerBase
    {
        private const int UnitGramsId = 2;
        private const int UnitMillilitersId = 3;
        private const int UnitPiecesId = 4;
        private const int UnitKilogramsId = 5;
        private const int UnitLitersId = 6;
        private const int MilkCategoryId = 10;
        private const int CoffeeCategoryId = 2;
        private const string DishItemType = "dish";
        private const string DrinkItemType = "drink";
        private const string ToppingItemType = "topping";
        private const string IngredientItemType = "ingredient";
        private const string SemiFinishedItemType = "semiFinished";
        private const decimal DecimalEpsilon = 0.000001m;

        private readonly AppDbContext _db;

        public ReportsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<ReportsResponse>> GetReports([FromQuery] string? period = null)
        {
            return Ok(await BuildReportAsync(period));
        }

        [HttpGet("export")]
        public async Task<IActionResult> Export([FromQuery] string? period = null, [FromQuery] string? format = null)
        {
            var report = await BuildReportAsync(period);
            var normalizedFormat = (format ?? string.Empty).Trim().ToLowerInvariant();

            if (normalizedFormat == "xlsx")
            {
                var content = BuildExcel(report);
                return File(
                    content,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    BuildFileName(report, "xlsx"));
            }

            if (normalizedFormat == "pdf")
            {
                var content = BuildPdf(report);
                return File(content, "application/pdf", BuildFileName(report, "pdf"));
            }

            return BadRequest("Выберите формат экспорта: xlsx или pdf.");
        }

        private async Task<ReportsResponse> BuildReportAsync(string? period)
        {
            var resolvedPeriod = ResolvePeriod(period);
            var orderIds = await BuildOrderQuery(resolvedPeriod)
                .Select(x => x.Id)
                .ToListAsync();

            return new ReportsResponse
            {
                Period = resolvedPeriod.Code,
                PeriodName = resolvedPeriod.Name,
                From = resolvedPeriod.From,
                To = resolvedPeriod.To,
                PopularItems = await BuildPopularItemsAsync(orderIds),
                UnpopularItems = await BuildUnpopularItemsAsync(orderIds),
                InventoryItems = await BuildInventoryItemsAsync(orderIds)
            };
        }

        private IQueryable<Order> BuildOrderQuery(ResolvedPeriod period)
        {
            var query = _db.Orders
                .AsNoTracking()
                .Where(x => x.CreatedAt.HasValue);

            if (period.From.HasValue)
            {
                query = query.Where(x => x.CreatedAt >= period.From.Value);
            }

            if (period.To.HasValue)
            {
                query = query.Where(x => x.CreatedAt <= period.To.Value);
            }

            return query;
        }

        private async Task<List<ReportMenuItemDto>> BuildPopularItemsAsync(List<int> orderIds)
        {
            var sold = await LoadSoldMenuItemsAsync(orderIds);
            return sold
                .OrderByDescending(x => x.QuantitySold)
                .ThenByDescending(x => x.Revenue)
                .ThenBy(x => x.Name)
                .Take(3)
                .ToList();
        }

        private async Task<List<ReportMenuItemDto>> BuildUnpopularItemsAsync(List<int> orderIds)
        {
            var soldByKey = (await LoadSoldMenuItemsAsync(orderIds))
                .ToDictionary(x => BuildMenuKey(x.ItemType, x.ItemId), x => x);

            var menuItems = await LoadAllMenuItemsAsync();
            foreach (var item in menuItems)
            {
                var key = BuildMenuKey(item.ItemType, item.ItemId);
                if (!soldByKey.TryGetValue(key, out var sold))
                {
                    continue;
                }

                item.QuantitySold = sold.QuantitySold;
                item.Revenue = sold.Revenue;
            }

            return menuItems
                .OrderBy(x => x.QuantitySold)
                .ThenBy(x => x.Revenue)
                .ThenBy(x => x.Name)
                .Take(3)
                .ToList();
        }

        private async Task<List<ReportMenuItemDto>> LoadSoldMenuItemsAsync(List<int> orderIds)
        {
            if (orderIds.Count == 0)
            {
                return new List<ReportMenuItemDto>();
            }

            var dishRows = await _db.OrderDishItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) && x.DishId.HasValue)
                .GroupBy(x => new
                {
                    ItemId = x.DishId!.Value,
                    Name = x.Dish != null ? x.Dish.Name ?? string.Empty : string.Empty
                })
                .Select(g => new ReportMenuItemDto
                {
                    ItemType = DishItemType,
                    ItemTypeName = "Блюдо",
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    QuantitySold = g.Sum(x => x.Quantity ?? 0m),
                    Revenue = g.Sum(x => x.FinalPrice ?? 0m)
                })
                .ToListAsync();

            var drinkRows = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) && x.DrinkId.HasValue)
                .GroupBy(x => new
                {
                    ItemId = x.DrinkId!.Value,
                    Name = x.Drink != null ? x.Drink.Name ?? string.Empty : string.Empty
                })
                .Select(g => new ReportMenuItemDto
                {
                    ItemType = DrinkItemType,
                    ItemTypeName = "Напиток",
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    QuantitySold = g.Sum(x => x.Quantity ?? 0m),
                    Revenue = g.Sum(x => x.FinalPrice ?? 0m)
                })
                .ToListAsync();

            var standaloneToppings = await _db.OrderToppingItems
                .AsNoTracking()
                .Where(x => orderIds.Contains(x.OrderId))
                .GroupBy(x => new
                {
                    x.ToppingId,
                    Name = x.Topping.Name ?? string.Empty
                })
                .Select(g => new ReportMenuItemDto
                {
                    ItemType = ToppingItemType,
                    ItemTypeName = "Добавка",
                    ItemId = g.Key.ToppingId,
                    Name = g.Key.Name,
                    QuantitySold = g.Sum(x => (decimal)x.Quantity),
                    Revenue = g.Sum(x => x.TotalPrice)
                })
                .ToListAsync();

            var dishToppings = await _db.DishToppings
                .AsNoTracking()
                .Where(x =>
                    x.OrderDishItem != null &&
                    x.OrderDishItem.OrderId.HasValue &&
                    orderIds.Contains(x.OrderDishItem.OrderId.Value) &&
                    x.ToppingId.HasValue)
                .GroupBy(x => new
                {
                    ItemId = x.ToppingId!.Value,
                    Name = x.Topping != null ? x.Topping.Name ?? string.Empty : string.Empty
                })
                .Select(g => new ReportMenuItemDto
                {
                    ItemType = ToppingItemType,
                    ItemTypeName = "Добавка",
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    QuantitySold = g.Sum(x => x.Quantity ?? 0m),
                    Revenue = g.Sum(x => x.FinalPrice ?? 0m)
                })
                .ToListAsync();

            var drinkToppings = await _db.DrinkToppings
                .AsNoTracking()
                .Where(x =>
                    x.OrderDrinkItem != null &&
                    x.OrderDrinkItem.OrderId.HasValue &&
                    orderIds.Contains(x.OrderDrinkItem.OrderId.Value) &&
                    x.ToppingId.HasValue)
                .GroupBy(x => new
                {
                    ItemId = x.ToppingId!.Value,
                    Name = x.Topping != null ? x.Topping.Name ?? string.Empty : string.Empty
                })
                .Select(g => new ReportMenuItemDto
                {
                    ItemType = ToppingItemType,
                    ItemTypeName = "Добавка",
                    ItemId = g.Key.ItemId,
                    Name = g.Key.Name,
                    QuantitySold = g.Sum(x => x.Quantity ?? 0m),
                    Revenue = g.Sum(x => x.FinalPrice ?? 0m)
                })
                .ToListAsync();

            return dishRows
                .Concat(drinkRows)
                .Concat(standaloneToppings)
                .Concat(dishToppings)
                .Concat(drinkToppings)
                .GroupBy(x => BuildMenuKey(x.ItemType, x.ItemId))
                .Select(g =>
                {
                    var first = g.First();
                    return new ReportMenuItemDto
                    {
                        ItemType = first.ItemType,
                        ItemTypeName = first.ItemTypeName,
                        ItemId = first.ItemId,
                        Name = first.Name,
                        QuantitySold = Round2(g.Sum(x => x.QuantitySold)),
                        Revenue = Round2(g.Sum(x => x.Revenue))
                    };
                })
                .ToList();
        }

        private async Task<List<ReportMenuItemDto>> LoadAllMenuItemsAsync()
        {
            var dishes = await _db.Dishes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ReportMenuItemDto
                {
                    ItemType = DishItemType,
                    ItemTypeName = "Блюдо",
                    ItemId = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var drinks = await _db.Drinks
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ReportMenuItemDto
                {
                    ItemType = DrinkItemType,
                    ItemTypeName = "Напиток",
                    ItemId = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            var toppings = await _db.ToppingsAndSyrups
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new ReportMenuItemDto
                {
                    ItemType = ToppingItemType,
                    ItemTypeName = "Добавка",
                    ItemId = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            return dishes.Concat(drinks).Concat(toppings).ToList();
        }

        private async Task<List<InventoryReportItemDto>> BuildInventoryItemsAsync(List<int> orderIds)
        {
            var requirements = await BuildRequirementsAsync(orderIds);

            var ingredientRows = await _db.Ingredients
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    Name = x.Name ?? string.Empty,
                    x.Stock,
                    x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty
                })
                .ToListAsync();

            var ingredientReportRows = ingredientRows.Select(x =>
            {
                requirements.RequiredByIngredients.TryGetValue(x.Id, out var required);
                var actual = ConvertToBaseUnits(ToNonNegative(x.Stock), x.UnitOfMeasureId);

                return new InventoryReportItemDto
                {
                    ItemType = IngredientItemType,
                    ItemTypeName = "Сырье",
                    ItemId = x.Id,
                    Name = x.Name,
                    UnitName = ResolveBaseUnitName(x.UnitOfMeasureId, x.UnitName),
                    ExpectedConsumption = Round2(required),
                    ActualStock = Round2(actual),
                    Difference = Round2(actual - required)
                };
            });

            var semiStocks = await _db.Preparations
                .AsNoTracking()
                .Where(x => x.SemiFinishedId.HasValue)
                .GroupBy(x => x.SemiFinishedId!.Value)
                .Select(g => new
                {
                    SemiFinishedId = g.Key,
                    Stock = g.Sum(x => x.StockGrams ?? 0m)
                })
                .ToDictionaryAsync(x => x.SemiFinishedId, x => x.Stock);

            var semiRows = await _db.SemiFinisheds
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    Name = x.Name ?? string.Empty,
                    x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty
                })
                .ToListAsync();

            var semiReportRows = semiRows.Select(x =>
            {
                requirements.RequiredBySemiFinished.TryGetValue(x.Id, out var required);
                semiStocks.TryGetValue(x.Id, out var actual);

                return new InventoryReportItemDto
                {
                    ItemType = SemiFinishedItemType,
                    ItemTypeName = "Полуфабрикат",
                    ItemId = x.Id,
                    Name = x.Name,
                    UnitName = ResolveBaseUnitName(x.UnitOfMeasureId, x.UnitName),
                    ExpectedConsumption = Round2(required),
                    ActualStock = Round2(actual),
                    Difference = Round2(actual - required)
                };
            });

            return ingredientReportRows
                .Concat(semiReportRows)
                .OrderBy(x => x.ItemTypeName)
                .ThenBy(x => x.Name)
                .ToList();
        }

        private async Task<OrderRequirements> BuildRequirementsAsync(List<int> orderIds)
        {
            var requiredBySemiFinished = new Dictionary<int, decimal>();
            var requiredByIngredients = new Dictionary<int, decimal>();

            if (orderIds.Count == 0)
            {
                return new OrderRequirements(requiredBySemiFinished, requiredByIngredients);
            }

            var dishItems = await _db.OrderDishItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) && x.DishId.HasValue)
                .Select(x => new SoldItem(x.DishId!.Value, x.Quantity ?? 0m))
                .ToListAsync();

            var drinkItems = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) && x.DrinkId.HasValue)
                .Select(x => new SoldDrinkItem(
                    x.Id,
                    x.DrinkId!.Value,
                    x.Quantity ?? 0m,
                    x.OrderDrinkItemModifier != null ? x.OrderDrinkItemModifier.MilkIngredientId : null,
                    x.OrderDrinkItemModifier != null ? x.OrderDrinkItemModifier.CoffeeIngredientId : null))
                .ToListAsync();

            var standaloneToppings = await _db.OrderToppingItems
                .AsNoTracking()
                .Where(x => orderIds.Contains(x.OrderId))
                .Select(x => new SoldItem(x.ToppingId, x.Quantity))
                .ToListAsync();

            var dishToppings = await _db.DishToppings
                .AsNoTracking()
                .Where(x =>
                    x.OrderDishItem != null &&
                    x.OrderDishItem.OrderId.HasValue &&
                    orderIds.Contains(x.OrderDishItem.OrderId.Value) &&
                    x.ToppingId.HasValue)
                .Select(x => new SoldItem(x.ToppingId!.Value, x.Quantity ?? 0m))
                .ToListAsync();

            var drinkToppings = await _db.DrinkToppings
                .AsNoTracking()
                .Where(x =>
                    x.OrderDrinkItem != null &&
                    x.OrderDrinkItem.OrderId.HasValue &&
                    orderIds.Contains(x.OrderDrinkItem.OrderId.Value) &&
                    x.ToppingId.HasValue)
                .Select(x => new SoldItem(x.ToppingId!.Value, x.Quantity ?? 0m))
                .ToListAsync();

            var dishCardById = await LoadDishCardsAsync(dishItems.Select(x => x.ItemId).Distinct().ToList());
            var drinkCardById = await LoadDrinkCardsAsync(drinkItems.Select(x => x.ItemId).Distinct().ToList());
            var toppingIds = standaloneToppings
                .Concat(dishToppings)
                .Concat(drinkToppings)
                .Select(x => x.ItemId)
                .Distinct()
                .ToList();
            var toppingCardById = await LoadToppingCardsAsync(toppingIds);

            var allCardIds = dishCardById.Values
                .Concat(drinkCardById.Values)
                .Concat(toppingCardById.Values)
                .Where(x => x.HasValue)
                .Select(x => x!.Value)
                .Distinct()
                .ToList();

            var semiRequirementsByCard = await LoadSemiRequirementsByCardAsync(allCardIds);
            var ingredientRequirementsByCard = await LoadIngredientRequirementsByCardAsync(allCardIds);
            var ingredientMetadata = await LoadIngredientMetadataAsync(
                ingredientRequirementsByCard.Values.SelectMany(x => x).Select(x => x.IngredientId)
                    .Concat(drinkItems.SelectMany(x => new[] { x.MilkIngredientId, x.CoffeeIngredientId }).Where(x => x.HasValue).Select(x => x!.Value))
                    .Distinct()
                    .ToList());

            foreach (var item in dishItems)
            {
                if (dishCardById.TryGetValue(item.ItemId, out var cardId) && cardId.HasValue)
                {
                    AddCardRequirements(requiredBySemiFinished, semiRequirementsByCard, cardId.Value, item.Quantity);
                    AddIngredientRows(requiredByIngredients, ingredientRequirementsByCard, cardId.Value, item.Quantity);
                }
            }

            foreach (var item in drinkItems)
            {
                if (!drinkCardById.TryGetValue(item.ItemId, out var cardId) || !cardId.HasValue)
                {
                    continue;
                }

                AddCardRequirements(requiredBySemiFinished, semiRequirementsByCard, cardId.Value, item.Quantity);

                if (ingredientRequirementsByCard.TryGetValue(cardId.Value, out var ingredientRows))
                {
                    foreach (var row in ApplyDrinkIngredientModifiers(ingredientRows, item, ingredientMetadata))
                    {
                        AddRequirement(requiredByIngredients, row.IngredientId, row.RequiredBase * item.Quantity);
                    }
                }
            }

            foreach (var item in standaloneToppings.Concat(dishToppings).Concat(drinkToppings))
            {
                if (toppingCardById.TryGetValue(item.ItemId, out var cardId) && cardId.HasValue)
                {
                    AddCardRequirements(requiredBySemiFinished, semiRequirementsByCard, cardId.Value, item.Quantity);
                    AddIngredientRows(requiredByIngredients, ingredientRequirementsByCard, cardId.Value, item.Quantity);
                }
            }

            return new OrderRequirements(requiredBySemiFinished, requiredByIngredients);
        }

        private async Task<Dictionary<int, int?>> LoadDishCardsAsync(List<int> ids)
        {
            return ids.Count == 0
                ? new Dictionary<int, int?>()
                : await _db.Dishes.AsNoTracking().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.TechnicalCardId);
        }

        private async Task<Dictionary<int, int?>> LoadDrinkCardsAsync(List<int> ids)
        {
            return ids.Count == 0
                ? new Dictionary<int, int?>()
                : await _db.Drinks.AsNoTracking().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.TechnicalCardId);
        }

        private async Task<Dictionary<int, int?>> LoadToppingCardsAsync(List<int> ids)
        {
            return ids.Count == 0
                ? new Dictionary<int, int?>()
                : await _db.ToppingsAndSyrups.AsNoTracking().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id, x => x.TechnicalCardId);
        }

        private async Task<Dictionary<int, List<SemiRequirementRow>>> LoadSemiRequirementsByCardAsync(List<int> cardIds)
        {
            if (cardIds.Count == 0)
            {
                return new Dictionary<int, List<SemiRequirementRow>>();
            }

            var rows = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId.HasValue && x.SemiFinishedId.HasValue && cardIds.Contains(x.TechnicalCardId.Value))
                .Select(x => new
                {
                    TechnicalCardId = x.TechnicalCardId!.Value,
                    SemiFinishedId = x.SemiFinishedId!.Value,
                    Required = ConvertToBaseUnits(GetRequiredWeight(x), x.UnitOfMeasureId)
                })
                .ToListAsync();

            return rows
                .Where(x => x.Required > DecimalEpsilon)
                .GroupBy(x => x.TechnicalCardId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.SemiFinishedId)
                        .Select(x => new SemiRequirementRow(x.Key, x.Sum(y => y.Required)))
                        .ToList());
        }

        private async Task<Dictionary<int, List<IngredientRequirementRow>>> LoadIngredientRequirementsByCardAsync(List<int> cardIds)
        {
            if (cardIds.Count == 0)
            {
                return new Dictionary<int, List<IngredientRequirementRow>>();
            }

            var rows = await _db.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId.HasValue && x.IngredientId.HasValue && cardIds.Contains(x.TechnicalCardId.Value))
                .Select(x => new
                {
                    TechnicalCardId = x.TechnicalCardId!.Value,
                    IngredientId = x.IngredientId!.Value,
                    Required = ConvertToBaseUnits(GetRequiredWeight(x), x.UnitOfMeasureId)
                })
                .ToListAsync();

            return rows
                .Where(x => x.Required > DecimalEpsilon)
                .GroupBy(x => x.TechnicalCardId)
                .ToDictionary(
                    g => g.Key,
                    g => g.GroupBy(x => x.IngredientId)
                        .Select(x => new IngredientRequirementRow(x.Key, x.Sum(y => y.Required)))
                        .ToList());
        }

        private async Task<Dictionary<int, IngredientMetadata>> LoadIngredientMetadataAsync(List<int> ingredientIds)
        {
            return ingredientIds.Count == 0
                ? new Dictionary<int, IngredientMetadata>()
                : await _db.Ingredients
                    .AsNoTracking()
                    .Where(x => ingredientIds.Contains(x.Id))
                    .Select(x => new IngredientMetadata(x.Id, x.CategoryId))
                    .ToDictionaryAsync(x => x.Id, x => x);
        }

        private static IEnumerable<IngredientRequirementRow> ApplyDrinkIngredientModifiers(
            List<IngredientRequirementRow> baseRows,
            SoldDrinkItem drink,
            Dictionary<int, IngredientMetadata> ingredientMetadata)
        {
            foreach (var row in baseRows)
            {
                var ingredientId = row.IngredientId;
                if (ingredientMetadata.TryGetValue(row.IngredientId, out var metadata))
                {
                    if (drink.MilkIngredientId.HasValue && metadata.CategoryId == MilkCategoryId)
                    {
                        ingredientId = drink.MilkIngredientId.Value;
                    }
                    else if (drink.CoffeeIngredientId.HasValue && metadata.CategoryId == CoffeeCategoryId)
                    {
                        ingredientId = drink.CoffeeIngredientId.Value;
                    }
                }

                yield return new IngredientRequirementRow(ingredientId, row.RequiredBase);
            }
        }

        private static void AddCardRequirements(
            Dictionary<int, decimal> target,
            Dictionary<int, List<SemiRequirementRow>> requirementsByCard,
            int cardId,
            decimal quantity)
        {
            if (quantity <= DecimalEpsilon || !requirementsByCard.TryGetValue(cardId, out var rows))
            {
                return;
            }

            foreach (var row in rows)
            {
                AddRequirement(target, row.SemiFinishedId, row.RequiredBase * quantity);
            }
        }

        private static void AddIngredientRows(
            Dictionary<int, decimal> target,
            Dictionary<int, List<IngredientRequirementRow>> requirementsByCard,
            int cardId,
            decimal quantity)
        {
            if (quantity <= DecimalEpsilon || !requirementsByCard.TryGetValue(cardId, out var rows))
            {
                return;
            }

            foreach (var row in rows)
            {
                AddRequirement(target, row.IngredientId, row.RequiredBase * quantity);
            }
        }

        private static void AddRequirement(Dictionary<int, decimal> target, int id, decimal quantity)
        {
            if (id <= 0 || quantity <= DecimalEpsilon)
            {
                return;
            }

            target[id] = target.TryGetValue(id, out var current)
                ? current + quantity
                : quantity;
        }

        private static byte[] BuildExcel(ReportsResponse report)
        {
            using var workbook = new XLWorkbook();
            FillMenuSheet(workbook.Worksheets.Add("Популярные"), report.PopularItems, report);
            FillMenuSheet(workbook.Worksheets.Add("Непопулярные"), report.UnpopularItems, report);
            FillInventorySheet(workbook.Worksheets.Add("Инвентаризация"), report);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void FillMenuSheet(IXLWorksheet sheet, List<ReportMenuItemDto> rows, ReportsResponse report)
        {
            sheet.Cell(1, 1).Value = "Отчет";
            sheet.Cell(1, 2).Value = report.PeriodName;
            sheet.Cell(3, 1).Value = "Тип";
            sheet.Cell(3, 2).Value = "Название";
            sheet.Cell(3, 3).Value = "Продано";
            sheet.Cell(3, 4).Value = "Выручка";

            for (var i = 0; i < rows.Count; i++)
            {
                var row = rows[i];
                var excelRow = i + 4;
                sheet.Cell(excelRow, 1).Value = row.ItemTypeName;
                sheet.Cell(excelRow, 2).Value = row.Name;
                sheet.Cell(excelRow, 3).Value = row.QuantitySold;
                sheet.Cell(excelRow, 4).Value = row.Revenue;
            }

            sheet.Range(3, 1, 3, 4).Style.Font.Bold = true;
            sheet.Columns().AdjustToContents();
        }

        private static void FillInventorySheet(IXLWorksheet sheet, ReportsResponse report)
        {
            sheet.Cell(1, 1).Value = "Отчет";
            sheet.Cell(1, 2).Value = report.PeriodName;
            sheet.Cell(3, 1).Value = "Тип";
            sheet.Cell(3, 2).Value = "Название";
            sheet.Cell(3, 3).Value = "Ед. изм.";
            sheet.Cell(3, 4).Value = "Расход";
            sheet.Cell(3, 5).Value = "Факт";
            sheet.Cell(3, 6).Value = "Разница";

            for (var i = 0; i < report.InventoryItems.Count; i++)
            {
                var row = report.InventoryItems[i];
                var excelRow = i + 4;
                sheet.Cell(excelRow, 1).Value = row.ItemTypeName;
                sheet.Cell(excelRow, 2).Value = row.Name;
                sheet.Cell(excelRow, 3).Value = row.UnitName;
                sheet.Cell(excelRow, 4).Value = row.ExpectedConsumption;
                sheet.Cell(excelRow, 5).Value = row.ActualStock;
                sheet.Cell(excelRow, 6).Value = row.Difference;
            }

            sheet.Range(3, 1, 3, 6).Style.Font.Bold = true;
            sheet.Columns().AdjustToContents();
        }

        private static byte[] BuildPdf(ReportsResponse report)
        {
            using var document = new PdfDocument();
            document.Info.Title = "Отчеты GardenNook";

            var font = new XFont("Arial", 10, XFontStyle.Regular);
            var boldFont = new XFont("Arial", 12, XFontStyle.Bold);
            var titleFont = new XFont("Arial", 18, XFontStyle.Bold);
            var page = document.AddPage();
            var gfx = XGraphics.FromPdfPage(page);
            var y = 36d;

            DrawText(gfx, "Отчеты GardenNook", titleFont, 36, ref y);
            DrawText(gfx, "Период: " + report.PeriodName, boldFont, 36, ref y);
            y += 10;

            DrawMenuSection(gfx, "3 самые популярные позиции", report.PopularItems, boldFont, font, ref y, page, document);
            DrawMenuSection(gfx, "3 самые непопулярные позиции", report.UnpopularItems, boldFont, font, ref y, page, document);
            DrawInventorySection(gfx, report.InventoryItems.Take(40).ToList(), boldFont, font, ref y, page, document);

            using var stream = new MemoryStream();
            document.Save(stream, false);
            return stream.ToArray();
        }

        private static void DrawMenuSection(
            XGraphics gfx,
            string title,
            List<ReportMenuItemDto> rows,
            XFont boldFont,
            XFont font,
            ref double y,
            PdfPage page,
            PdfDocument document)
        {
            EnsurePdfSpace(ref gfx, ref page, document, ref y, 95);
            DrawText(gfx, title, boldFont, 36, ref y);
            DrawText(gfx, "Тип | Название | Продано | Выручка", font, 36, ref y);

            foreach (var row in rows)
            {
                var line = $"{row.ItemTypeName} | {row.Name} | {FormatDecimal(row.QuantitySold)} | {FormatDecimal(row.Revenue)} руб.";
                DrawText(gfx, line, font, 36, ref y);
                var barWidth = Math.Min(280, (double)row.QuantitySold * 18);
                gfx.DrawRectangle(XBrushes.DarkSeaGreen, 36, y - 10, barWidth, 7);
                y += 6;
            }

            y += 10;
        }

        private static void DrawInventorySection(
            XGraphics gfx,
            List<InventoryReportItemDto> rows,
            XFont boldFont,
            XFont font,
            ref double y,
            PdfPage page,
            PdfDocument document)
        {
            EnsurePdfSpace(ref gfx, ref page, document, ref y, 70);
            DrawText(gfx, "Инвентаризация", boldFont, 36, ref y);
            DrawText(gfx, "Тип | Название | Расход | Факт | Разница", font, 36, ref y);

            foreach (var row in rows)
            {
                EnsurePdfSpace(ref gfx, ref page, document, ref y, 25);
                var line = $"{row.ItemTypeName} | {TrimForPdf(row.Name)} | {FormatDecimal(row.ExpectedConsumption)} | {FormatDecimal(row.ActualStock)} | {FormatDecimal(row.Difference)} {row.UnitName}";
                DrawText(gfx, line, font, 36, ref y);
                var width = Math.Min(180, Math.Abs((double)row.Difference) * 0.5);
                var brush = row.Difference < 0 ? XBrushes.IndianRed : XBrushes.DarkSeaGreen;
                gfx.DrawRectangle(brush, 36, y - 10, width, 6);
                y += 4;
            }
        }

        private static void DrawText(XGraphics gfx, string text, XFont font, double x, ref double y)
        {
            gfx.DrawString(text, font, XBrushes.Black, new XRect(x, y, 520, 18), XStringFormats.TopLeft);
            y += 18;
        }

        private static void EnsurePdfSpace(ref XGraphics gfx, ref PdfPage page, PdfDocument document, ref double y, double needed)
        {
            if (y + needed <= page.Height - 36)
            {
                return;
            }

            page = document.AddPage();
            gfx = XGraphics.FromPdfPage(page);
            y = 36;
        }

        private static string BuildFileName(ReportsResponse report, string extension)
        {
            var period = report.PeriodName
                .Replace(" ", "_")
                .Replace("/", "_")
                .Replace("\\", "_");
            return $"Отчет_{period}.{extension}";
        }

        private static ResolvedPeriod ResolvePeriod(string? period)
        {
            var now = DateTime.Now;
            var normalized = (period ?? string.Empty).Trim();

            return normalized switch
            {
                "month" => new ResolvedPeriod("month", "Месяц", now.AddMonths(-1), now),
                "threeMonths" => new ResolvedPeriod("threeMonths", "3 месяца", now.AddMonths(-3), now),
                "halfYear" => new ResolvedPeriod("halfYear", "Полгода", now.AddMonths(-6), now),
                "allTime" => new ResolvedPeriod("allTime", "Все время", null, now),
                _ => new ResolvedPeriod("week", "Неделя", now.AddDays(-7), now)
            };
        }

        private static decimal GetRequiredWeight(TechnicalCardIngredientComposition row)
        {
            if (row.OutputWeight.HasValue && row.OutputWeight.Value > 0)
            {
                return row.OutputWeight.Value;
            }

            if (row.NetWeight.HasValue && row.NetWeight.Value > 0)
            {
                return row.NetWeight.Value;
            }

            return row.GrossWeight.HasValue && row.GrossWeight.Value > 0 ? row.GrossWeight.Value : 0m;
        }

        private static decimal GetRequiredWeight(TechnicalCardSemiFinishedComposition row)
        {
            if (row.OutputWeight.HasValue && row.OutputWeight.Value > 0)
            {
                return row.OutputWeight.Value;
            }

            if (row.NetWeight.HasValue && row.NetWeight.Value > 0)
            {
                return row.NetWeight.Value;
            }

            return row.GrossWeight.HasValue && row.GrossWeight.Value > 0 ? row.GrossWeight.Value : 0m;
        }

        private static decimal ConvertToBaseUnits(decimal value, int? unitOfMeasureId)
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

        private static string ResolveBaseUnitName(int? unitOfMeasureId, string unitName)
        {
            return unitOfMeasureId switch
            {
                UnitKilogramsId => "г",
                UnitLitersId => "мл",
                UnitGramsId => "г",
                UnitMillilitersId => "мл",
                UnitPiecesId => "шт.",
                _ => string.IsNullOrWhiteSpace(unitName) ? "ед." : unitName
            };
        }

        private static decimal ToNonNegative(decimal? value)
            => value.HasValue && value.Value > 0 ? value.Value : 0m;

        private static decimal Round2(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);

        private static string FormatDecimal(decimal value)
            => value.ToString("0.##");

        private static string BuildMenuKey(string itemType, int itemId)
            => $"{itemType}:{itemId}";

        private static string TrimForPdf(string value)
            => value.Length <= 34 ? value : value.Substring(0, 31) + "...";

        private sealed record ResolvedPeriod(string Code, string Name, DateTime? From, DateTime? To);

        private sealed record SoldItem(int ItemId, decimal Quantity);

        private sealed record SoldDrinkItem(int OrderDrinkItemId, int ItemId, decimal Quantity, int? MilkIngredientId, int? CoffeeIngredientId);

        private sealed record OrderRequirements(Dictionary<int, decimal> RequiredBySemiFinished, Dictionary<int, decimal> RequiredByIngredients);

        private sealed record SemiRequirementRow(int SemiFinishedId, decimal RequiredBase);

        private sealed record IngredientRequirementRow(int IngredientId, decimal RequiredBase);

        private sealed record IngredientMetadata(int Id, int? CategoryId);
    }
}
