using GardenNookApi.Entities;
using GardenNookApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Security.Claims;
using TransferModels.Kitchen;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/kitchen")]
    [Authorize]
    public class KitchenController : ControllerBase
    {
        private const string ActiveStatusTokenRu = "процесс";
        private const string ActiveStatusTokenEn = "process";
        private const string KitchenOrdersStatusActive = "active";
        private const string KitchenOrdersStatusReady = "ready";
        private const string ReadyStatusNameRu = "Готов";
        private const string ReadyStatusTokenRu = "готов";
        private const string ReadyStatusTokenEn = "ready";
        private const string CancelledStatusNameRu = "Отменен";
        private const string CancelledStatusTokenRu = "отмен";
        private const string CancelledStatusTokenEn = "cancel";
        private const string AdminRole = "Администратор";
        private const string CookRole = "Повар";
        private const string BaristaRole = "Бариста";
        private const string DishToppingCategoryToken = "к блюд";
        private const string DrinkToppingCategoryToken = "к напит";
        private const string SystemRecommendationComment = "Рекомендовано системой";
        private const string SystemRecommendationSnoozePrefix = "Рекомендовано системой: скрыто до ";
        private const int CriticalDays = 14;
        private const int UnitGramsId = 2;
        private const int UnitMillilitersId = 3;
        private const int UnitPiecesId = 4;
        private const int UnitKilogramsId = 5;
        private const int UnitLitersId = 6;
        private const decimal DecimalEpsilon = 0.000001m;

        private readonly AppDbContext _db;
        private readonly IPreparationStockService _stockService;
        private readonly KitchenPickupFilterOptions _pickupFilterOptions;

        public KitchenController(
            AppDbContext db,
            IPreparationStockService stockService,
            IOptions<KitchenPickupFilterOptions> pickupFilterOptions)
        {
            _db = db;
            _stockService = stockService;
            _pickupFilterOptions = pickupFilterOptions?.Value ?? new KitchenPickupFilterOptions();
        }

        [HttpGet("orders")]
        public async Task<ActionResult<KitchenOrdersResponse>> GetOrders([FromQuery] string? status = null)
        {
            var statusFilter = NormalizeKitchenOrdersStatus(status);
            if (statusFilter == null)
            {
                return BadRequest("Неизвестный фильтр статуса заказов");
            }

            var includeCompletedItems = statusFilter == KitchenOrdersStatusReady;
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            var ordersQuery = _db.Orders
                .AsNoTracking()
                .Where(o =>
                    o.Status != null &&
                    o.Status.Name != null);

            ordersQuery = statusFilter == KitchenOrdersStatusReady
                ? ordersQuery.Where(o =>
                    (EF.Functions.Like(o.Status!.Name!.ToLower(), $"%{ReadyStatusTokenRu}%") ||
                     EF.Functions.Like(o.Status.Name.ToLower(), $"%{ReadyStatusTokenEn}%")) &&
                    o.CreatedAt >= today &&
                    o.CreatedAt < tomorrow)
                : ordersQuery.Where(o =>
                    EF.Functions.Like(o.Status!.Name!.ToLower(), $"%{ActiveStatusTokenRu}%") ||
                    EF.Functions.Like(o.Status.Name.ToLower(), $"%{ActiveStatusTokenEn}%"));

            var orderSources = await ordersQuery
                .OrderBy(o => o.CreatedAt)
                .ThenBy(o => o.Id)
                .Select(o => new
                {
                    o.Id,
                    o.Comment,
                    o.CreatedAt,
                    o.PickupAt,
                    OrderType = o.OrderType != null ? o.OrderType.Name : null,
                    Status = o.Status != null ? o.Status.Name : null
                })
                .ToListAsync();

            if (orderSources.Count == 0)
            {
                return Ok(new KitchenOrdersResponse());
            }

            var now = DateTime.Now;
            var pickupWindow = TimeSpan.FromMinutes(Math.Max(0, _pickupFilterOptions.WindowMinutes));

            var filteredOrderSources = orderSources
                .Where(o =>
                    !o.PickupAt.HasValue ||
                    IsWithinPickupWindow(o.PickupAt.Value, now, pickupWindow))
                .ToList();

            if (filteredOrderSources.Count == 0)
            {
                return Ok(new KitchenOrdersResponse());
            }

            var overduePickupSources = filteredOrderSources
                .Where(o => o.PickupAt <= now)
                .OrderByDescending(o => o.PickupAt)
                .ThenBy(o => o.Id)
                .ToList();

            var futurePickupSources = filteredOrderSources
                .Where(o => o.PickupAt > now)
                .OrderBy(o => o.PickupAt)
                .ThenBy(o => o.Id)
                .ToList();

            var noPickupSources = filteredOrderSources
                .Where(o => !o.PickupAt.HasValue)
                .OrderBy(o => o.CreatedAt)
                .ThenBy(o => o.Id)
                .ToList();

            var orderedOrderSources = overduePickupSources
                .Concat(futurePickupSources)
                .Concat(noPickupSources)
                .ToList();

            var orderIds = orderedOrderSources
                .Select(o => o.Id)
                .ToList();

            var ordersById = orderedOrderSources.ToDictionary(
                o => o.Id,
                o => new KitchenOrderDto
                {
                    OrderId = o.Id,
                    Comment = o.Comment ?? string.Empty,
                    CreatedAt = o.CreatedAt,
                    PickupAt = o.PickupAt,
                    OrderType = o.OrderType ?? string.Empty,
                    Status = o.Status ?? string.Empty
                });

            var dishSources = await _db.OrderDishItems
                .AsNoTracking()
                .Where(i =>
                    i.OrderId.HasValue &&
                    orderIds.Contains(i.OrderId.Value) &&
                    (includeCompletedItems || !i.IsCompleted))
                .OrderBy(i => i.Id)
                .Select(i => new DishSource
                {
                    ItemId = i.Id,
                    OrderId = i.OrderId!.Value,
                    Name = i.Dish != null ? i.Dish.Name : null,
                    Quantity = i.Quantity ?? 0m
                })
                .ToListAsync();

            var dishItemIds = dishSources
                .Select(i => i.ItemId)
                .ToList();

            var dishToppingSources = dishItemIds.Count == 0
                ? new List<DishToppingSource>()
                : await _db.DishToppings
                    .AsNoTracking()
                    .Where(t => t.OrderDishItemId.HasValue && dishItemIds.Contains(t.OrderDishItemId.Value))
                    .OrderBy(t => t.Id)
                    .Select(t => new DishToppingSource
                    {
                        OrderDishItemId = t.OrderDishItemId!.Value,
                        Name = t.Topping != null ? t.Topping.Name : null,
                        Quantity = t.Quantity ?? 0m
                    })
                    .ToListAsync();

            var dishToppingsByItemId = dishToppingSources
                .GroupBy(t => t.OrderDishItemId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => new KitchenOrderDishToppingDto
                    {
                        Name = t.Name ?? "Без названия",
                        Quantity = t.Quantity
                    }).ToList());

            foreach (var dishSource in dishSources)
            {
                if (!ordersById.TryGetValue(dishSource.OrderId, out var order))
                    continue;

                var dish = new KitchenOrderDishDto
                {
                    ItemId = dishSource.ItemId,
                    Name = dishSource.Name ?? "Без названия",
                    Quantity = dishSource.Quantity
                };

                if (dishToppingsByItemId.TryGetValue(dishSource.ItemId, out var dishToppings))
                {
                    dish.Toppings = dishToppings;
                }

                order.Dishes.Add(dish);
            }

            var drinkSources = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(i =>
                    i.OrderId.HasValue &&
                    orderIds.Contains(i.OrderId.Value) &&
                    (includeCompletedItems || !i.IsCompleted))
                .OrderBy(i => i.Id)
                .Select(i => new DrinkSource
                {
                    ItemId = i.Id,
                    OrderId = i.OrderId!.Value,
                    Name = i.Drink != null ? i.Drink.Name : null,
                    Quantity = i.Quantity ?? 0m
                })
                .ToListAsync();

            var drinkItemIds = drinkSources
                .Select(i => i.ItemId)
                .ToList();

            var drinkToppingSources = drinkItemIds.Count == 0
                ? new List<DrinkToppingSource>()
                : await _db.DrinkToppings
                    .AsNoTracking()
                    .Where(t => t.OrderDrinkItemId.HasValue && drinkItemIds.Contains(t.OrderDrinkItemId.Value))
                    .OrderBy(t => t.Id)
                    .Select(t => new DrinkToppingSource
                    {
                        OrderDrinkItemId = t.OrderDrinkItemId!.Value,
                        Name = t.Topping != null ? t.Topping.Name : null,
                        Quantity = t.Quantity ?? 0m
                    })
                    .ToListAsync();

            var drinkToppingsByItemId = drinkToppingSources
                .GroupBy(t => t.OrderDrinkItemId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(t => new KitchenOrderDrinkToppingDto
                    {
                        Name = t.Name ?? "Без названия",
                        Quantity = t.Quantity
                    }).ToList());

            foreach (var drinkSource in drinkSources)
            {
                if (!ordersById.TryGetValue(drinkSource.OrderId, out var order))
                    continue;

                var drink = new KitchenOrderDrinkDto
                {
                    ItemId = drinkSource.ItemId,
                    Name = drinkSource.Name ?? "Без названия",
                    Quantity = drinkSource.Quantity
                };

                if (drinkToppingsByItemId.TryGetValue(drinkSource.ItemId, out var drinkToppings))
                {
                    drink.Toppings = drinkToppings;
                }

                order.Drinks.Add(drink);
            }

            var standaloneToppings = await _db.OrderToppingItems
                .AsNoTracking()
                .Where(i =>
                    orderIds.Contains(i.OrderId) &&
                    (includeCompletedItems || !i.IsCompleted))
                .OrderBy(i => i.Id)
                .Select(i => new
                {
                    i.Id,
                    i.OrderId,
                    Name = i.Topping != null ? i.Topping.Name : null,
                    Quantity = (decimal)i.Quantity,
                    CategoryName = i.Topping != null && i.Topping.Category != null
                        ? i.Topping.Category.Name
                        : null
                })
                .ToListAsync();

            foreach (var topping in standaloneToppings)
            {
                if (!ordersById.TryGetValue(topping.OrderId, out var order))
                    continue;

                order.Toppings.Add(new KitchenOrderStandaloneToppingDto
                {
                    ItemId = topping.Id,
                    Name = topping.Name ?? "Без названия",
                    Quantity = topping.Quantity,
                    CategoryName = topping.CategoryName ?? string.Empty
                });
            }

            var resultOrders = orderedOrderSources
                .Select(o => ordersById[o.Id])
                .Where(o => o.Dishes.Count > 0 || o.Drinks.Count > 0 || o.Toppings.Count > 0)
                .ToList();

            return Ok(new KitchenOrdersResponse
            {
                Orders = resultOrders
            });
        }

        [HttpGet("stop-list/positions")]
        public async Task<ActionResult<KitchenStopListPositionsResponse>> GetStopListPositions()
        {
            await _stockService.RefreshMenuAvailabilityAsync();

            var limitsByKey = await _db.MenuItemPortionLimits
                .AsNoTracking()
                .ToDictionaryAsync(
                    x => BuildMenuItemLimitKey(x.ItemType, x.ItemId),
                    x => RoundTo2(Math.Max(0m, x.RemainingPortions)));

            var dishSources = await _db.Dishes
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ThenBy(d => d.Id)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    CategoryName = d.Category != null ? d.Category.Name : null,
                    d.IsAvailable,
                    d.TechnicalCardId,
                    UnitName = d.UnitOfMeasure != null ? d.UnitOfMeasure.Name : null
                })
                .ToListAsync();

            var drinkSources = await _db.Drinks
                .AsNoTracking()
                .OrderBy(d => d.Name)
                .ThenBy(d => d.Id)
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    CategoryName = d.Category != null ? d.Category.Name : null,
                    d.IsAvailable,
                    d.TechnicalCardId,
                    d.Quantity,
                    UnitName = d.UnitOfMeasure != null ? d.UnitOfMeasure.Name : null
                })
                .ToListAsync();

            var toppingSources = await _db.ToppingsAndSyrups
                .AsNoTracking()
                .OrderBy(t => t.Name)
                .ThenBy(t => t.Id)
                .Select(t => new
                {
                    t.Id,
                    t.Name,
                    CategoryName = t.Category != null ? t.Category.Name : null,
                    t.IsAvailable,
                    t.TechnicalCardId,
                    t.Quantity,
                    UnitName = t.UnitOfMeasure != null ? t.UnitOfMeasure.Name : null
                })
                .ToListAsync();

            var technicalCardIds = dishSources
                .Where(x => x.TechnicalCardId.HasValue)
                .Select(x => x.TechnicalCardId!.Value)
                .Concat(drinkSources
                    .Where(x => x.TechnicalCardId.HasValue)
                    .Select(x => x.TechnicalCardId!.Value))
                .Concat(toppingSources
                    .Where(x => x.TechnicalCardId.HasValue)
                    .Select(x => x.TechnicalCardId!.Value))
                .Distinct()
                .ToList();

            var autoPortionsByTechnicalCard = await BuildAutoAvailablePortionsByTechnicalCardAsync(technicalCardIds);
            var outputByTechnicalCard = await BuildOutputWeightByTechnicalCardAsync(technicalCardIds);

            var positions = new List<KitchenStopListPositionDto>(dishSources.Count + drinkSources.Count + toppingSources.Count);

            positions.AddRange(dishSources.Select(source => BuildStopListPosition(
                KitchenItemTypes.Dish,
                source.Id,
                source.Name,
                source.CategoryName,
                BuildTechnicalCardVolumeWeight(source.TechnicalCardId, outputByTechnicalCard, source.UnitName),
                source.IsAvailable,
                source.TechnicalCardId,
                limitsByKey,
                autoPortionsByTechnicalCard)));

            positions.AddRange(drinkSources.Select(source => BuildStopListPosition(
                KitchenItemTypes.Drink,
                source.Id,
                source.Name,
                source.CategoryName,
                BuildVolumeWeight(source.Quantity, source.UnitName),
                source.IsAvailable,
                source.TechnicalCardId,
                limitsByKey,
                autoPortionsByTechnicalCard)));

            positions.AddRange(toppingSources.Select(source => BuildStopListPosition(
                KitchenItemTypes.Topping,
                source.Id,
                source.Name,
                source.CategoryName,
                BuildVolumeWeight(source.Quantity, source.UnitName),
                source.IsAvailable,
                source.TechnicalCardId,
                limitsByKey,
                autoPortionsByTechnicalCard)));

            positions = positions
                .Where(IsStopListPositionVisibleForCurrentRole)
                .OrderBy(x => x.Name)
                .ThenBy(x => x.ItemType)
                .ThenBy(x => x.ItemId)
                .ToList();

            return Ok(new KitchenStopListPositionsResponse
            {
                Positions = positions
            });
        }

        [HttpPost("stop-list/items")]
        public async Task<IActionResult> AddStopListItem([FromBody] KitchenStopListItemRequest request)
        {
            if (request == null || request.ItemId <= 0)
            {
                return BadRequest("Некорректные данные позиции");
            }

            if (!TryParseItemType(request.ItemType, out var itemType))
            {
                return BadRequest("Неизвестный тип позиции");
            }

            if (request.RemainingPortions < 0m)
            {
                return BadRequest("Остаток порций не может быть отрицательным.");
            }

            if (!await CanCurrentRoleManageStopListItemAsync(itemType, request.ItemId))
            {
                return Forbid();
            }

            var roundedRemainingPortions = RoundTo2(request.RemainingPortions);
            var now = DateTime.Now;
            var itemTypeToken = ToItemTypeToken(itemType);

            await using var tx = await _db.Database.BeginTransactionAsync();

            var itemExists = await SetMenuItemAvailabilityAsync(
                itemType,
                request.ItemId,
                roundedRemainingPortions > 0m,
                saveChanges: false);
            if (!itemExists)
            {
                await tx.RollbackAsync();
                return NotFound("Позиция не найдена");
            }

            await UpsertMenuItemPortionLimitAsync(itemTypeToken, request.ItemId, roundedRemainingPortions, now);
            await _db.SaveChangesAsync();

            if (roundedRemainingPortions > 0m)
            {
                await _stockService.RefreshMenuAvailabilityAsync();
            }

            await tx.CommitAsync();
            return NoContent();
        }

        [HttpDelete("stop-list/items/{itemType}/{itemId:int}")]
        public async Task<IActionResult> RemoveStopListItem(string itemType, int itemId)
        {
            if (itemId <= 0)
            {
                return BadRequest("Некорректный идентификатор позиции");
            }

            if (!TryParseItemType(itemType, out var parsedItemType))
            {
                return BadRequest("Неизвестный тип позиции");
            }

            if (!await CanCurrentRoleManageStopListItemAsync(parsedItemType, itemId))
            {
                return Forbid();
            }

            await using var tx = await _db.Database.BeginTransactionAsync();

            var itemTypeToken = ToItemTypeToken(parsedItemType);
            var menuItemLimit = await _db.MenuItemPortionLimits
                .FirstOrDefaultAsync(x =>
                    x.ItemType == itemTypeToken &&
                    x.ItemId == itemId);
            if (menuItemLimit != null)
            {
                _db.MenuItemPortionLimits.Remove(menuItemLimit);
            }

            var itemExists = await SetMenuItemAvailabilityAsync(parsedItemType, itemId, true, saveChanges: false);
            if (!itemExists)
            {
                await tx.RollbackAsync();
                return NotFound("Позиция не найдена");
            }

            await _db.SaveChangesAsync();
            await _stockService.RefreshMenuAvailabilityAsync();
            await tx.CommitAsync();
            return NoContent();
        }

        [HttpPost("orders/{orderId:int}/items/{itemType}/{itemId:int}/complete")]
        public async Task<ActionResult<KitchenCompleteOrderItemResponse>> CompleteOrderItem(
            int orderId,
            string itemType,
            int itemId)
        {
            if (!TryParseItemType(itemType, out var parsedItemType))
            {
                return BadRequest("Неизвестный тип позиции заказа");
            }

            var orderExists = await _db.Orders.AnyAsync(o => o.Id == orderId);
            if (!orderExists)
            {
                return NotFound("Заказ не найден");
            }

            var itemCompleted = await MarkItemCompletedAsync(orderId, parsedItemType, itemId);
            if (!itemCompleted)
            {
                return NotFound("Позиция заказа не найдена");
            }

            try
            {
                var orderCompleted = await TryMarkOrderReadyIfCompletedAsync(orderId);
                var orderStatusName = await GetOrderStatusNameAsync(orderId);

                return Ok(new KitchenCompleteOrderItemResponse
                {
                    OrderId = orderId,
                    ItemType = ToItemTypeToken(parsedItemType),
                    ItemId = itemId,
                    OrderCompleted = orderCompleted,
                    OrderStatus = orderStatusName
                });
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("orders/{orderId:int}/complete")]
        public async Task<ActionResult<KitchenCompleteOrderResponse>> CompleteOrder(int orderId)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            await MarkAllOrderItemsCompletedAsync(orderId);

            var readyStatus = await ResolveReadyStatusAsync();
            if (readyStatus == null)
            {
                return StatusCode(500, "Статус готовности заказа не найден");
            }

            order.StatusId = readyStatus.Value.Id;
            await _db.SaveChangesAsync();

            return Ok(new KitchenCompleteOrderResponse
            {
                OrderId = orderId,
                OrderStatus = readyStatus.Value.Name
            });
        }

        [HttpPost("orders/{orderId:int}/cancel")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<KitchenCompleteOrderResponse>> CancelOrder(int orderId)
        {
            var cancelledStatus = await ResolveCancelledStatusAsync();
            if (cancelledStatus == null)
            {
                return StatusCode(500, "Статус отмены заказа не найден");
            }

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            order.StatusId = cancelledStatus.Value.Id;
            await _db.SaveChangesAsync();

            return Ok(new KitchenCompleteOrderResponse
            {
                OrderId = order.Id,
                OrderStatus = cancelledStatus.Value.Name
            });
        }

        [HttpGet("orders/{orderId:int}/items/{itemType}/{itemId:int}/technical-card")]
        public async Task<ActionResult<KitchenTechnicalCardResponse>> GetTechnicalCard(
            int orderId,
            string itemType,
            int itemId)
        {
            if (!TryParseItemType(itemType, out var parsedItemType))
            {
                return BadRequest("Неизвестный тип позиции заказа");
            }

            var source = await LoadTechnicalCardSourceAsync(orderId, parsedItemType, itemId);
            if (source == null)
            {
                return NotFound("Позиция заказа не найдена");
            }

            if (!source.TechnicalCardId.HasValue)
            {
                return NotFound("Тех. карта для позиции не найдена");
            }

            var technicalCardId = source.TechnicalCardId.Value;
            var response = await BuildTechnicalCardResponseAsync(technicalCardId, source.ItemName);
            if (response == null)
            {
                return NotFound("Тех. карта не найдена");
            }

            return Ok(response);
        }

        [HttpGet("technical-cards")]
        public async Task<ActionResult<KitchenTechnicalCardsResponse>> GetTechnicalCards()
        {
            var technicalCards = await _db.TechnicalCards
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .ThenBy(c => c.Id)
                .Select(c => new KitchenTechnicalCardListItemDto
                {
                    TechnicalCardId = c.Id,
                    CardName = string.IsNullOrWhiteSpace(c.Name)
                        ? $"Technical card #{c.Id}"
                        : c.Name,
                    Description = c.Description ?? string.Empty
                })
                .ToListAsync();

            return Ok(new KitchenTechnicalCardsResponse
            {
                TechnicalCards = technicalCards
            });
        }

        [HttpGet("technical-cards/{technicalCardId:int}")]
        public async Task<ActionResult<KitchenTechnicalCardResponse>> GetTechnicalCardById(int technicalCardId)
        {
            var response = await BuildTechnicalCardResponseAsync(technicalCardId, null);
            if (response == null)
            {
                return NotFound("Technical card not found");
            }

            return Ok(response);
        }

        [HttpGet("technical-cards/edit-options")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<KitchenTechnicalCardEditOptionsResponse>> GetTechnicalCardEditOptions()
        {
            var ingredients = await _db.Ingredients
                .AsNoTracking()
                .OrderBy(i => i.Name)
                .ThenBy(i => i.Id)
                .Select(i => new KitchenTechnicalCardReferenceDto
                {
                    Id = i.Id,
                    Name = string.IsNullOrWhiteSpace(i.Name) ? $"Ingredient #{i.Id}" : i.Name,
                    UnitOfMeasureId = i.UnitOfMeasureId,
                    UnitName = i.UnitOfMeasure != null ? (i.UnitOfMeasure.Name ?? string.Empty) : string.Empty
                })
                .ToListAsync();

            var semiFinisheds = await _db.SemiFinisheds
                .AsNoTracking()
                .OrderBy(s => s.Name)
                .ThenBy(s => s.Id)
                .Select(s => new KitchenTechnicalCardReferenceDto
                {
                    Id = s.Id,
                    Name = string.IsNullOrWhiteSpace(s.Name) ? $"SemiFinished #{s.Id}" : s.Name,
                    UnitOfMeasureId = s.UnitOfMeasureId,
                    UnitName = s.UnitOfMeasure != null ? (s.UnitOfMeasure.Name ?? string.Empty) : string.Empty
                })
                .ToListAsync();

            var units = await _db.UnitsOfMeasures
                .AsNoTracking()
                .OrderBy(u => u.Name)
                .ThenBy(u => u.Id)
                .Select(u => new KitchenTechnicalCardUnitDto
                {
                    Id = u.Id,
                    Name = string.IsNullOrWhiteSpace(u.Name) ? $"Unit #{u.Id}" : u.Name
                })
                .ToListAsync();

            var bindingOptions = new List<KitchenTechnicalCardBindingOptionDto>();
            bindingOptions.AddRange(await _db.Dishes
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new KitchenTechnicalCardBindingOptionDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.Dish,
                    ItemId = x.Id,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Dish #{x.Id}" : x.Name,
                    TechnicalCardId = x.TechnicalCardId
                })
                .ToListAsync());
            bindingOptions.AddRange(await _db.Drinks
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new KitchenTechnicalCardBindingOptionDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.Drink,
                    ItemId = x.Id,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Drink #{x.Id}" : x.Name,
                    TechnicalCardId = x.TechnicalCardId
                })
                .ToListAsync());
            bindingOptions.AddRange(await _db.ToppingsAndSyrups
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new KitchenTechnicalCardBindingOptionDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.Topping,
                    ItemId = x.Id,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Topping #{x.Id}" : x.Name,
                    TechnicalCardId = x.TechnicalCardId
                })
                .ToListAsync());
            bindingOptions.AddRange(semiFinisheds.Select(x => new KitchenTechnicalCardBindingOptionDto
            {
                Kind = KitchenTechnicalCardBindingKinds.SemiFinished,
                ItemId = x.Id,
                Name = x.Name,
                TechnicalCardId = null
            }));

            var semiFinishedCardIds = await _db.SemiFinisheds
                .AsNoTracking()
                .Select(x => new { x.Id, x.TechnicalCardId })
                .ToDictionaryAsync(x => x.Id, x => x.TechnicalCardId);

            foreach (var option in bindingOptions.Where(x => x.Kind == KitchenTechnicalCardBindingKinds.SemiFinished))
            {
                option.TechnicalCardId = semiFinishedCardIds.TryGetValue(option.ItemId, out var technicalCardId)
                    ? technicalCardId
                    : null;
            }

            return Ok(new KitchenTechnicalCardEditOptionsResponse
            {
                Ingredients = ingredients,
                SemiFinisheds = semiFinisheds,
                Units = units,
                BindingOptions = bindingOptions
            });
        }

        [HttpGet("technical-cards/{technicalCardId:int}/edit")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<KitchenTechnicalCardEditResponse>> GetTechnicalCardForEdit(int technicalCardId)
        {
            var response = await BuildTechnicalCardEditResponseAsync(technicalCardId);
            if (response == null)
            {
                return NotFound("Technical card not found");
            }

            return Ok(response);
        }

        [HttpPost("technical-cards")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<KitchenTechnicalCardResponse>> CreateTechnicalCard(KitchenTechnicalCardUpsertRequest request)
        {
            var validationError = ValidateTechnicalCardRequest(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var referenceError = await ValidateTechnicalCardReferencesAsync(request);
            if (referenceError != null)
            {
                return BadRequest(referenceError);
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            var technicalCard = new TechnicalCard
            {
                Name = request.CardName.Trim(),
                Description = NormalizeDescription(request.Description)
            };

            _db.TechnicalCards.Add(technicalCard);
            await _db.SaveChangesAsync();

            await ReplaceTechnicalCardDetailsAsync(technicalCard.Id, request);
            await transaction.CommitAsync();

            var response = await BuildTechnicalCardResponseAsync(technicalCard.Id, null);
            return CreatedAtAction(nameof(GetTechnicalCardById), new { technicalCardId = technicalCard.Id }, response);
        }

        [HttpPut("technical-cards/{technicalCardId:int}")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<KitchenTechnicalCardResponse>> UpdateTechnicalCard(
            int technicalCardId,
            KitchenTechnicalCardUpsertRequest request)
        {
            var validationError = ValidateTechnicalCardRequest(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var referenceError = await ValidateTechnicalCardReferencesAsync(request);
            if (referenceError != null)
            {
                return BadRequest(referenceError);
            }

            var technicalCard = await _db.TechnicalCards.FirstOrDefaultAsync(c => c.Id == technicalCardId);
            if (technicalCard == null)
            {
                return NotFound("Technical card not found");
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            technicalCard.Name = request.CardName.Trim();
            technicalCard.Description = NormalizeDescription(request.Description);
            await ReplaceTechnicalCardDetailsAsync(technicalCard.Id, request);
            await transaction.CommitAsync();

            var response = await BuildTechnicalCardResponseAsync(technicalCard.Id, null);
            return Ok(response);
        }

        [HttpDelete("technical-cards/{technicalCardId:int}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteTechnicalCard(int technicalCardId)
        {
            var technicalCard = await _db.TechnicalCards.FirstOrDefaultAsync(c => c.Id == technicalCardId);
            if (technicalCard == null)
            {
                return NotFound("Technical card not found");
            }

            var linkedNames = await LoadTechnicalCardLinkedNamesAsync(technicalCardId);
            if (linkedNames.Count > 0)
            {
                return BadRequest("Нельзя удалить техкарту: она привязана к позициям: " + string.Join(", ", linkedNames));
            }

            await using var transaction = await _db.Database.BeginTransactionAsync();

            var ingredientRows = await _db.TechnicalCardIngredientCompositions
                .Where(x => x.TechnicalCardId == technicalCardId)
                .ToListAsync();
            var semiFinishedRows = await _db.TechnicalCardSemiFinishedCompositions
                .Where(x => x.TechnicalCardId == technicalCardId)
                .ToListAsync();

            _db.TechnicalCardIngredientCompositions.RemoveRange(ingredientRows);
            _db.TechnicalCardSemiFinishedCompositions.RemoveRange(semiFinishedRows);
            _db.TechnicalCards.Remove(technicalCard);

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return NoContent();
        }

        [HttpGet("preparations")]
        public async Task<ActionResult<KitchenPreparationsBoardResponse>> GetPreparationsBoard()
        {
            await CleanupNonPositivePreparationsAsync();
            await EnsureSystemPreparationTasksAsync();
            var visibleSemiFinishedIds = await ResolveVisiblePreparationSemiFinishedIdsAsync();

            var tasks = await _db.PreparationTasks
                .AsNoTracking()
                .OrderByDescending(t => t.CreatedAt)
                .ThenByDescending(t => t.Id)
                .Select(t => new KitchenPreparationTaskDto
                {
                    TaskId = t.Id,
                    SemiFinishedId = t.SemiFinishedId,
                    TaskText = t.TaskText,
                    IsLinkedToSemiFinished = t.SemiFinishedId.HasValue,
                    TechnicalCardId = t.SemiFinished != null ? t.SemiFinished.TechnicalCardId : null,
                    SemiFinishedName = t.SemiFinishedId.HasValue
                        ? (t.SemiFinished != null && !string.IsNullOrWhiteSpace(t.SemiFinished.Name)
                            ? t.SemiFinished.Name
                            : $"SemiFinished #{t.SemiFinishedId.Value}")
                        : string.Empty,
                    Comment = t.Comment ?? string.Empty,
                    CreatedAt = t.CreatedAt
                })
                .ToListAsync();

            tasks = tasks
                .Where(t => !IsSnoozedSystemRecommendationComment(t.Comment))
                .ToList();

            if (visibleSemiFinishedIds != null)
            {
                tasks = tasks
                    .Where(t => !t.SemiFinishedId.HasValue || visibleSemiFinishedIds.Contains(t.SemiFinishedId.Value))
                    .ToList();
            }

            var existingPreparations = await _db.Preparations
                .AsNoTracking()
                .Where(p =>
                    p.SemiFinishedId.HasValue &&
                    p.StockGrams.HasValue &&
                    p.StockGrams.Value > 0m)
                .OrderByDescending(p => p.ProductionDate)
                .ThenByDescending(p => p.Id)
                .Select(p => new KitchenPreparationListItemDto
                {
                    PreparationId = p.Id,
                    SemiFinishedId = p.SemiFinishedId!.Value,
                    TechnicalCardId = p.SemiFinished != null ? p.SemiFinished.TechnicalCardId : null,
                    PreparationName = p.SemiFinished != null && !string.IsNullOrWhiteSpace(p.SemiFinished.Name)
                        ? p.SemiFinished.Name
                        : (string.IsNullOrWhiteSpace(p.Name) ? $"Preparation #{p.Id}" : p.Name),
                    StockGrams = p.StockGrams ?? 0m,
                    ProductionDate = p.ProductionDate.HasValue
                        ? p.ProductionDate.Value.ToDateTime(TimeOnly.MinValue)
                        : null
                })
                .ToListAsync();

            if (visibleSemiFinishedIds != null)
            {
                existingPreparations = existingPreparations
                    .Where(p => visibleSemiFinishedIds.Contains(p.SemiFinishedId))
                    .ToList();
            }

            var semiFinishedOptions = await _db.SemiFinisheds
                .AsNoTracking()
                .OrderBy(sf => sf.Name)
                .ThenBy(sf => sf.Id)
                .Select(sf => new KitchenSemiFinishedOptionDto
                {
                    SemiFinishedId = sf.Id,
                    TechnicalCardId = sf.TechnicalCardId,
                    Name = string.IsNullOrWhiteSpace(sf.Name)
                        ? $"SemiFinished #{sf.Id}"
                        : sf.Name
                })
                .ToListAsync();

            if (visibleSemiFinishedIds != null)
            {
                semiFinishedOptions = semiFinishedOptions
                    .Where(sf => visibleSemiFinishedIds.Contains(sf.SemiFinishedId))
                    .ToList();
            }

            return Ok(new KitchenPreparationsBoardResponse
            {
                Tasks = tasks,
                ExistingPreparations = existingPreparations,
                SemiFinishedOptions = semiFinishedOptions
            });
        }

        private async Task CleanupNonPositivePreparationsAsync()
        {
            var invalidPreparations = await _db.Preparations
                .Where(p => !p.StockGrams.HasValue || p.StockGrams.Value <= 0m)
                .ToListAsync();

            if (invalidPreparations.Count == 0)
            {
                return;
            }

            _db.Preparations.RemoveRange(invalidPreparations);
            await _db.SaveChangesAsync();
            await _stockService.RefreshMenuAvailabilityAsync();
        }

        private async Task<HashSet<int>?> ResolveVisiblePreparationSemiFinishedIdsAsync()
        {
            if (IsCurrentUserInRole(AdminRole))
            {
                return null;
            }

            if (IsCurrentUserInRole(CookRole))
            {
                return await LoadSemiFinishedIdsUsedByDishesAsync();
            }

            if (IsCurrentUserInRole(BaristaRole))
            {
                return await LoadSemiFinishedIdsUsedByDrinksAsync();
            }

            return new HashSet<int>();
        }

        private async Task<HashSet<int>> LoadSemiFinishedIdsUsedByDishesAsync()
        {
            return (await _db.TechnicalCardSemiFinishedCompositions
                    .AsNoTracking()
                    .Where(c =>
                        c.SemiFinishedId.HasValue &&
                        c.TechnicalCardId.HasValue &&
                        c.TechnicalCard != null &&
                        c.TechnicalCard.Dishes.Any())
                    .Select(c => c.SemiFinishedId!.Value)
                    .Distinct()
                    .ToListAsync())
                .ToHashSet();
        }

        private async Task<HashSet<int>> LoadSemiFinishedIdsUsedByDrinksAsync()
        {
            return (await _db.TechnicalCardSemiFinishedCompositions
                    .AsNoTracking()
                    .Where(c =>
                        c.SemiFinishedId.HasValue &&
                        c.TechnicalCardId.HasValue &&
                        c.TechnicalCard != null &&
                        c.TechnicalCard.Drinks.Any())
                    .Select(c => c.SemiFinishedId!.Value)
                    .Distinct()
                    .ToListAsync())
                .ToHashSet();
        }

        private async Task EnsureSystemPreparationTasksAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            var criticalBoundaryDate = today.AddDays(-CriticalDays);

            var latestProductionDates = await _db.Preparations
                .AsNoTracking()
                .Where(p =>
                    p.SemiFinishedId.HasValue &&
                    p.ProductionDate.HasValue &&
                    p.StockGrams.HasValue &&
                    p.StockGrams.Value > 0m)
                .GroupBy(p => p.SemiFinishedId!.Value)
                .Select(g => new
                {
                    SemiFinishedId = g.Key,
                    LatestProductionDate = g.Max(p => p.ProductionDate)
                })
                .ToListAsync();

            var staleSemiFinishedIds = latestProductionDates
                .Where(x => x.LatestProductionDate.HasValue && x.LatestProductionDate.Value < criticalBoundaryDate)
                .Select(x => x.SemiFinishedId)
                .ToHashSet();

            var systemTasks = await _db.PreparationTasks
                .Where(t =>
                    t.SemiFinishedId.HasValue &&
                    (t.Comment == SystemRecommendationComment ||
                     (t.Comment != null && t.Comment.StartsWith(SystemRecommendationSnoozePrefix))))
                .OrderByDescending(t => t.CreatedAt)
                .ThenByDescending(t => t.Id)
                .ToListAsync();

            var now = DateTime.Now;
            var hasChanges = false;
            var activeSystemTaskSemiFinishedIds = new HashSet<int>();
            var snoozedSystemTaskSemiFinishedIds = new HashSet<int>();

            foreach (var group in systemTasks
                .Where(t => t.SemiFinishedId.HasValue)
                .GroupBy(t => t.SemiFinishedId!.Value))
            {
                var semiFinishedId = group.Key;
                var orderedTasks = group
                    .OrderByDescending(t => t.CreatedAt)
                    .ThenByDescending(t => t.Id)
                    .ToList();

                if (!staleSemiFinishedIds.Contains(semiFinishedId))
                {
                    _db.PreparationTasks.RemoveRange(orderedTasks);
                    hasChanges = true;
                    continue;
                }

                var latestTask = orderedTasks[0];
                if (TryParseSnoozedSystemRecommendationUntil(latestTask.Comment, out var snoozedUntilDate))
                {
                    if (snoozedUntilDate > today)
                    {
                        snoozedSystemTaskSemiFinishedIds.Add(semiFinishedId);
                    }
                    else
                    {
                        latestTask.Comment = SystemRecommendationComment;
                        latestTask.CreatedAt = now;
                        hasChanges = true;
                        activeSystemTaskSemiFinishedIds.Add(semiFinishedId);
                    }
                }
                else
                {
                    activeSystemTaskSemiFinishedIds.Add(semiFinishedId);
                }

                if (orderedTasks.Count > 1)
                {
                    _db.PreparationTasks.RemoveRange(orderedTasks.Skip(1));
                    hasChanges = true;
                }
            }

            var missingSemiFinishedIds = staleSemiFinishedIds
                .Except(activeSystemTaskSemiFinishedIds)
                .Except(snoozedSystemTaskSemiFinishedIds)
                .ToList();

            if (missingSemiFinishedIds.Count > 0)
            {
                var semiFinishedNamesById = await _db.SemiFinisheds
                    .AsNoTracking()
                    .Where(sf => missingSemiFinishedIds.Contains(sf.Id))
                    .Select(sf => new
                    {
                        sf.Id,
                        sf.Name
                    })
                    .ToDictionaryAsync(
                        sf => sf.Id,
                        sf => string.IsNullOrWhiteSpace(sf.Name)
                            ? $"SemiFinished #{sf.Id}"
                            : sf.Name!);

                foreach (var semiFinishedId in missingSemiFinishedIds)
                {
                    var taskText = semiFinishedNamesById.TryGetValue(semiFinishedId, out var semiFinishedName)
                        ? semiFinishedName
                        : $"SemiFinished #{semiFinishedId}";

                    _db.PreparationTasks.Add(new PreparationTask
                    {
                        SemiFinishedId = semiFinishedId,
                        TaskText = taskText,
                        Comment = SystemRecommendationComment,
                        CreatedAt = now
                    });
                }

                hasChanges = true;
            }

            if (!hasChanges)
            {
                return;
            }

            await _db.SaveChangesAsync();
        }

        [HttpPost("preparations/tasks")]
        public async Task<ActionResult<KitchenPreparationTaskDto>> CreatePreparationTask(
            [FromBody] KitchenCreatePreparationTaskRequest request)
        {
            if (IsCurrentUserInRole(AdminRole))
            {
                return Forbid();
            }

            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            var normalizedTaskText = NormalizeTaskText(request.TaskText);
            if (normalizedTaskText == null)
            {
                return BadRequest("TaskText is required.");
            }

            if (normalizedTaskText.Length > 255)
            {
                return BadRequest("TaskText is too long. Max length is 255 characters.");
            }

            if (request.SemiFinishedId.HasValue && request.SemiFinishedId.Value <= 0)
            {
                return BadRequest("SemiFinishedId must be greater than zero.");
            }

            var normalizedComment = NormalizeComment(request.Comment);
            if (normalizedComment != null && normalizedComment.Length > 500)
            {
                return BadRequest("Comment is too long. Max length is 500 characters.");
            }

            int? semiFinishedId = null;
            int? technicalCardId = null;
            string semiFinishedName = string.Empty;
            var visibleSemiFinishedIds = await ResolveVisiblePreparationSemiFinishedIdsAsync();

            if (request.SemiFinishedId.HasValue)
            {
                if (visibleSemiFinishedIds != null &&
                    !visibleSemiFinishedIds.Contains(request.SemiFinishedId.Value))
                {
                    return Forbid();
                }

                var semiFinished = await _db.SemiFinisheds
                    .AsNoTracking()
                    .Where(sf => sf.Id == request.SemiFinishedId.Value)
                    .Select(sf => new
                    {
                        sf.Id,
                        sf.Name,
                        sf.TechnicalCardId
                    })
                    .FirstOrDefaultAsync();

                if (semiFinished == null)
                {
                    return NotFound("Semi-finished item not found.");
                }

                semiFinishedId = semiFinished.Id;
                technicalCardId = semiFinished.TechnicalCardId;
                semiFinishedName = string.IsNullOrWhiteSpace(semiFinished.Name)
                    ? $"SemiFinished #{semiFinished.Id}"
                    : semiFinished.Name;
            }

            var task = new PreparationTask
            {
                SemiFinishedId = semiFinishedId,
                TaskText = normalizedTaskText,
                Comment = normalizedComment,
                CreatedAt = DateTime.Now
            };

            _db.PreparationTasks.Add(task);
            await _db.SaveChangesAsync();

            return Ok(new KitchenPreparationTaskDto
            {
                TaskId = task.Id,
                SemiFinishedId = task.SemiFinishedId,
                TaskText = task.TaskText,
                IsLinkedToSemiFinished = task.SemiFinishedId.HasValue,
                TechnicalCardId = technicalCardId,
                SemiFinishedName = semiFinishedName,
                Comment = task.Comment ?? string.Empty,
                CreatedAt = task.CreatedAt
            });
        }

        [HttpDelete("preparations/tasks/{taskId:int}")]
        public async Task<IActionResult> DeletePreparationTask(int taskId)
        {
            if (IsCurrentUserInRole(AdminRole))
            {
                return Forbid();
            }

            var task = await _db.PreparationTasks.FirstOrDefaultAsync(t => t.Id == taskId);
            if (task == null)
            {
                return NotFound("Preparation task not found.");
            }

            if (task.SemiFinishedId.HasValue && IsSystemRecommendationComment(task.Comment))
            {
                var snoozedUntilDate = DateOnly.FromDateTime(DateTime.Today).AddDays(1);
                task.Comment = BuildSnoozedSystemRecommendationComment(snoozedUntilDate);
                task.CreatedAt = DateTime.Now;
            }
            else
            {
                _db.PreparationTasks.Remove(task);
            }

            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("preparations/{preparationId:int}")]
        public async Task<IActionResult> DeletePreparation(int preparationId)
        {
            var preparation = await _db.Preparations.FirstOrDefaultAsync(p => p.Id == preparationId);
            if (preparation == null)
            {
                return NotFound("Preparation not found.");
            }

            _db.Preparations.Remove(preparation);
            await _db.SaveChangesAsync();
            await _stockService.RefreshMenuAvailabilityAsync();

            return NoContent();
        }

        [HttpPost("preparations/tasks/{taskId:int}/complete")]
        public async Task<ActionResult<KitchenPreparationListItemDto>> CompletePreparationTask(
            int taskId,
            [FromBody] KitchenCompletePreparationTaskRequest request)
        {
            if (IsCurrentUserInRole(AdminRole))
            {
                return Forbid();
            }

            var task = await _db.PreparationTasks
                .Include(t => t.SemiFinished)
                .FirstOrDefaultAsync(t => t.Id == taskId);

            if (task == null)
            {
                return NotFound("Preparation task not found.");
            }

            if (!task.SemiFinishedId.HasValue)
            {
                _db.PreparationTasks.Remove(task);
                await _db.SaveChangesAsync();
                return NoContent();
            }

            if (request == null)
            {
                return BadRequest("Request body is required.");
            }

            if (request.StockGrams <= 0m)
            {
                return BadRequest("StockGrams must be greater than zero.");
            }

            var productionDate = (request.ProductionDate ?? DateTime.Today).Date;
            var roundedStock = decimal.Round(request.StockGrams, 2, MidpointRounding.AwayFromZero);
            var preparationName = !string.IsNullOrWhiteSpace(task.SemiFinished?.Name)
                ? task.SemiFinished.Name
                : task.TaskText;

            var preparation = new Preparation
            {
                Name = preparationName,
                SemiFinishedId = task.SemiFinishedId.Value,
                StockGrams = roundedStock,
                ProductionDate = DateOnly.FromDateTime(productionDate)
            };

            _db.Preparations.Add(preparation);
            _db.PreparationTasks.Remove(task);

            await _db.SaveChangesAsync();
            await _stockService.RefreshMenuAvailabilityAsync();

            return Ok(new KitchenPreparationListItemDto
            {
                PreparationId = preparation.Id,
                SemiFinishedId = task.SemiFinishedId.Value,
                TechnicalCardId = task.SemiFinished?.TechnicalCardId,
                PreparationName = preparationName,
                StockGrams = roundedStock,
                ProductionDate = productionDate
            });
        }

        [HttpGet("write-off/board")]
        public async Task<ActionResult<KitchenWriteOffBoardResponse>> GetWriteOffBoard()
        {
            var writeOffTypes = await _db.WriteOffTypes
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new KitchenWriteOffTypeDto
                {
                    WriteOffTypeId = x.Id,
                    Name = x.Name
                })
                .ToListAsync();

            var gramsUnitName = await _db.UnitsOfMeasures
                .AsNoTracking()
                .Where(x => x.Id == UnitGramsId)
                .Select(x => x.Name)
                .FirstOrDefaultAsync() ?? "г";

            var semiFinishedStockById = await _db.Preparations
                .AsNoTracking()
                .Where(x =>
                    x.SemiFinishedId.HasValue &&
                    x.StockGrams.HasValue &&
                    x.StockGrams.Value > 0m)
                .GroupBy(x => x.SemiFinishedId!.Value)
                .Select(g => new
                {
                    SemiFinishedId = g.Key,
                    AvailableStockGrams = g.Sum(x => x.StockGrams ?? 0m)
                })
                .ToDictionaryAsync(x => x.SemiFinishedId, x => RoundTo2(Math.Max(0m, x.AvailableStockGrams)));

            var semiFinishedOptions = await _db.SemiFinisheds
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Name
                })
                .ToListAsync();

            var semiFinishedDtos = semiFinishedOptions
                .Select(x => new KitchenWriteOffSemiFinishedOptionDto
                {
                    SemiFinishedId = x.Id,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Полуфабрикат #{x.Id}" : x.Name!,
                    UnitOfMeasureId = UnitGramsId,
                    UnitName = gramsUnitName,
                    AvailableStock = semiFinishedStockById.TryGetValue(x.Id, out var stock)
                        ? stock
                        : 0m
                })
                .ToList();

            var ingredientDtos = await _db.Ingredients
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new KitchenWriteOffIngredientOptionDto
                {
                    IngredientId = x.Id,
                    Name = string.IsNullOrWhiteSpace(x.Name) ? $"Сырье #{x.Id}" : x.Name!,
                    UnitOfMeasureId = x.UnitOfMeasureId,
                    UnitName = x.UnitOfMeasure != null && !string.IsNullOrWhiteSpace(x.UnitOfMeasure.Name)
                        ? x.UnitOfMeasure.Name
                        : string.Empty,
                    AvailableStock = RoundTo2(ToNonNegative(x.Stock))
                })
                .ToListAsync();

            var acts = await _db.WriteOffActs
                .AsNoTracking()
                .Include(x => x.Staff)
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.Ingredient)
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.WriteOffType)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.SemiFinished)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.WriteOffType)
                .OrderByDescending(x => x.Date)
                .ThenByDescending(x => x.Id)
                .Take(100)
                .ToListAsync();

            return Ok(new KitchenWriteOffBoardResponse
            {
                WriteOffTypes = writeOffTypes,
                SemiFinishedOptions = semiFinishedDtos,
                IngredientOptions = ingredientDtos,
                Acts = acts.Select(BuildWriteOffActDto).ToList()
            });
        }

        [HttpPost("write-off/acts")]
        public async Task<ActionResult<KitchenWriteOffActDto>> CreateWriteOffAct(
            [FromBody] KitchenCreateWriteOffActRequest request)
        {
            if (request == null)
            {
                return BadRequest("Тело запроса обязательно.");
            }

            request.IngredientLines ??= new List<KitchenCreateIngredientWriteOffLineRequest>();
            request.SemiFinishedLines ??= new List<KitchenCreateSemiFinishedWriteOffLineRequest>();

            if (request.IngredientLines.Count == 0 && request.SemiFinishedLines.Count == 0)
            {
                return BadRequest("Добавьте хотя бы одну строку состава акта.");
            }

            var normalizedComment = NormalizeComment(request.Comment);
            if (normalizedComment != null && normalizedComment.Length > 500)
            {
                return BadRequest("Комментарий слишком длинный. Максимум 500 символов.");
            }

            if (request.IngredientLines.Any(x => x.IngredientId <= 0) ||
                request.SemiFinishedLines.Any(x => x.SemiFinishedId <= 0))
            {
                return BadRequest("В составе акта есть некорректная позиция.");
            }

            if (request.IngredientLines.Any(x => x.Quantity <= 0m) ||
                request.SemiFinishedLines.Any(x => x.Quantity <= 0m))
            {
                return BadRequest("Количество в каждой строке должно быть больше нуля.");
            }

            var writeOffTypeIds = request.IngredientLines
                .Select(x => x.WriteOffTypeId)
                .Concat(request.SemiFinishedLines.Select(x => x.WriteOffTypeId))
                .Distinct()
                .ToList();

            if (writeOffTypeIds.Any(x => x <= 0))
            {
                return BadRequest("В составе акта есть некорректный тип списания.");
            }

            var existingWriteOffTypeIds = await _db.WriteOffTypes
                .AsNoTracking()
                .Where(x => writeOffTypeIds.Contains(x.Id))
                .Select(x => x.Id)
                .ToListAsync();
            if (existingWriteOffTypeIds.Count != writeOffTypeIds.Count)
            {
                return NotFound("Тип списания не найден.");
            }

            var ingredientIds = request.IngredientLines.Select(x => x.IngredientId).Distinct().ToList();
            var semiFinishedIds = request.SemiFinishedLines.Select(x => x.SemiFinishedId).Distinct().ToList();

            var ingredientsById = ingredientIds.Count == 0
                ? new Dictionary<int, Ingredient>()
                : await _db.Ingredients
                    .Include(x => x.UnitOfMeasure)
                    .Where(x => ingredientIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id);
            if (ingredientsById.Count != ingredientIds.Count)
            {
                return NotFound("Сырье из состава акта не найдено.");
            }

            var semiFinishedById = semiFinishedIds.Count == 0
                ? new Dictionary<int, SemiFinished>()
                : await _db.SemiFinisheds
                    .Where(x => semiFinishedIds.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id);
            if (semiFinishedById.Count != semiFinishedIds.Count)
            {
                return NotFound("Полуфабрикат из состава акта не найден.");
            }

            foreach (var group in request.IngredientLines.GroupBy(x => x.IngredientId))
            {
                var ingredient = ingredientsById[group.Key];
                var availableBase = ConvertToBaseUnits(ToNonNegative(ingredient.Stock), ingredient.UnitOfMeasureId);
                var requestedBase = group.Sum(x => ConvertToBaseUnits(RoundTo2(x.Quantity), ingredient.UnitOfMeasureId));
                if (availableBase + DecimalEpsilon < requestedBase)
                {
                    var name = string.IsNullOrWhiteSpace(ingredient.Name) ? $"Сырье #{ingredient.Id}" : ingredient.Name;
                    return BadRequest($"Недостаточно остатка сырья \"{name}\" для списания.");
                }
            }

            var preparationsBySemiFinished = semiFinishedIds.Count == 0
                ? new Dictionary<int, List<Preparation>>()
                : (await _db.Preparations
                    .Where(x =>
                        x.SemiFinishedId.HasValue &&
                        semiFinishedIds.Contains(x.SemiFinishedId.Value) &&
                        x.StockGrams.HasValue &&
                        x.StockGrams.Value > 0m)
                    .OrderBy(x => x.ProductionDate ?? DateOnly.MinValue)
                    .ThenBy(x => x.Id)
                    .ToListAsync())
                    .GroupBy(x => x.SemiFinishedId!.Value)
                    .ToDictionary(x => x.Key, x => x.ToList());

            foreach (var group in request.SemiFinishedLines.GroupBy(x => x.SemiFinishedId))
            {
                preparationsBySemiFinished.TryGetValue(group.Key, out var preparationRows);
                var available = (preparationRows ?? new List<Preparation>()).Sum(x => ToNonNegative(x.StockGrams));
                var requested = group.Sum(x => RoundTo2(x.Quantity));
                if (available + DecimalEpsilon < requested)
                {
                    var semiFinished = semiFinishedById[group.Key];
                    var name = string.IsNullOrWhiteSpace(semiFinished.Name) ? $"Полуфабрикат #{semiFinished.Id}" : semiFinished.Name;
                    return BadRequest($"Недостаточно остатка полуфабриката \"{name}\" для списания.");
                }
            }

            await using var tx = await _db.Database.BeginTransactionAsync();

            foreach (var group in request.IngredientLines.GroupBy(x => x.IngredientId))
            {
                var ingredient = ingredientsById[group.Key];
                var availableBase = ConvertToBaseUnits(ToNonNegative(ingredient.Stock), ingredient.UnitOfMeasureId);
                var requestedBase = group.Sum(x => ConvertToBaseUnits(RoundTo2(x.Quantity), ingredient.UnitOfMeasureId));
                var remainingBase = Math.Max(0m, availableBase - requestedBase);
                ingredient.Stock = RoundTo2(ConvertFromBaseUnits(remainingBase, ingredient.UnitOfMeasureId));
            }

            foreach (var group in request.SemiFinishedLines.GroupBy(x => x.SemiFinishedId))
            {
                var remaining = group.Sum(x => RoundTo2(x.Quantity));
                foreach (var preparation in preparationsBySemiFinished[group.Key])
                {
                    var stock = ToNonNegative(preparation.StockGrams);
                    if (stock <= DecimalEpsilon)
                    {
                        continue;
                    }

                    var toTake = Math.Min(stock, remaining);
                    preparation.StockGrams = RoundTo2(stock - toTake);
                    remaining -= toTake;

                    if (remaining <= DecimalEpsilon)
                    {
                        break;
                    }
                }
            }

            var depletedPreparations = preparationsBySemiFinished.Values
                .SelectMany(x => x)
                .Where(x => !x.StockGrams.HasValue || x.StockGrams.Value <= 0m)
                .ToList();
            if (depletedPreparations.Count > 0)
            {
                _db.Preparations.RemoveRange(depletedPreparations);
            }

            var act = new WriteOffAct
            {
                Date = (request.Date ?? DateTime.Now).Date,
                Comment = normalizedComment,
                StaffId = TryResolveStaffId()
            };

            foreach (var line in request.IngredientLines)
            {
                var ingredient = ingredientsById[line.IngredientId];
                act.IngredientItems.Add(new IngredientWriteOffActItem
                {
                    IngredientId = line.IngredientId,
                    Quantity = RoundTo2(line.Quantity),
                    UnitOfMeasureId = ingredient.UnitOfMeasureId,
                    WriteOffTypeId = line.WriteOffTypeId
                });
            }

            foreach (var line in request.SemiFinishedLines)
            {
                act.SemiFinishedItems.Add(new SemiFinishedWriteOffActItem
                {
                    SemiFinishedId = line.SemiFinishedId,
                    Quantity = RoundTo2(line.Quantity),
                    UnitOfMeasureId = UnitGramsId,
                    WriteOffTypeId = line.WriteOffTypeId
                });
            }

            _db.WriteOffActs.Add(act);
            await _db.SaveChangesAsync();
            await _stockService.RefreshMenuAvailabilityAsync();
            await tx.CommitAsync();

            var createdAct = await _db.WriteOffActs
                .AsNoTracking()
                .Include(x => x.Staff)
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.Ingredient)
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.WriteOffType)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.SemiFinished)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.UnitOfMeasure)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.WriteOffType)
                .FirstAsync(x => x.Id == act.Id);

            return Ok(BuildWriteOffActDto(createdAct));
        }

        [HttpDelete("write-off/acts/{actId:int}")]
        [Authorize(Roles = "Администратор")]
        public async Task<IActionResult> DeleteWriteOffAct(int actId)
        {
            if (actId <= 0)
            {
                return BadRequest("Некорректный идентификатор акта.");
            }

            var act = await _db.WriteOffActs
                .Include(x => x.IngredientItems)
                    .ThenInclude(x => x.Ingredient)
                .Include(x => x.SemiFinishedItems)
                    .ThenInclude(x => x.SemiFinished)
                .FirstOrDefaultAsync(x => x.Id == actId);

            if (act == null)
            {
                return NotFound("Акт списания не найден.");
            }

            await using var tx = await _db.Database.BeginTransactionAsync();

            foreach (var group in act.IngredientItems.GroupBy(x => x.IngredientId))
            {
                var firstLine = group.First();
                var ingredient = firstLine.Ingredient;
                if (ingredient == null)
                {
                    continue;
                }

                var currentBase = ConvertToBaseUnits(ToNonNegative(ingredient.Stock), ingredient.UnitOfMeasureId);
                var restoreBase = group.Sum(x => ConvertToBaseUnits(RoundTo2(x.Quantity), x.UnitOfMeasureId));
                ingredient.Stock = RoundTo2(ConvertFromBaseUnits(currentBase + restoreBase, ingredient.UnitOfMeasureId));
            }

            foreach (var group in act.SemiFinishedItems.GroupBy(x => x.SemiFinishedId))
            {
                var firstLine = group.First();
                var semiFinished = firstLine.SemiFinished;
                var stockGrams = RoundTo2(group.Sum(x => x.Quantity));
                if (stockGrams <= 0m)
                {
                    continue;
                }

                _db.Preparations.Add(new Preparation
                {
                    Name = semiFinished != null && !string.IsNullOrWhiteSpace(semiFinished.Name)
                        ? semiFinished.Name
                        : $"SemiFinished #{group.Key}",
                    SemiFinishedId = group.Key,
                    StockGrams = stockGrams,
                    ProductionDate = DateOnly.FromDateTime(act.Date.Date)
                });
            }

            _db.IngredientWriteOffActItems.RemoveRange(act.IngredientItems);
            _db.SemiFinishedWriteOffActItems.RemoveRange(act.SemiFinishedItems);
            _db.WriteOffActs.Remove(act);

            await _db.SaveChangesAsync();
            await _stockService.RefreshMenuAvailabilityAsync();
            await tx.CommitAsync();

            return NoContent();
        }

        private async Task<bool> MarkItemCompletedAsync(int orderId, KitchenOrderItemType itemType, int itemId)
        {
            switch (itemType)
            {
                case KitchenOrderItemType.Dish:
                    {
                        var entity = await _db.OrderDishItems
                            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrderId == orderId);

                        if (entity == null)
                        {
                            return false;
                        }

                        entity.IsCompleted = true;
                        break;
                    }
                case KitchenOrderItemType.Drink:
                    {
                        var entity = await _db.OrderDrinkItems
                            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrderId == orderId);

                        if (entity == null)
                        {
                            return false;
                        }

                        entity.IsCompleted = true;
                        break;
                    }
                case KitchenOrderItemType.Topping:
                    {
                        var entity = await _db.OrderToppingItems
                            .FirstOrDefaultAsync(i => i.Id == itemId && i.OrderId == orderId);

                        if (entity == null)
                        {
                            return false;
                        }

                        entity.IsCompleted = true;
                        break;
                    }
                default:
                    return false;
            }

            await _db.SaveChangesAsync();
            return true;
        }

        private async Task MarkAllOrderItemsCompletedAsync(int orderId)
        {
            await _db.OrderDishItems
                .Where(i => i.OrderId == orderId && !i.IsCompleted)
                .ExecuteUpdateAsync(setters => setters.SetProperty(i => i.IsCompleted, true));

            await _db.OrderDrinkItems
                .Where(i => i.OrderId == orderId && !i.IsCompleted)
                .ExecuteUpdateAsync(setters => setters.SetProperty(i => i.IsCompleted, true));

            await _db.OrderToppingItems
                .Where(i => i.OrderId == orderId && !i.IsCompleted)
                .ExecuteUpdateAsync(setters => setters.SetProperty(i => i.IsCompleted, true));
        }

        private async Task<bool> TryMarkOrderReadyIfCompletedAsync(int orderId)
        {
            var hasPendingDishItems = await _db.OrderDishItems.AnyAsync(i => i.OrderId == orderId && !i.IsCompleted);
            if (hasPendingDishItems)
            {
                return false;
            }

            var hasPendingDrinkItems = await _db.OrderDrinkItems.AnyAsync(i => i.OrderId == orderId && !i.IsCompleted);
            if (hasPendingDrinkItems)
            {
                return false;
            }

            var hasPendingToppingItems = await _db.OrderToppingItems.AnyAsync(i => i.OrderId == orderId && !i.IsCompleted);
            if (hasPendingToppingItems)
            {
                return false;
            }

            var readyStatus = await ResolveReadyStatusAsync();
            if (readyStatus == null)
            {
                throw new InvalidOperationException("Статус готовности заказа не найден");
            }

            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == orderId);
            if (order == null)
            {
                return false;
            }

            if (order.StatusId == readyStatus.Value.Id)
            {
                return true;
            }

            order.StatusId = readyStatus.Value.Id;
            await _db.SaveChangesAsync();
            return true;
        }

        private async Task<string> GetOrderStatusNameAsync(int orderId)
        {
            var status = await _db.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .Select(o => o.Status != null ? o.Status.Name : null)
                .FirstOrDefaultAsync();

            return status ?? string.Empty;
        }

        private async Task<(int Id, string Name)?> ResolveReadyStatusAsync()
        {
            var status = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name != null &&
                            (EF.Functions.Like(s.Name.ToLower(), $"%{ReadyStatusTokenRu}%") ||
                             EF.Functions.Like(s.Name.ToLower(), $"%{ReadyStatusTokenEn}%")))
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (status != null)
            {
                return (status.Id, status.Name ?? ReadyStatusNameRu);
            }

            var fallback = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name == ReadyStatusNameRu)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (fallback != null)
            {
                return (fallback.Id, fallback.Name ?? ReadyStatusNameRu);
            }

            return null;
        }

        private async Task<(int Id, string Name)?> ResolveCancelledStatusAsync()
        {
            var status = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name != null &&
                            (EF.Functions.Like(s.Name.ToLower(), $"%{CancelledStatusTokenRu}%") ||
                             EF.Functions.Like(s.Name.ToLower(), $"%{CancelledStatusTokenEn}%")))
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (status != null)
            {
                return (status.Id, status.Name ?? CancelledStatusNameRu);
            }

            var fallback = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name == CancelledStatusNameRu)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (fallback != null)
            {
                return (fallback.Id, fallback.Name ?? CancelledStatusNameRu);
            }

            return null;
        }

        private async Task<TechnicalCardSource?> LoadTechnicalCardSourceAsync(
            int orderId,
            KitchenOrderItemType itemType,
            int itemId)
        {
            switch (itemType)
            {
                case KitchenOrderItemType.Dish:
                    {
                        return await _db.OrderDishItems
                            .AsNoTracking()
                            .Where(i => i.Id == itemId && i.OrderId == orderId)
                            .Select(i => new TechnicalCardSource
                            {
                                TechnicalCardId = i.Dish != null ? i.Dish.TechnicalCardId : null,
                                ItemName = i.Dish != null ? i.Dish.Name : null
                            })
                            .FirstOrDefaultAsync();
                    }
                case KitchenOrderItemType.Drink:
                    {
                        return await _db.OrderDrinkItems
                            .AsNoTracking()
                            .Where(i => i.Id == itemId && i.OrderId == orderId)
                            .Select(i => new TechnicalCardSource
                            {
                                TechnicalCardId = i.Drink != null ? i.Drink.TechnicalCardId : null,
                                ItemName = i.Drink != null ? i.Drink.Name : null
                            })
                            .FirstOrDefaultAsync();
                    }
                case KitchenOrderItemType.Topping:
                    {
                        return await _db.OrderToppingItems
                            .AsNoTracking()
                            .Where(i => i.Id == itemId && i.OrderId == orderId)
                            .Select(i => new TechnicalCardSource
                            {
                                TechnicalCardId = i.Topping != null ? i.Topping.TechnicalCardId : null,
                                ItemName = i.Topping != null ? i.Topping.Name : null
                            })
                            .FirstOrDefaultAsync();
                    }
                default:
                    return null;
            }
        }

        private async Task<List<KitchenTechnicalCardComponentDto>> LoadTechnicalCardComponentsAsync(int technicalCardId)
        {
            var ingredientRows = await _db.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(c => c.TechnicalCardId.HasValue && c.TechnicalCardId.Value == technicalCardId)
                .OrderBy(c => c.Id)
                .Select(c => new
                {
                    Name = c.Ingredient != null ? c.Ingredient.Name : null,
                    Unit = c.UnitOfMeasure != null ? c.UnitOfMeasure.Name : null,
                    c.OutputWeight,
                    c.NetWeight,
                    c.GrossWeight
                })
                .ToListAsync();

            var semiFinishedRows = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(c => c.TechnicalCardId.HasValue && c.TechnicalCardId.Value == technicalCardId)
                .OrderBy(c => c.Id)
                .Select(c => new
                {
                    Name = c.SemiFinished != null ? c.SemiFinished.Name : null,
                    Unit = c.UnitOfMeasure != null ? c.UnitOfMeasure.Name : null,
                    c.OutputWeight,
                    c.NetWeight,
                    c.GrossWeight
                })
                .ToListAsync();

            var components = new List<KitchenTechnicalCardComponentDto>(ingredientRows.Count + semiFinishedRows.Count);

            components.AddRange(ingredientRows.Select(r => new KitchenTechnicalCardComponentDto
            {
                Name = r.Name ?? "Без названия",
                Weight = PickComponentWeight(r.OutputWeight, r.NetWeight, r.GrossWeight),
                Unit = r.Unit ?? string.Empty
            }));

            components.AddRange(semiFinishedRows.Select(r => new KitchenTechnicalCardComponentDto
            {
                Name = r.Name ?? "Без названия",
                Weight = PickComponentWeight(r.OutputWeight, r.NetWeight, r.GrossWeight),
                Unit = r.Unit ?? string.Empty
            }));

            return components;
        }

        private async Task<KitchenTechnicalCardEditResponse?> BuildTechnicalCardEditResponseAsync(int technicalCardId)
        {
            var technicalCard = await _db.TechnicalCards
                .AsNoTracking()
                .Where(c => c.Id == technicalCardId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (technicalCard == null)
            {
                return null;
            }

            var ingredientLines = await _db.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(c => c.TechnicalCardId == technicalCardId && c.IngredientId.HasValue)
                .OrderBy(c => c.Id)
                .Select(c => new KitchenTechnicalCardCompositionLineDto
                {
                    ItemId = c.IngredientId!.Value,
                    UnitOfMeasureId = c.UnitOfMeasureId,
                    GrossWeight = c.GrossWeight,
                    ColdLossPercent = c.ColdLossPercent,
                    NetWeight = c.NetWeight,
                    HotLossPercent = c.HotLossPercent,
                    OutputWeight = c.OutputWeight
                })
                .ToListAsync();

            var semiFinishedLines = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(c => c.TechnicalCardId == technicalCardId && c.SemiFinishedId.HasValue)
                .OrderBy(c => c.Id)
                .Select(c => new KitchenTechnicalCardCompositionLineDto
                {
                    ItemId = c.SemiFinishedId!.Value,
                    UnitOfMeasureId = c.UnitOfMeasureId,
                    GrossWeight = c.GrossWeight,
                    ColdLossPercent = c.ColdLossPercent,
                    NetWeight = c.NetWeight,
                    HotLossPercent = c.HotLossPercent,
                    OutputWeight = c.OutputWeight
                })
                .ToListAsync();

            var bindings = new List<KitchenTechnicalCardBindingDto>();
            bindings.AddRange(await _db.Dishes
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => new KitchenTechnicalCardBindingDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.Dish,
                    ItemId = x.Id
                })
                .ToListAsync());
            bindings.AddRange(await _db.Drinks
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => new KitchenTechnicalCardBindingDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.Drink,
                    ItemId = x.Id
                })
                .ToListAsync());
            bindings.AddRange(await _db.ToppingsAndSyrups
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => new KitchenTechnicalCardBindingDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.Topping,
                    ItemId = x.Id
                })
                .ToListAsync());
            bindings.AddRange(await _db.SemiFinisheds
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => new KitchenTechnicalCardBindingDto
                {
                    Kind = KitchenTechnicalCardBindingKinds.SemiFinished,
                    ItemId = x.Id
                })
                .ToListAsync());

            return new KitchenTechnicalCardEditResponse
            {
                TechnicalCardId = technicalCard.Id,
                CardName = technicalCard.Name ?? string.Empty,
                Description = technicalCard.Description ?? string.Empty,
                IngredientLines = ingredientLines,
                SemiFinishedLines = semiFinishedLines,
                Bindings = bindings
            };
        }

        private async Task<string?> ValidateTechnicalCardReferencesAsync(KitchenTechnicalCardUpsertRequest request)
        {
            var ingredientIds = request.IngredientLines.Select(x => x.ItemId).Distinct().ToList();
            var semiFinishedCompositionIds = request.SemiFinishedLines.Select(x => x.ItemId).Distinct().ToList();
            var unitIds = request.IngredientLines
                .Concat(request.SemiFinishedLines)
                .Where(x => x.UnitOfMeasureId.HasValue)
                .Select(x => x.UnitOfMeasureId!.Value)
                .Distinct()
                .ToList();

            if (ingredientIds.Count > 0)
            {
                var existingCount = await _db.Ingredients.CountAsync(x => ingredientIds.Contains(x.Id));
                if (existingCount != ingredientIds.Count)
                {
                    return "В составе указаны несуществующие ингредиенты.";
                }
            }

            if (semiFinishedCompositionIds.Count > 0)
            {
                var existingCount = await _db.SemiFinisheds.CountAsync(x => semiFinishedCompositionIds.Contains(x.Id));
                if (existingCount != semiFinishedCompositionIds.Count)
                {
                    return "В составе указаны несуществующие полуфабрикаты.";
                }
            }

            if (unitIds.Count > 0)
            {
                var existingCount = await _db.UnitsOfMeasures.CountAsync(x => unitIds.Contains(x.Id));
                if (existingCount != unitIds.Count)
                {
                    return "В составе указаны несуществующие единицы измерения.";
                }
            }

            return null;
        }

        private async Task ReplaceTechnicalCardDetailsAsync(
            int technicalCardId,
            KitchenTechnicalCardUpsertRequest request)
        {
            var oldIngredientRows = await _db.TechnicalCardIngredientCompositions
                .Where(x => x.TechnicalCardId == technicalCardId)
                .ToListAsync();
            var oldSemiFinishedRows = await _db.TechnicalCardSemiFinishedCompositions
                .Where(x => x.TechnicalCardId == technicalCardId)
                .ToListAsync();

            _db.TechnicalCardIngredientCompositions.RemoveRange(oldIngredientRows);
            _db.TechnicalCardSemiFinishedCompositions.RemoveRange(oldSemiFinishedRows);

            _db.TechnicalCardIngredientCompositions.AddRange(request.IngredientLines.Select(line => new TechnicalCardIngredientComposition
            {
                TechnicalCardId = technicalCardId,
                IngredientId = line.ItemId,
                UnitOfMeasureId = line.UnitOfMeasureId,
                GrossWeight = line.GrossWeight,
                ColdLossPercent = line.ColdLossPercent,
                NetWeight = line.NetWeight,
                HotLossPercent = line.HotLossPercent,
                OutputWeight = line.OutputWeight
            }));

            _db.TechnicalCardSemiFinishedCompositions.AddRange(request.SemiFinishedLines.Select(line => new TechnicalCardSemiFinishedComposition
            {
                TechnicalCardId = technicalCardId,
                SemiFinishedId = line.ItemId,
                UnitOfMeasureId = line.UnitOfMeasureId,
                GrossWeight = line.GrossWeight,
                ColdLossPercent = line.ColdLossPercent,
                NetWeight = line.NetWeight,
                HotLossPercent = line.HotLossPercent,
                OutputWeight = line.OutputWeight
            }));

            await _db.SaveChangesAsync();
        }

        private async Task<List<string>> LoadTechnicalCardLinkedNamesAsync(int technicalCardId)
        {
            var names = new List<string>();
            names.AddRange(await _db.Dishes
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => "блюдо: " + (x.Name ?? $"#{x.Id}"))
                .ToListAsync());
            names.AddRange(await _db.Drinks
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => "напиток: " + (x.Name ?? $"#{x.Id}"))
                .ToListAsync());
            names.AddRange(await _db.ToppingsAndSyrups
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => "добавка: " + (x.Name ?? $"#{x.Id}"))
                .ToListAsync());
            names.AddRange(await _db.SemiFinisheds
                .AsNoTracking()
                .Where(x => x.TechnicalCardId == technicalCardId)
                .Select(x => "полуфабрикат: " + (x.Name ?? $"#{x.Id}"))
                .ToListAsync());

            return names;
        }

        private static string? ValidateTechnicalCardRequest(KitchenTechnicalCardUpsertRequest request)
        {
            if (request == null)
            {
                return "Не переданы данные техкарты.";
            }

            if (string.IsNullOrWhiteSpace(request.CardName))
            {
                return "Укажите название техкарты.";
            }

            request.IngredientLines ??= new List<KitchenTechnicalCardCompositionLineDto>();
            request.SemiFinishedLines ??= new List<KitchenTechnicalCardCompositionLineDto>();
            request.Bindings ??= new List<KitchenTechnicalCardBindingDto>();

            if (request.IngredientLines.Any(x => x.ItemId <= 0) ||
                request.SemiFinishedLines.Any(x => x.ItemId <= 0))
            {
                return "В составе есть строки без выбранной позиции.";
            }

            return null;
        }

        private static string? NormalizeDescription(string? description)
        {
            return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
        }

        private async Task<KitchenTechnicalCardResponse?> BuildTechnicalCardResponseAsync(int technicalCardId, string? fallbackCardName)
        {
            var technicalCard = await _db.TechnicalCards
                .AsNoTracking()
                .Where(c => c.Id == technicalCardId)
                .Select(c => new
                {
                    c.Id,
                    c.Name,
                    c.Description
                })
                .FirstOrDefaultAsync();

            if (technicalCard == null)
            {
                return null;
            }

            var components = await LoadTechnicalCardComponentsAsync(technicalCardId);

            return new KitchenTechnicalCardResponse
            {
                TechnicalCardId = technicalCard.Id,
                CardName = string.IsNullOrWhiteSpace(technicalCard.Name)
                    ? (fallbackCardName ?? $"Technical card #{technicalCard.Id}")
                    : technicalCard.Name,
                Description = technicalCard.Description ?? string.Empty,
                Components = components
            };
        }

        private static decimal PickComponentWeight(decimal? outputWeight, decimal? netWeight, decimal? grossWeight)
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

        private static KitchenStopListPositionDto BuildStopListPosition(
            string itemType,
            int itemId,
            string? itemName,
            string? categoryName,
            string? volumeWeight,
            bool isAvailable,
            int? technicalCardId,
            IReadOnlyDictionary<string, decimal> limitsByKey,
            IReadOnlyDictionary<int, decimal?> autoPortionsByTechnicalCard)
        {
            var itemTypeToken = itemType?.Trim().ToLowerInvariant() ?? string.Empty;
            var key = BuildMenuItemLimitKey(itemTypeToken, itemId);
            var manualRemainingPortions = limitsByKey.TryGetValue(key, out var manualValue)
                ? manualValue
                : (decimal?)null;

            decimal? autoAvailablePortions = null;
            if (technicalCardId.HasValue && autoPortionsByTechnicalCard.TryGetValue(technicalCardId.Value, out var autoValue))
            {
                autoAvailablePortions = autoValue;
            }

            var effectiveRemainingPortions = ResolveEffectiveRemainingPortions(
                manualRemainingPortions,
                autoAvailablePortions);

            return new KitchenStopListPositionDto
            {
                ItemType = itemTypeToken,
                ItemId = itemId,
                Name = string.IsNullOrWhiteSpace(itemName)
                    ? $"Позиция #{itemId}"
                    : itemName,
                Category = categoryName ?? string.Empty,
                VolumeWeight = volumeWeight ?? string.Empty,
                IsAvailable = isAvailable,
                ManualRemainingPortions = manualRemainingPortions,
                AutoAvailablePortions = autoAvailablePortions,
                EffectiveRemainingPortions = effectiveRemainingPortions
            };
        }

        private bool IsStopListPositionVisibleForCurrentRole(KitchenStopListPositionDto position)
        {
            if (IsCurrentUserInRole(AdminRole))
            {
                return true;
            }

            var itemType = position.ItemType?.Trim().ToLowerInvariant() ?? string.Empty;
            if (IsCurrentUserInRole(CookRole))
            {
                return itemType == KitchenItemTypes.Dish ||
                       (itemType == KitchenItemTypes.Topping &&
                        CategoryContainsToken(position.Category, DishToppingCategoryToken));
            }

            if (IsCurrentUserInRole(BaristaRole))
            {
                return itemType == KitchenItemTypes.Drink ||
                       (itemType == KitchenItemTypes.Topping &&
                        CategoryContainsToken(position.Category, DrinkToppingCategoryToken));
            }

            return false;
        }

        private async Task<bool> CanCurrentRoleManageStopListItemAsync(KitchenOrderItemType itemType, int itemId)
        {
            if (IsCurrentUserInRole(AdminRole))
            {
                return true;
            }

            if (IsCurrentUserInRole(CookRole))
            {
                return itemType switch
                {
                    KitchenOrderItemType.Dish => true,
                    KitchenOrderItemType.Topping => await ToppingCategoryContainsTokenAsync(itemId, DishToppingCategoryToken),
                    _ => false
                };
            }

            if (IsCurrentUserInRole(BaristaRole))
            {
                return itemType switch
                {
                    KitchenOrderItemType.Drink => true,
                    KitchenOrderItemType.Topping => await ToppingCategoryContainsTokenAsync(itemId, DrinkToppingCategoryToken),
                    _ => false
                };
            }

            return false;
        }

        private async Task<bool> ToppingCategoryContainsTokenAsync(int toppingId, string token)
        {
            var categoryName = await _db.ToppingsAndSyrups
                .AsNoTracking()
                .Where(t => t.Id == toppingId)
                .Select(t => t.Category != null ? t.Category.Name : null)
                .FirstOrDefaultAsync();

            return CategoryContainsToken(categoryName, token);
        }

        private static bool CategoryContainsToken(string? categoryName, string token)
        {
            return (categoryName ?? string.Empty)
                .Trim()
                .ToLowerInvariant()
                .Contains(token);
        }

        private async Task<Dictionary<int, decimal>> BuildOutputWeightByTechnicalCardAsync(IReadOnlyCollection<int> technicalCardIds)
        {
            if (technicalCardIds.Count == 0)
            {
                return new Dictionary<int, decimal>();
            }

            var ingredientRows = await _db.TechnicalCardIngredientCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId.HasValue && technicalCardIds.Contains(x.TechnicalCardId.Value))
                .Select(x => new
                {
                    TechnicalCardId = x.TechnicalCardId!.Value,
                    x.OutputWeight,
                    x.NetWeight,
                    x.GrossWeight
                })
                .ToListAsync();

            var semiFinishedRows = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(x => x.TechnicalCardId.HasValue && technicalCardIds.Contains(x.TechnicalCardId.Value))
                .Select(x => new
                {
                    TechnicalCardId = x.TechnicalCardId!.Value,
                    x.OutputWeight,
                    x.NetWeight,
                    x.GrossWeight
                })
                .ToListAsync();

            return ingredientRows
                .Select(x => new
                {
                    x.TechnicalCardId,
                    Weight = PickComponentWeight(x.OutputWeight, x.NetWeight, x.GrossWeight)
                })
                .Concat(semiFinishedRows.Select(x => new
                {
                    x.TechnicalCardId,
                    Weight = PickComponentWeight(x.OutputWeight, x.NetWeight, x.GrossWeight)
                }))
                .GroupBy(x => x.TechnicalCardId)
                .ToDictionary(x => x.Key, x => RoundTo2(x.Sum(y => y.Weight)));
        }

        private static string BuildTechnicalCardVolumeWeight(
            int? technicalCardId,
            IReadOnlyDictionary<int, decimal> outputByTechnicalCard,
            string? unitName)
        {
            if (!technicalCardId.HasValue ||
                !outputByTechnicalCard.TryGetValue(technicalCardId.Value, out var value) ||
                value <= 0m)
            {
                return string.Empty;
            }

            return BuildVolumeWeight(value, unitName);
        }

        private static string BuildVolumeWeight(decimal? quantity, string? unitName)
        {
            if (!quantity.HasValue || quantity.Value <= 0m)
            {
                return string.Empty;
            }

            var unit = ToShortUnitName(unitName);
            return string.IsNullOrWhiteSpace(unit)
                ? quantity.Value.ToString("0.##", CultureInfo.CurrentCulture)
                : $"{quantity.Value.ToString("0.##", CultureInfo.CurrentCulture)} {unit}";
        }

        private static string ToShortUnitName(string? unitName)
        {
            var normalized = (unitName ?? string.Empty).Trim().ToLowerInvariant();
            return normalized switch
            {
                "килограмм" or "килограмма" or "килограммы" or "кг." => "кг",
                "грамм" or "грамма" or "граммы" or "гр" or "гр." or "г." => "г",
                "литр" or "литра" or "литры" or "л." => "л",
                "миллилитр" or "миллилитра" or "миллилитры" or "мл." => "мл",
                "штука" or "штуки" or "штук" or "шт." => "шт",
                _ => (unitName ?? string.Empty).Trim()
            };
        }

        private static decimal? ResolveEffectiveRemainingPortions(decimal? manualRemainingPortions, decimal? autoAvailablePortions)
        {
            if (manualRemainingPortions.HasValue && autoAvailablePortions.HasValue)
            {
                return Math.Min(manualRemainingPortions.Value, autoAvailablePortions.Value);
            }

            return manualRemainingPortions ?? autoAvailablePortions;
        }

        private async Task<Dictionary<int, decimal?>> BuildAutoAvailablePortionsByTechnicalCardAsync(
            IReadOnlyCollection<int> technicalCardIds)
        {
            if (technicalCardIds.Count == 0)
            {
                return new Dictionary<int, decimal?>();
            }

            var requirementRows = await _db.TechnicalCardSemiFinishedCompositions
                .AsNoTracking()
                .Where(x =>
                    x.TechnicalCardId.HasValue &&
                    x.SemiFinishedId.HasValue &&
                    technicalCardIds.Contains(x.TechnicalCardId.Value))
                .Select(x => new
                {
                    TechnicalCardId = x.TechnicalCardId!.Value,
                    SemiFinishedId = x.SemiFinishedId!.Value,
                    RequiredBase = ConvertToBaseUnits(
                        PickComponentWeight(x.OutputWeight, x.NetWeight, x.GrossWeight),
                        x.UnitOfMeasureId)
                })
                .ToListAsync();

            var requirementsByCard = requirementRows
                .Where(x => x.RequiredBase > DecimalEpsilon)
                .GroupBy(x => new { x.TechnicalCardId, x.SemiFinishedId })
                .Select(g => new
                {
                    g.Key.TechnicalCardId,
                    g.Key.SemiFinishedId,
                    RequiredBase = g.Sum(x => x.RequiredBase)
                })
                .GroupBy(x => x.TechnicalCardId)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => new CardSemiFinishedRequirement(
                        x.SemiFinishedId,
                        x.RequiredBase)).ToList());

            var semiFinishedIds = requirementsByCard.Values
                .SelectMany(x => x)
                .Select(x => x.SemiFinishedId)
                .Distinct()
                .ToList();

            var stockBySemiFinished = semiFinishedIds.Count == 0
                ? new Dictionary<int, decimal>()
                : await _db.Preparations
                    .AsNoTracking()
                    .Where(x =>
                        x.SemiFinishedId.HasValue &&
                        x.StockGrams.HasValue &&
                        x.StockGrams.Value > 0m &&
                        semiFinishedIds.Contains(x.SemiFinishedId.Value))
                    .GroupBy(x => x.SemiFinishedId!.Value)
                    .Select(g => new
                    {
                        SemiFinishedId = g.Key,
                        Available = g.Sum(x => x.StockGrams ?? 0m)
                    })
                    .ToDictionaryAsync(x => x.SemiFinishedId, x => Math.Max(0m, x.Available));

            var result = technicalCardIds
                .Distinct()
                .ToDictionary(x => x, _ => (decimal?)null);

            foreach (var technicalCardId in technicalCardIds)
            {
                if (!requirementsByCard.TryGetValue(technicalCardId, out var requirements) || requirements.Count == 0)
                {
                    result[technicalCardId] = null;
                    continue;
                }

                decimal? minAvailablePortions = null;

                foreach (var requirement in requirements)
                {
                    if (requirement.RequiredBase <= DecimalEpsilon)
                    {
                        continue;
                    }

                    stockBySemiFinished.TryGetValue(requirement.SemiFinishedId, out var stockBase);
                    var availablePortions = Math.Floor(stockBase / requirement.RequiredBase);
                    if (availablePortions < 0m)
                    {
                        availablePortions = 0m;
                    }

                    minAvailablePortions = !minAvailablePortions.HasValue
                        ? availablePortions
                        : Math.Min(minAvailablePortions.Value, availablePortions);
                }

                result[technicalCardId] = minAvailablePortions.HasValue
                    ? Math.Max(0m, minAvailablePortions.Value)
                    : null;
            }

            return result;
        }

        private async Task UpsertMenuItemPortionLimitAsync(
            string itemTypeToken,
            int itemId,
            decimal remainingPortions,
            DateTime now)
        {
            var row = await _db.MenuItemPortionLimits
                .FirstOrDefaultAsync(x =>
                    x.ItemType == itemTypeToken &&
                    x.ItemId == itemId);

            if (row == null)
            {
                _db.MenuItemPortionLimits.Add(new MenuItemPortionLimit
                {
                    ItemType = itemTypeToken,
                    ItemId = itemId,
                    RemainingPortions = remainingPortions,
                    CreatedAt = now,
                    UpdatedAt = now
                });
                return;
            }

            row.RemainingPortions = remainingPortions;
            row.UpdatedAt = now;
        }

        private static string BuildMenuItemLimitKey(string itemType, int itemId)
        {
            return $"{itemType.Trim().ToLowerInvariant()}:{itemId}";
        }

        private static decimal ConvertToBaseUnits(decimal value, int? unitOfMeasureId)
        {
            if (value <= 0m)
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
            if (value <= 0m)
            {
                return 0m;
            }

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

        private static decimal RoundTo2(decimal value)
        {
            return Math.Round(value, 2, MidpointRounding.AwayFromZero);
        }

        private static decimal ToNonNegative(decimal? value)
        {
            return value.HasValue && value.Value > 0m
                ? value.Value
                : 0m;
        }

        private static KitchenWriteOffActDto BuildWriteOffActDto(WriteOffAct act)
        {
            return new KitchenWriteOffActDto
            {
                ActId = act.Id,
                Date = act.Date,
                Comment = act.Comment ?? string.Empty,
                StaffId = act.StaffId,
                StaffFullName = act.Staff != null && !string.IsNullOrWhiteSpace(act.Staff.FullName)
                    ? act.Staff.FullName!
                    : string.Empty,
                IngredientLines = act.IngredientItems
                    .OrderBy(x => x.Id)
                    .Select(x => new KitchenWriteOffActLineDto
                    {
                        LineId = x.Id,
                        ItemId = x.IngredientId,
                        ItemName = string.IsNullOrWhiteSpace(x.Ingredient?.Name)
                            ? $"Сырье #{x.IngredientId}"
                            : x.Ingredient.Name!,
                        Quantity = RoundTo2(x.Quantity),
                        UnitOfMeasureId = x.UnitOfMeasureId,
                        UnitName = x.UnitOfMeasure?.Name ?? string.Empty,
                        WriteOffTypeId = x.WriteOffTypeId,
                        WriteOffTypeName = x.WriteOffType?.Name ?? string.Empty
                    })
                    .ToList(),
                SemiFinishedLines = act.SemiFinishedItems
                    .OrderBy(x => x.Id)
                    .Select(x => new KitchenWriteOffActLineDto
                    {
                        LineId = x.Id,
                        ItemId = x.SemiFinishedId,
                        ItemName = string.IsNullOrWhiteSpace(x.SemiFinished?.Name)
                            ? $"Полуфабрикат #{x.SemiFinishedId}"
                            : x.SemiFinished.Name!,
                        Quantity = RoundTo2(x.Quantity),
                        UnitOfMeasureId = x.UnitOfMeasureId,
                        UnitName = x.UnitOfMeasure?.Name ?? string.Empty,
                        WriteOffTypeId = x.WriteOffTypeId,
                        WriteOffTypeName = x.WriteOffType?.Name ?? string.Empty
                    })
                    .ToList()
            };
        }

        private int? TryResolveStaffId()
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var staffId) && staffId > 0
                ? staffId
                : (int?)null;
        }

        private async Task<bool> SetMenuItemAvailabilityAsync(
            KitchenOrderItemType itemType,
            int itemId,
            bool isAvailable,
            bool saveChanges = true)
        {
            switch (itemType)
            {
                case KitchenOrderItemType.Dish:
                    {
                        var dish = await _db.Dishes.FirstOrDefaultAsync(x => x.Id == itemId);
                        if (dish == null)
                        {
                            return false;
                        }

                        dish.IsAvailable = isAvailable;
                        break;
                    }
                case KitchenOrderItemType.Drink:
                    {
                        var drink = await _db.Drinks.FirstOrDefaultAsync(x => x.Id == itemId);
                        if (drink == null)
                        {
                            return false;
                        }

                        drink.IsAvailable = isAvailable;
                        break;
                    }
                case KitchenOrderItemType.Topping:
                    {
                        var topping = await _db.ToppingsAndSyrups.FirstOrDefaultAsync(x => x.Id == itemId);
                        if (topping == null)
                        {
                            return false;
                        }

                        topping.IsAvailable = isAvailable;
                        break;
                    }
                default:
                    return false;
            }

            if (saveChanges)
            {
                await _db.SaveChangesAsync();
            }

            return true;
        }

        private static bool TryParseItemType(string itemTypeToken, out KitchenOrderItemType itemType)
        {
            var normalized = itemTypeToken?.Trim().ToLowerInvariant() ?? string.Empty;

            switch (normalized)
            {
                case KitchenItemTypes.Dish:
                case "блюдо":
                case "блюда":
                    itemType = KitchenOrderItemType.Dish;
                    return true;
                case KitchenItemTypes.Drink:
                case "напиток":
                case "напитки":
                    itemType = KitchenOrderItemType.Drink;
                    return true;
                case KitchenItemTypes.Topping:
                case "добавка":
                case "добавки":
                    itemType = KitchenOrderItemType.Topping;
                    return true;
                default:
                    itemType = KitchenOrderItemType.Dish;
                    return false;
            }
        }

        private static string ToItemTypeToken(KitchenOrderItemType itemType)
        {
            return itemType switch
            {
                KitchenOrderItemType.Dish => KitchenItemTypes.Dish,
                KitchenOrderItemType.Drink => KitchenItemTypes.Drink,
                KitchenOrderItemType.Topping => KitchenItemTypes.Topping,
                _ => string.Empty
            };
        }

        private static string? NormalizeKitchenOrdersStatus(string? status)
        {
            var normalized = status?.Trim().ToLowerInvariant() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(normalized) ||
                normalized == KitchenOrdersStatusActive ||
                normalized.Contains(ActiveStatusTokenRu) ||
                normalized.Contains(ActiveStatusTokenEn))
            {
                return KitchenOrdersStatusActive;
            }

            if (normalized == KitchenOrdersStatusReady ||
                normalized.Contains(ReadyStatusTokenRu) ||
                normalized.Contains(ReadyStatusTokenEn))
            {
                return KitchenOrdersStatusReady;
            }

            return null;
        }

        private static bool IsWithinPickupWindow(DateTime pickupAt, DateTime now, TimeSpan pickupWindow)
        {
            var delta = pickupAt - now;
            if (delta < TimeSpan.Zero)
            {
                delta = delta.Negate();
            }

            return delta <= pickupWindow;
        }

        private static bool IsSystemRecommendationComment(string? comment)
        {
            return string.Equals(comment, SystemRecommendationComment, StringComparison.Ordinal) ||
                   IsSnoozedSystemRecommendationComment(comment);
        }

        private static bool IsSnoozedSystemRecommendationComment(string? comment)
        {
            return TryParseSnoozedSystemRecommendationUntil(comment, out _);
        }

        private static bool TryParseSnoozedSystemRecommendationUntil(string? comment, out DateOnly snoozedUntilDate)
        {
            snoozedUntilDate = default;

            if (string.IsNullOrWhiteSpace(comment))
            {
                return false;
            }

            if (!comment.StartsWith(SystemRecommendationSnoozePrefix, StringComparison.Ordinal))
            {
                return false;
            }

            var rawDate = comment.Substring(SystemRecommendationSnoozePrefix.Length).Trim();
            return DateOnly.TryParseExact(
                rawDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out snoozedUntilDate);
        }

        private static string BuildSnoozedSystemRecommendationComment(DateOnly snoozedUntilDate)
        {
            return $"{SystemRecommendationSnoozePrefix}{snoozedUntilDate:yyyy-MM-dd}";
        }

        private static string? NormalizeComment(string? comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
            {
                return null;
            }

            return comment.Trim();
        }

        private bool IsCurrentUserInRole(string role)
        {
            return User.IsInRole(role);
        }

        private static string? NormalizeTaskText(string? taskText)
        {
            if (string.IsNullOrWhiteSpace(taskText))
            {
                return null;
            }

            return taskText.Trim();
        }

        private enum KitchenOrderItemType
        {
            Dish,
            Drink,
            Topping
        }

        private sealed class DishSource
        {
            public int ItemId { get; set; }
            public int OrderId { get; set; }
            public string? Name { get; set; }
            public decimal Quantity { get; set; }
        }

        private sealed class DishToppingSource
        {
            public int OrderDishItemId { get; set; }
            public string? Name { get; set; }
            public decimal Quantity { get; set; }
        }

        private sealed class DrinkSource
        {
            public int ItemId { get; set; }
            public int OrderId { get; set; }
            public string? Name { get; set; }
            public decimal Quantity { get; set; }
        }

        private sealed class DrinkToppingSource
        {
            public int OrderDrinkItemId { get; set; }
            public string? Name { get; set; }
            public decimal Quantity { get; set; }
        }

        private sealed class TechnicalCardSource
        {
            public int? TechnicalCardId { get; set; }
            public string? ItemName { get; set; }
        }

        private sealed class CardSemiFinishedRequirement
        {
            public CardSemiFinishedRequirement(int semiFinishedId, decimal requiredBase)
            {
                SemiFinishedId = semiFinishedId;
                RequiredBase = requiredBase;
            }

            public int SemiFinishedId { get; }
            public decimal RequiredBase { get; }
        }
    }
}
