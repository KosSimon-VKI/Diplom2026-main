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
        private const decimal DecimalEpsilon = 0.000001m;

        private readonly AppDbContext _db;
        private readonly Dictionary<int, List<CardIngredientRow>> _ingredientRowsByCard = new Dictionary<int, List<CardIngredientRow>>();
        private readonly Dictionary<int, List<CardSemiFinishedRow>> _semiFinishedRowsByCard = new Dictionary<int, List<CardSemiFinishedRow>>();
        private readonly Dictionary<int, int?> _semiFinishedCardById = new Dictionary<int, int?>();
        private readonly Dictionary<int, decimal> _technicalCardOutputById = new Dictionary<int, decimal>();
        private readonly Dictionary<int, IngredientMetadata> _ingredientMetadataById = new Dictionary<int, IngredientMetadata>();

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

            var warnings = new List<string>();

            return new ReportsResponse
            {
                Period = resolvedPeriod.Code,
                PeriodName = resolvedPeriod.Name,
                From = resolvedPeriod.From,
                To = resolvedPeriod.To,
                GeneratedAt = DateTime.Now,
                GeneratedBy = User?.Identity?.Name ?? "Администратор",
                Warnings = warnings,
                PopularItems = await BuildPopularItemsAsync(orderIds),
                UnpopularItems = await BuildUnpopularItemsAsync(orderIds),
                AbcItems = await BuildAbcItemsAsync(orderIds),
                InventoryItems = await BuildInventoryItemsAsync(orderIds, resolvedPeriod, warnings)
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

        private IQueryable<WriteOffAct> BuildWriteOffQuery(ResolvedPeriod period)
        {
            var query = _db.WriteOffActs.AsNoTracking();

            if (period.From.HasValue)
            {
                query = query.Where(x => x.Date >= period.From.Value);
            }

            if (period.To.HasValue)
            {
                query = query.Where(x => x.Date <= period.To.Value);
            }

            return query;
        }

        private IQueryable<Preparation> BuildPreparationQuery(ResolvedPeriod period)
        {
            var query = _db.Preparations
                .AsNoTracking()
                .Where(x => x.ProductionDate.HasValue && x.SemiFinishedId.HasValue && x.StockGrams.HasValue && x.StockGrams.Value > 0m);

            if (period.From.HasValue)
            {
                var fromDate = DateOnly.FromDateTime(period.From.Value.Date);
                query = query.Where(x => x.ProductionDate >= fromDate);
            }

            if (period.To.HasValue)
            {
                var toDate = DateOnly.FromDateTime(period.To.Value.Date);
                query = query.Where(x => x.ProductionDate <= toDate);
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

        private async Task<List<AbcReportItemDto>> BuildAbcItemsAsync(List<int> orderIds)
        {
            var soldItems = await LoadSoldMenuItemsAsync(orderIds);
            var totalRevenue = soldItems.Sum(x => x.Revenue);
            var cumulativeShare = 0m;
            var place = 0;

            return soldItems
                .OrderByDescending(x => x.Revenue)
                .ThenByDescending(x => x.QuantitySold)
                .ThenBy(x => x.Name)
                .Select(x =>
                {
                    place++;
                    var share = totalRevenue > DecimalEpsilon
                        ? x.Revenue / totalRevenue * 100m
                        : 0m;
                    cumulativeShare += share;

                    return new AbcReportItemDto
                    {
                        ItemType = x.ItemType,
                        ItemTypeName = x.ItemTypeName,
                        ItemId = x.ItemId,
                        Name = x.Name,
                        QuantitySold = Round2(x.QuantitySold),
                        Revenue = Round2(x.Revenue),
                        RevenueSharePercent = Round2(share),
                        CumulativeSharePercent = Round2(cumulativeShare),
                        Group = place.ToString()
                    };
                })
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

        private async Task<List<InventoryReportItemDto>> BuildInventoryItemsAsync(
            List<int> orderIds,
            ResolvedPeriod period,
            List<string> warnings)
        {
            var orderConsumption = new Dictionary<int, decimal>();
            var writeOffConsumption = new Dictionary<int, decimal>();
            var preparationConsumption = new Dictionary<int, decimal>();
            var detailRowsByIngredient = new Dictionary<int, List<InventoryDetailDto>>();

            await AddOrderIngredientConsumptionAsync(orderIds, orderConsumption, detailRowsByIngredient, warnings);
            await AddWriteOffIngredientConsumptionAsync(period, writeOffConsumption, detailRowsByIngredient, warnings);
            await AddPreparationIngredientConsumptionAsync(period, preparationConsumption, detailRowsByIngredient, warnings);

            var ingredientRows = await _db.Ingredients
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    Name = x.Name ?? string.Empty,
                    x.Stock,
                    CostRub = x.CostRub ?? 0m,
                    x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null ? x.UnitOfMeasure.Name ?? string.Empty : string.Empty
                })
                .ToListAsync();

            return ingredientRows.Select(x =>
                {
                    orderConsumption.TryGetValue(x.Id, out var orderRequired);
                    writeOffConsumption.TryGetValue(x.Id, out var writeOffRequired);
                    preparationConsumption.TryGetValue(x.Id, out var preparationRequired);
                    var totalRequired = orderRequired + writeOffRequired + preparationRequired;
                    var actual = ConvertToBaseUnits(ToNonNegative(x.Stock), x.UnitOfMeasureId);
                    var difference = actual - totalRequired;
                    var differenceForCost = ConvertFromBaseUnits(difference, x.UnitOfMeasureId);
                    var unitName = ResolveBaseUnitName(x.UnitOfMeasureId, x.UnitName);
                    detailRowsByIngredient.TryGetValue(x.Id, out var detailRows);

                    return new InventoryReportItemDto
                    {
                        ItemType = IngredientItemType,
                        ItemTypeName = "Сырье",
                        ItemId = x.Id,
                        Name = x.Name,
                        UnitName = unitName,
                        OrderConsumption = Round2(orderRequired),
                        WriteOffConsumption = Round2(writeOffRequired),
                        PreparationConsumption = Round2(preparationRequired),
                        ExpectedConsumption = Round2(totalRequired),
                        ActualStock = Round2(actual),
                        Difference = Round2(difference),
                        UnitCostRub = Round2(x.CostRub),
                        DifferenceCostRub = Round2(differenceForCost * x.CostRub),
                        Details = (detailRows ?? new List<InventoryDetailDto>())
                            .Select(d => new InventoryDetailDto
                            {
                                SourceType = d.SourceType,
                                SourceName = d.SourceName,
                                SourceDate = d.SourceDate,
                                Quantity = Round2(d.Quantity),
                                UnitName = unitName
                            })
                            .OrderBy(d => d.SourceType)
                            .ThenBy(d => d.SourceDate)
                            .ThenBy(d => d.SourceName)
                            .ToList()
                    };
                })
                .ToList();
        }

        private async Task AddOrderIngredientConsumptionAsync(
            List<int> orderIds,
            Dictionary<int, decimal> target,
            Dictionary<int, List<InventoryDetailDto>> detailRowsByIngredient,
            List<string> warnings)
        {
            if (orderIds.Count == 0)
            {
                return;
            }

            var dishItems = await _db.OrderDishItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) && x.DishId.HasValue)
                .Select(x => new SoldItem(
                    x.DishId!.Value,
                    x.Quantity ?? 0m,
                    "Заказ #" + x.OrderId!.Value + ": " + (x.Dish != null ? x.Dish.Name ?? string.Empty : string.Empty),
                    x.Order != null ? x.Order.CreatedAt : null))
                .ToListAsync();

            var drinkItems = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && orderIds.Contains(x.OrderId.Value) && x.DrinkId.HasValue)
                .Select(x => new SoldDrinkItem(
                    x.DrinkId!.Value,
                    x.Quantity ?? 0m,
                    x.OrderDrinkItemModifier != null ? x.OrderDrinkItemModifier.MilkIngredientId : null,
                    x.OrderDrinkItemModifier != null ? x.OrderDrinkItemModifier.CoffeeIngredientId : null,
                    "Заказ #" + x.OrderId!.Value + ": " + (x.Drink != null ? x.Drink.Name ?? string.Empty : string.Empty),
                    x.Order != null ? x.Order.CreatedAt : null))
                .ToListAsync();

            var standaloneToppings = await _db.OrderToppingItems
                .AsNoTracking()
                .Where(x => orderIds.Contains(x.OrderId))
                .Select(x => new SoldItem(
                    x.ToppingId,
                    x.Quantity,
                    "Заказ #" + x.OrderId + ": " + (x.Topping != null ? x.Topping.Name ?? string.Empty : string.Empty),
                    x.Order != null ? x.Order.CreatedAt : null))
                .ToListAsync();

            var dishToppings = await _db.DishToppings
                .AsNoTracking()
                .Where(x =>
                    x.OrderDishItem != null &&
                    x.OrderDishItem.OrderId.HasValue &&
                    orderIds.Contains(x.OrderDishItem.OrderId.Value) &&
                    x.ToppingId.HasValue)
                .Select(x => new SoldItem(
                    x.ToppingId!.Value,
                    x.Quantity ?? 0m,
                    "Заказ #" + x.OrderDishItem!.OrderId!.Value + ": " + (x.Topping != null ? x.Topping.Name ?? string.Empty : string.Empty),
                    x.OrderDishItem.Order != null ? x.OrderDishItem.Order.CreatedAt : null))
                .ToListAsync();

            var drinkToppings = await _db.DrinkToppings
                .AsNoTracking()
                .Where(x =>
                    x.OrderDrinkItem != null &&
                    x.OrderDrinkItem.OrderId.HasValue &&
                    orderIds.Contains(x.OrderDrinkItem.OrderId.Value) &&
                    x.ToppingId.HasValue)
                .Select(x => new SoldItem(
                    x.ToppingId!.Value,
                    x.Quantity ?? 0m,
                    "Заказ #" + x.OrderDrinkItem!.OrderId!.Value + ": " + (x.Topping != null ? x.Topping.Name ?? string.Empty : string.Empty),
                    x.OrderDrinkItem.Order != null ? x.OrderDrinkItem.Order.CreatedAt : null))
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

            foreach (var item in dishItems)
            {
                if (dishCardById.TryGetValue(item.ItemId, out var cardId) && cardId.HasValue)
                {
                    await AddTechnicalCardIngredientsAsync(cardId.Value, item.Quantity, target, detailRowsByIngredient, warnings, new HashSet<int>(), null, new ConsumptionSource("Заказы", item.SourceName, item.SourceDate));
                }
            }

            foreach (var item in drinkItems)
            {
                if (drinkCardById.TryGetValue(item.ItemId, out var cardId) && cardId.HasValue)
                {
                    await AddTechnicalCardIngredientsAsync(
                        cardId.Value,
                        item.Quantity,
                        target,
                        detailRowsByIngredient,
                        warnings,
                        new HashSet<int>(),
                        new DrinkModifier(item.MilkIngredientId, item.CoffeeIngredientId),
                        new ConsumptionSource("Заказы", item.SourceName, item.SourceDate));
                }
            }

            foreach (var item in standaloneToppings.Concat(dishToppings).Concat(drinkToppings))
            {
                if (toppingCardById.TryGetValue(item.ItemId, out var cardId) && cardId.HasValue)
                {
                    await AddTechnicalCardIngredientsAsync(cardId.Value, item.Quantity, target, detailRowsByIngredient, warnings, new HashSet<int>(), null, new ConsumptionSource("Заказы", item.SourceName, item.SourceDate));
                }
            }
        }

        private async Task AddWriteOffIngredientConsumptionAsync(
            ResolvedPeriod period,
            Dictionary<int, decimal> target,
            Dictionary<int, List<InventoryDetailDto>> detailRowsByIngredient,
            List<string> warnings)
        {
            var directIngredientRows = await BuildWriteOffQuery(period)
                .SelectMany(x => x.IngredientItems)
                .Select(x => new
                {
                    x.IngredientId,
                    x.Quantity,
                    x.UnitOfMeasureId,
                    ActId = x.WriteOffActId,
                    SourceDate = (DateTime?)x.WriteOffAct.Date,
                    TypeName = x.WriteOffType != null ? x.WriteOffType.Name ?? string.Empty : string.Empty
                })
                .ToListAsync();

            foreach (var row in directIngredientRows)
            {
                AddRequirement(
                    target,
                    detailRowsByIngredient,
                    row.IngredientId,
                    ConvertToBaseUnits(row.Quantity, row.UnitOfMeasureId),
                    new ConsumptionSource("Списания", "Акт #" + row.ActId + (string.IsNullOrWhiteSpace(row.TypeName) ? string.Empty : ": " + row.TypeName), row.SourceDate));
            }

            var semiRows = await BuildWriteOffQuery(period)
                .SelectMany(x => x.SemiFinishedItems)
                .Select(x => new
                {
                    x.SemiFinishedId,
                    x.Quantity,
                    x.UnitOfMeasureId,
                    ActId = x.WriteOffActId,
                    SourceDate = (DateTime?)x.WriteOffAct.Date,
                    SemiFinishedName = x.SemiFinished != null ? x.SemiFinished.Name ?? string.Empty : string.Empty
                })
                .ToListAsync();

            foreach (var row in semiRows)
            {
                var cardId = await ResolveSemiFinishedTechnicalCardIdAsync(row.SemiFinishedId, warnings);
                if (!cardId.HasValue)
                {
                    continue;
                }

                var quantityBase = ConvertToBaseUnits(row.Quantity, row.UnitOfMeasureId);
                await AddSemiFinishedTechnicalCardIngredientsByWeightAsync(
                    cardId.Value,
                    quantityBase,
                    target,
                    detailRowsByIngredient,
                    warnings,
                    new HashSet<int>(),
                    new ConsumptionSource("Списания", "Акт #" + row.ActId + ": " + row.SemiFinishedName, row.SourceDate));
            }
        }

        private async Task AddPreparationIngredientConsumptionAsync(
            ResolvedPeriod period,
            Dictionary<int, decimal> target,
            Dictionary<int, List<InventoryDetailDto>> detailRowsByIngredient,
            List<string> warnings)
        {
            var preparations = await BuildPreparationQuery(period)
                .Select(x => new
                {
                    SemiFinishedId = x.SemiFinishedId!.Value,
                    StockGrams = x.StockGrams!.Value,
                    PreparationId = x.Id,
                    SourceDate = x.ProductionDate.HasValue ? (DateTime?)x.ProductionDate.Value.ToDateTime(TimeOnly.MinValue) : null,
                    SemiFinishedName = x.SemiFinished != null ? x.SemiFinished.Name ?? string.Empty : string.Empty
                })
                .ToListAsync();

            foreach (var preparation in preparations)
            {
                var cardId = await ResolveSemiFinishedTechnicalCardIdAsync(preparation.SemiFinishedId, warnings);
                if (!cardId.HasValue)
                {
                    continue;
                }

                await AddSemiFinishedTechnicalCardIngredientsByWeightAsync(
                    cardId.Value,
                    preparation.StockGrams,
                    target,
                    detailRowsByIngredient,
                    warnings,
                    new HashSet<int>(),
                    new ConsumptionSource("Заготовки", "Заготовка #" + preparation.PreparationId + ": " + preparation.SemiFinishedName, preparation.SourceDate));
            }
        }

        private async Task AddSemiFinishedTechnicalCardIngredientsByWeightAsync(
            int technicalCardId,
            decimal requiredBaseWeight,
            Dictionary<int, decimal> target,
            Dictionary<int, List<InventoryDetailDto>> detailRowsByIngredient,
            List<string> warnings,
            HashSet<int> visitedTechnicalCardIds,
            ConsumptionSource source)
        {
            if (requiredBaseWeight <= DecimalEpsilon)
            {
                return;
            }

            var outputBaseWeight = await LoadTechnicalCardOutputBaseAsync(technicalCardId);
            if (outputBaseWeight <= DecimalEpsilon)
            {
                AddWarning(warnings, $"У техкарты #{technicalCardId} не найден выход. Расход сырья по полуфабрикату не рассчитан.");
                return;
            }

            await AddTechnicalCardIngredientsAsync(
                technicalCardId,
                requiredBaseWeight / outputBaseWeight,
                target,
                detailRowsByIngredient,
                warnings,
                visitedTechnicalCardIds,
                null,
                source);
        }

        private async Task AddTechnicalCardIngredientsAsync(
            int technicalCardId,
            decimal multiplier,
            Dictionary<int, decimal> target,
            Dictionary<int, List<InventoryDetailDto>> detailRowsByIngredient,
            List<string> warnings,
            HashSet<int> visitedTechnicalCardIds,
            DrinkModifier? drinkModifier,
            ConsumptionSource source)
        {
            if (multiplier <= DecimalEpsilon)
            {
                return;
            }

            if (!visitedTechnicalCardIds.Add(technicalCardId))
            {
                AddWarning(warnings, $"Найдена циклическая техкарта #{technicalCardId}. Вложенная ветка пропущена.");
                return;
            }

            var ingredientRows = await LoadIngredientRowsByCardAsync(technicalCardId);
            foreach (var row in ApplyDrinkIngredientModifiers(ingredientRows, drinkModifier))
            {
                AddRequirement(target, detailRowsByIngredient, row.IngredientId, row.RequiredBase * multiplier, source);
            }

            var semiFinishedRows = await LoadSemiFinishedRowsByCardAsync(technicalCardId);
            foreach (var row in semiFinishedRows)
            {
                var cardId = await ResolveSemiFinishedTechnicalCardIdAsync(row.SemiFinishedId, warnings);
                if (!cardId.HasValue)
                {
                    continue;
                }

                await AddSemiFinishedTechnicalCardIngredientsByWeightAsync(
                    cardId.Value,
                    row.RequiredBase * multiplier,
                    target,
                    detailRowsByIngredient,
                    warnings,
                    new HashSet<int>(visitedTechnicalCardIds),
                    source);
            }
        }

        private async Task<decimal> LoadTechnicalCardOutputBaseAsync(int technicalCardId)
        {
            if (_technicalCardOutputById.TryGetValue(technicalCardId, out var cachedOutput))
            {
                return cachedOutput;
            }

            var ingredientOutput = await _db.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => ConvertToBaseUnits(GetRequiredWeight(x), x.UnitOfMeasureId))
                .ToListAsync();

            var semiFinishedOutput = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => ConvertToBaseUnits(GetRequiredWeight(x), x.UnitOfMeasureId))
                .ToListAsync();

            var output = ingredientOutput.Sum() + semiFinishedOutput.Sum();
            _technicalCardOutputById[technicalCardId] = output;
            return output;
        }

        private async Task<List<CardIngredientRow>> LoadIngredientRowsByCardAsync(int technicalCardId)
        {
            if (_ingredientRowsByCard.TryGetValue(technicalCardId, out var cachedRows))
            {
                return cachedRows;
            }

            var rows = await _db.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId && x.IngredientId.HasValue)
                .Select(x => new CardIngredientRow(
                    x.IngredientId!.Value,
                    ConvertToBaseUnits(GetRequiredWeight(x), x.UnitOfMeasureId)))
                .ToListAsync();

            rows = rows
                .Where(x => x.RequiredBase > DecimalEpsilon)
                .GroupBy(x => x.IngredientId)
                .Select(x => new CardIngredientRow(x.Key, x.Sum(y => y.RequiredBase)))
                .ToList();

            _ingredientRowsByCard[technicalCardId] = rows;
            return rows;
        }

        private async Task<List<CardSemiFinishedRow>> LoadSemiFinishedRowsByCardAsync(int technicalCardId)
        {
            if (_semiFinishedRowsByCard.TryGetValue(technicalCardId, out var cachedRows))
            {
                return cachedRows;
            }

            var rows = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId && x.SemiFinishedId.HasValue)
                .Select(x => new CardSemiFinishedRow(
                    x.SemiFinishedId!.Value,
                    ConvertToBaseUnits(GetRequiredWeight(x), x.UnitOfMeasureId)))
                .ToListAsync();

            rows = rows
                .Where(x => x.RequiredBase > DecimalEpsilon)
                .GroupBy(x => x.SemiFinishedId)
                .Select(x => new CardSemiFinishedRow(x.Key, x.Sum(y => y.RequiredBase)))
                .ToList();

            _semiFinishedRowsByCard[technicalCardId] = rows;
            return rows;
        }

        private async Task<int?> ResolveSemiFinishedTechnicalCardIdAsync(int semiFinishedId, List<string> warnings)
        {
            if (!_semiFinishedCardById.TryGetValue(semiFinishedId, out var cardId))
            {
                cardId = await _db.SemiFinisheds
                    .AsNoTracking()
                    .Where(x => x.Id == semiFinishedId)
                    .Select(x => x.TechnicalCardId)
                    .FirstOrDefaultAsync();

                _semiFinishedCardById[semiFinishedId] = cardId;
            }

            if (!cardId.HasValue)
            {
                AddWarning(warnings, $"У полуфабриката #{semiFinishedId} не указана техкарта. Расход сырья по нему не рассчитан.");
            }

            return cardId;
        }

        private IEnumerable<CardIngredientRow> ApplyDrinkIngredientModifiers(
            List<CardIngredientRow> baseRows,
            DrinkModifier? modifier)
        {
            foreach (var row in baseRows)
            {
                var ingredientId = row.IngredientId;
                if (modifier != null)
                {
                    var metadata = LoadIngredientMetadata(row.IngredientId);
                    if (modifier.MilkIngredientId.HasValue && metadata.CategoryId == MilkCategoryId)
                    {
                        ingredientId = modifier.MilkIngredientId.Value;
                    }
                    else if (modifier.CoffeeIngredientId.HasValue && metadata.CategoryId == CoffeeCategoryId)
                    {
                        ingredientId = modifier.CoffeeIngredientId.Value;
                    }
                }

                yield return new CardIngredientRow(ingredientId, row.RequiredBase);
            }
        }

        private IngredientMetadata LoadIngredientMetadata(int ingredientId)
        {
            if (_ingredientMetadataById.TryGetValue(ingredientId, out var metadata))
            {
                return metadata;
            }

            metadata = _db.Ingredients
                .AsNoTracking()
                .Where(x => x.Id == ingredientId)
                .Select(x => new IngredientMetadata(x.Id, x.CategoryId))
                .FirstOrDefault() ?? new IngredientMetadata(ingredientId, null);

            _ingredientMetadataById[ingredientId] = metadata;
            return metadata;
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

        private static void AddRequirement(
            Dictionary<int, decimal> target,
            Dictionary<int, List<InventoryDetailDto>> detailRowsByIngredient,
            int id,
            decimal quantity,
            ConsumptionSource source)
        {
            AddRequirement(target, id, quantity);

            if (id <= 0 || quantity <= DecimalEpsilon)
            {
                return;
            }

            if (!detailRowsByIngredient.TryGetValue(id, out var rows))
            {
                rows = new List<InventoryDetailDto>();
                detailRowsByIngredient[id] = rows;
            }

            rows.Add(new InventoryDetailDto
            {
                SourceType = source.SourceType,
                SourceName = source.SourceName,
                SourceDate = source.SourceDate,
                Quantity = quantity
            });
        }

        private static void AddWarning(List<string> warnings, string warning)
        {
            if (!warnings.Contains(warning))
            {
                warnings.Add(warning);
            }
        }

        private static byte[] BuildExcel(ReportsResponse report)
        {
            using var workbook = new XLWorkbook();
            FillMenuSheet(workbook.Worksheets.Add("Популярные"), report.PopularItems, report);
            FillMenuSheet(workbook.Worksheets.Add("Непопулярные"), report.UnpopularItems, report);
            FillAbcSheet(workbook.Worksheets.Add("ABC-анализ"), report);
            FillInventorySheet(workbook.Worksheets.Add("Инвентаризация"), report);
            FillInventoryDetailsSheet(workbook.Worksheets.Add("Расшифровка"), report);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static void FillMenuSheet(IXLWorksheet sheet, List<ReportMenuItemDto> rows, ReportsResponse report)
        {
            sheet.Cell(1, 1).Value = "Отчет";
            sheet.Cell(1, 2).Value = report.PeriodName;
            sheet.Cell(2, 1).Value = "Сформирован";
            sheet.Cell(2, 2).Value = report.GeneratedAt;
            sheet.Cell(2, 3).Value = "Пользователь";
            sheet.Cell(2, 4).Value = report.GeneratedBy;
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

        private static void FillAbcSheet(IXLWorksheet sheet, ReportsResponse report)
        {
            sheet.Cell(1, 1).Value = "Отчет";
            sheet.Cell(1, 2).Value = report.PeriodName;
            sheet.Cell(2, 1).Value = "Сформирован";
            sheet.Cell(2, 2).Value = report.GeneratedAt;
            sheet.Cell(2, 3).Value = "Пользователь";
            sheet.Cell(2, 4).Value = report.GeneratedBy;
            sheet.Cell(3, 1).Value = "Место";
            sheet.Cell(3, 2).Value = "Тип";
            sheet.Cell(3, 3).Value = "Название";
            sheet.Cell(3, 4).Value = "Продано";
            sheet.Cell(3, 5).Value = "Выручка";
            sheet.Cell(3, 6).Value = "Доля, %";
            sheet.Cell(3, 7).Value = "Накопительно, %";

            for (var i = 0; i < report.AbcItems.Count; i++)
            {
                var row = report.AbcItems[i];
                var excelRow = i + 4;
                sheet.Cell(excelRow, 1).Value = row.Group;
                sheet.Cell(excelRow, 2).Value = row.ItemTypeName;
                sheet.Cell(excelRow, 3).Value = row.Name;
                sheet.Cell(excelRow, 4).Value = row.QuantitySold;
                sheet.Cell(excelRow, 5).Value = row.Revenue;
                sheet.Cell(excelRow, 6).Value = row.RevenueSharePercent;
                sheet.Cell(excelRow, 7).Value = row.CumulativeSharePercent;
            }

            sheet.Range(3, 1, 3, 7).Style.Font.Bold = true;
            sheet.Columns().AdjustToContents();
        }

        private static void FillInventorySheet(IXLWorksheet sheet, ReportsResponse report)
        {
            sheet.Cell(1, 1).Value = "Отчет";
            sheet.Cell(1, 2).Value = report.PeriodName;
            sheet.Cell(2, 1).Value = "Сформирован";
            sheet.Cell(2, 2).Value = report.GeneratedAt;
            sheet.Cell(2, 3).Value = "Пользователь";
            sheet.Cell(2, 4).Value = report.GeneratedBy;
            sheet.Cell(3, 1).Value = "Сырье";
            sheet.Cell(3, 2).Value = "Ед. изм.";
            sheet.Cell(3, 3).Value = "По заказам";
            sheet.Cell(3, 4).Value = "По списаниям";
            sheet.Cell(3, 5).Value = "По заготовкам";
            sheet.Cell(3, 6).Value = "Суммарный расход";
            sheet.Cell(3, 7).Value = "Текущий остаток";
            sheet.Cell(3, 8).Value = "Разница";
            sheet.Cell(3, 9).Value = "Цена";
            sheet.Cell(3, 10).Value = "Разница, руб.";

            for (var i = 0; i < report.InventoryItems.Count; i++)
            {
                var row = report.InventoryItems[i];
                var excelRow = i + 4;
                sheet.Cell(excelRow, 1).Value = row.Name;
                sheet.Cell(excelRow, 2).Value = row.UnitName;
                sheet.Cell(excelRow, 3).Value = row.OrderConsumption;
                sheet.Cell(excelRow, 4).Value = row.WriteOffConsumption;
                sheet.Cell(excelRow, 5).Value = row.PreparationConsumption;
                sheet.Cell(excelRow, 6).Value = row.ExpectedConsumption;
                sheet.Cell(excelRow, 7).Value = row.ActualStock;
                sheet.Cell(excelRow, 8).Value = row.Difference;
                sheet.Cell(excelRow, 9).Value = row.UnitCostRub;
                sheet.Cell(excelRow, 10).Value = row.DifferenceCostRub;
            }

            sheet.Range(3, 1, 3, 10).Style.Font.Bold = true;
            sheet.Columns().AdjustToContents();
        }

        private static void FillInventoryDetailsSheet(IXLWorksheet sheet, ReportsResponse report)
        {
            sheet.Cell(1, 1).Value = "Отчет";
            sheet.Cell(1, 2).Value = report.PeriodName;
            sheet.Cell(2, 1).Value = "Сформирован";
            sheet.Cell(2, 2).Value = report.GeneratedAt;
            sheet.Cell(2, 3).Value = "Пользователь";
            sheet.Cell(2, 4).Value = report.GeneratedBy;
            sheet.Cell(3, 1).Value = "Сырье";
            sheet.Cell(3, 2).Value = "Источник";
            sheet.Cell(3, 3).Value = "Документ";
            sheet.Cell(3, 4).Value = "Дата";
            sheet.Cell(3, 5).Value = "Количество";
            sheet.Cell(3, 6).Value = "Ед. изм.";

            var excelRow = 4;
            foreach (var item in report.InventoryItems)
            {
                foreach (var detail in item.Details)
                {
                    sheet.Cell(excelRow, 1).Value = item.Name;
                    sheet.Cell(excelRow, 2).Value = detail.SourceType;
                    sheet.Cell(excelRow, 3).Value = detail.SourceName;
                    sheet.Cell(excelRow, 4).Value = detail.SourceDate;
                    sheet.Cell(excelRow, 5).Value = detail.Quantity;
                    sheet.Cell(excelRow, 6).Value = detail.UnitName;
                    excelRow++;
                }
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
            DrawText(gfx, "Сформирован: " + report.GeneratedAt.ToString("dd.MM.yyyy HH:mm") + " | Пользователь: " + report.GeneratedBy, font, 36, ref y);
            y += 10;

            DrawMenuSection(gfx, "3 самые популярные позиции", report.PopularItems, boldFont, font, ref y, page, document);
            DrawMenuSection(gfx, "3 самые непопулярные позиции", report.UnpopularItems, boldFont, font, ref y, page, document);
            DrawAbcSection(gfx, report.AbcItems.Take(20).ToList(), boldFont, font, ref y, page, document);
            DrawInventorySection(gfx, report.InventoryItems.Take(40).ToList(), boldFont, font, ref y, page, document);
            DrawInventoryDetailsSection(gfx, report.InventoryItems, boldFont, font, ref y, page, document);

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

        private static void DrawAbcSection(
            XGraphics gfx,
            List<AbcReportItemDto> rows,
            XFont boldFont,
            XFont font,
            ref double y,
            PdfPage page,
            PdfDocument document)
        {
            EnsurePdfSpace(ref gfx, ref page, document, ref y, 95);
            DrawText(gfx, "ABC-анализ по выручке", boldFont, 36, ref y);
            DrawText(gfx, "Место | Название | Выручка | Доля | Накопительно", font, 36, ref y);

            foreach (var row in rows)
            {
                EnsurePdfSpace(ref gfx, ref page, document, ref y, 22);
                var line = $"{row.Group} | {TrimForPdf(row.Name)} | {FormatDecimal(row.Revenue)} руб. | {FormatDecimal(row.RevenueSharePercent)}% | {FormatDecimal(row.CumulativeSharePercent)}%";
                DrawText(gfx, line, font, 36, ref y);
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
            DrawText(gfx, "Инвентаризация сырья", boldFont, 36, ref y);
            DrawText(gfx, "Сырье | Заказы | Списания | Заготовки | Сумма | Остаток | Разница", font, 36, ref y);

            foreach (var row in rows)
            {
                EnsurePdfSpace(ref gfx, ref page, document, ref y, 25);
                var line = $"{TrimForPdf(row.Name)} | {FormatDecimal(row.OrderConsumption)} | {FormatDecimal(row.WriteOffConsumption)} | {FormatDecimal(row.PreparationConsumption)} | {FormatDecimal(row.ExpectedConsumption)} | {FormatDecimal(row.ActualStock)} | {FormatDecimal(row.Difference)} {row.UnitName} | {FormatDecimal(row.DifferenceCostRub)} руб.";
                DrawText(gfx, line, font, 36, ref y);
                var width = Math.Min(180, Math.Abs((double)row.Difference) * 0.5);
                var brush = row.Difference < 0 ? XBrushes.IndianRed : XBrushes.DarkSeaGreen;
                gfx.DrawRectangle(brush, 36, y - 10, width, 6);
                y += 4;
            }
        }

        private static void DrawInventoryDetailsSection(
            XGraphics gfx,
            List<InventoryReportItemDto> rows,
            XFont boldFont,
            XFont font,
            ref double y,
            PdfPage page,
            PdfDocument document)
        {
            var details = rows
                .SelectMany(item => item.Details.Take(5).Select(detail => new { Item = item, Detail = detail }))
                .Take(60)
                .ToList();

            if (details.Count == 0)
            {
                return;
            }

            EnsurePdfSpace(ref gfx, ref page, document, ref y, 70);
            DrawText(gfx, "Расшифровка инвентаризации", boldFont, 36, ref y);
            DrawText(gfx, "Сырье | Источник | Документ | Количество", font, 36, ref y);

            foreach (var row in details)
            {
                EnsurePdfSpace(ref gfx, ref page, document, ref y, 22);
                var line = $"{TrimForPdf(row.Item.Name)} | {row.Detail.SourceType} | {TrimForPdf(row.Detail.SourceName)} | {FormatDecimal(row.Detail.Quantity)} {row.Detail.UnitName}";
                DrawText(gfx, line, font, 36, ref y);
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

        private static decimal ConvertFromBaseUnits(decimal value, int? unitOfMeasureId)
        {
            return unitOfMeasureId switch
            {
                UnitKilogramsId => value / 1000m,
                UnitLitersId => value / 1000m,
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

        private sealed record SoldItem(int ItemId, decimal Quantity, string SourceName, DateTime? SourceDate);

        private sealed record SoldDrinkItem(int ItemId, decimal Quantity, int? MilkIngredientId, int? CoffeeIngredientId, string SourceName, DateTime? SourceDate);

        private sealed record DrinkModifier(int? MilkIngredientId, int? CoffeeIngredientId);

        private sealed record ConsumptionSource(string SourceType, string SourceName, DateTime? SourceDate);

        private sealed record CardIngredientRow(int IngredientId, decimal RequiredBase);

        private sealed record CardSemiFinishedRow(int SemiFinishedId, decimal RequiredBase);

        private sealed record IngredientMetadata(int Id, int? CategoryId);
    }
}
