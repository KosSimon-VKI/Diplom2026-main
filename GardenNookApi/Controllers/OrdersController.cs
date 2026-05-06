using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using GardenNookApi.Entities;
using GardenNookApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Kitchen;
using TransferModels.Orders;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IPreparationStockService _stockService;
        private readonly IPickupSchedulingService _pickupSchedulingService;

        // --- Правила категорий по кол-ву заказов ---
        // Под себя легко поменяешь.
        private const int RegularFromOrdersCount = 5;  // >= 5 => "Постоянный"

        // Id категорий из твоего seed:
        private const int ClientCategoryNewId = 1;       // "Новый"
        private const int ClientCategoryRegularId = 2;   // "Постоянный"
        private const int ClientCategorySpecialId = 3;   // "Особый"
        private const int ClientCategoryNoneId = 4;      // "Без категории"

        // Id скидок из твоего seed:
        private const int DiscountNewId = 1;       // 15%
        private const int DiscountRegularId = 2;   // 7%
        private const int DiscountSpecialId = 3;   // 20%

        private const string OrderStatusCreatedName = "В процессе";
        private const string OrderStatusCreatedToken = "процесс";
        private const string InactiveCategoryName = "Неактивные";
        private const int MilkCategoryId = 10;
        private const int CoffeeCategoryId = 2;
        private const string PreferredMilkModifierName = "КОРОВЬЕ МОЛОКО";
        private const string PreferredCoffeeModifierName = "Кофе в зернах ТАВ Galaxy";
        private const string WpfGuestClientFullName = "Гость WPF";
        private const string WpfGuestClientPhone = "79990000000";
        private const string WpfGuestClientPassword = "wpf-guest";
        private const decimal DecimalEpsilon = 0.000001m;
        private static readonly int[] MilkModifierIngredientIds = [106, 107, 108, 110, 113, 115, 118];
        private static readonly int[] CoffeeModifierIngredientIds = [6, 8];
        private static readonly int[] ModifierExcludedDrinkIds = [5, 6, 43, 12];

        public OrdersController(
            AppDbContext db,
            IPreparationStockService stockService,
            IPickupSchedulingService pickupSchedulingService)
        {
            _db = db;
            _stockService = stockService;
            _pickupSchedulingService = pickupSchedulingService;
        }

        [HttpGet("pickup-slots")]
        [AllowAnonymous]
        public ActionResult<PickupSlotsResponse> GetPickupSlots()
        {
            return Ok(_pickupSchedulingService.BuildSlotsResponse());
        }

        [HttpGet("discounts")]
        public async Task<ActionResult<List<DiscountDto>>> GetDiscounts()
        {
            var discounts = await _db.Discounts
                .AsNoTracking()
                .OrderBy(x => x.Id)
                .Select(x => new DiscountDto
                {
                    Id = x.Id,
                    Name = x.Name ?? $"Скидка #{x.Id}",
                    DiscountPercent = x.DiscountPercent ?? 0m
                })
                .ToListAsync();

            return Ok(discounts);
        }

        [HttpGet("history")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<OrderHistoryResponse>> GetHistory([FromQuery] string? period = null, [FromQuery] int? clientId = null)
        {
            var fromDate = ResolveHistoryPeriodStart(period);

            var query = _db.Orders
                .AsNoTracking()
                .Where(o => o.CreatedAt.HasValue && o.CreatedAt.Value >= fromDate);

            if (clientId.HasValue && clientId.Value > 0)
            {
                query = query.Where(o => o.ClientId == clientId.Value);
            }

            var orders = await query
                .OrderByDescending(o => o.CreatedAt)
                .ThenByDescending(o => o.Id)
                .Select(o => new OrderHistoryListItemDto
                {
                    OrderId = o.Id,
                    CreatedAt = o.CreatedAt,
                    PickupAt = o.PickupAt,
                    ClientId = o.ClientId,
                    ClientName = o.Client != null ? (o.Client.FullName ?? string.Empty) : string.Empty,
                    ClientPhone = o.Client != null ? (o.Client.PhoneNumber ?? string.Empty) : string.Empty,
                    OrderTypeId = o.OrderTypeId,
                    OrderType = o.OrderType != null ? (o.OrderType.Name ?? string.Empty) : string.Empty,
                    StatusId = o.StatusId,
                    Status = o.Status != null ? (o.Status.Name ?? string.Empty) : string.Empty,
                    DiscountId = o.DiscountId,
                    DiscountName = o.Discount != null ? (o.Discount.Name ?? string.Empty) : string.Empty,
                    DiscountPercent = o.Discount != null ? (o.Discount.DiscountPercent ?? 0m) : 0m,
                    TotalPrice = o.TotalPrice ?? 0m,
                    TotalCalories = o.TotalCalories ?? 0m,
                    Comment = o.Comment ?? string.Empty
                })
                .ToListAsync();

            var summaries = await BuildCompositionSummariesAsync(orders.Select(x => x.OrderId).ToList());
            foreach (var order in orders)
            {
                order.CompositionSummary = summaries.TryGetValue(order.OrderId, out var summary)
                    ? summary
                    : "Состав не указан";
            }

            return Ok(new OrderHistoryResponse
            {
                Orders = orders
            });
        }

        private static DateTime ResolveHistoryPeriodStart(string? period)
        {
            var now = DateTime.Now;
            var normalized = (period ?? string.Empty).Trim();

            return normalized switch
            {
                "week" => now.AddDays(-7),
                "month" => now.AddMonths(-1),
                "threeMonths" => now.AddMonths(-3),
                _ => DateTime.Today
            };
        }

        [HttpGet("history/{orderId:int}")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<OrderHistoryDetailsDto>> GetHistoryDetails(int orderId)
        {
            var details = await BuildOrderHistoryDetailsAsync(orderId, includeOptions: true);
            if (details == null)
            {
                return NotFound("Заказ не найден");
            }

            return Ok(details);
        }

        [HttpPut("history/{orderId:int}")]
        [Authorize(Roles = "Администратор")]
        public async Task<ActionResult<OrderHistoryDetailsDto>> UpdateHistoryOrder(
            int orderId,
            [FromBody] OrderHistoryUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Пустое тело запроса");
            }

            request.Dishes ??= [];
            request.Drinks ??= [];
            request.Toppings ??= [];

            var validationError = await ValidateHistoryUpdateRequestAsync(request);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var order = await _db.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            if (order == null)
            {
                return NotFound("Заказ не найден");
            }

            var newOrderRequest = new OrderRequest
            {
                OrderTypeId = request.OrderTypeId,
                DiscountId = request.DiscountId,
                Comment = request.Comment,
                PickupAt = request.PickupAt,
                Dishes = request.Dishes,
                Drinks = request.Drinks,
                Toppings = request.Toppings
            };

            if (newOrderRequest.OrderTypeId != _pickupSchedulingService.TakeawayOrderTypeId)
            {
                newOrderRequest.PickupAt = null;
            }
            else if (newOrderRequest.PickupAt.HasValue)
            {
                var pickupAt = NormalizePickupAt(newOrderRequest.PickupAt.Value);
                if (!_pickupSchedulingService.IsPickupAtAllowed(pickupAt))
                {
                    return BadRequest("Выбранное время самовывоза недоступно. Выберите слот из списка.");
                }

                newOrderRequest.PickupAt = pickupAt;
            }

            await ApplyDefaultDrinkModifiersAsync(newOrderRequest.Drinks);
            var modifierValidationError = await ValidateDrinkModifiersAsync(newOrderRequest.Drinks);
            if (modifierValidationError != null)
            {
                return BadRequest(modifierValidationError);
            }

            var savedOrderRequest = await BuildOrderRequestFromSavedOrderAsync(orderId);

            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);
            try
            {
                await RestorePortionLimitsAsync(savedOrderRequest);
                await _stockService.RestoreConsumedForOrderAsync(orderId);
                await ClearOrderCompositionAsync(orderId);

                var rebuildResult = await RebuildOrderCompositionAsync(order, newOrderRequest, request.StatusId);
                if (rebuildResult is ActionResult<OrderHistoryDetailsDto> actionResult)
                {
                    await tx.RollbackAsync();
                    return actionResult;
                }

                StockConsumptionResult consumeResult;
                try
                {
                    consumeResult = await _stockService.TryConsumeForOrderAsync(order.Id);
                }
                catch (InvalidOperationException ex)
                {
                    await tx.RollbackAsync();
                    return StatusCode(500, ex.Message);
                }

                if (!consumeResult.IsSuccess)
                {
                    await tx.RollbackAsync();
                    return Conflict(new OrderStockConflictResponse
                    {
                        ErrorCode = "STOCK_SHORTAGE",
                        Message = "Недостаточно заготовок или сырья для сохранения заказа.",
                        Items = consumeResult.Items
                            .Select(x => new StockConflictItem
                            {
                                SemiFinishedId = x.SemiFinishedId,
                                SemiFinishedName = x.SemiFinishedName,
                                Required = x.Required,
                                Available = x.Available
                            })
                            .ToList()
                    });
                }

                var portionLimitsResult = await TryConsumePortionLimitsAsync(
                    BuildPortionLimitRequirements(newOrderRequest),
                    await LoadDishNamesAsync(newOrderRequest),
                    await LoadDrinkNamesAsync(newOrderRequest),
                    await LoadToppingNamesAsync(newOrderRequest));

                if (!portionLimitsResult.IsSuccess)
                {
                    await tx.RollbackAsync();
                    return Conflict(new OrderStockConflictResponse
                    {
                        ErrorCode = "PORTION_LIMIT_SHORTAGE",
                        Message = "Недостаточно лимита порций для некоторых позиций.",
                        Items = portionLimitsResult.Items
                    });
                }

                await _stockService.RefreshMenuAvailabilityAsync();
                await _db.SaveChangesAsync();
                await tx.CommitAsync();
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

            var details = await BuildOrderHistoryDetailsAsync(orderId, includeOptions: true);
            return Ok(details);
        }

        [HttpPost]
        public async Task<ActionResult<OrderResponse>> Create([FromBody] OrderRequest request)
        {
            // ---- базовая валидация ----
            if (request == null)
                return BadRequest("Пустое тело запроса");

            request.Dishes ??= [];
            request.Drinks ??= [];
            request.Toppings ??= [];

            if (request.OrderTypeId <= 0)
                return BadRequest("OrderTypeId обязателен");

            var hasAny = request.Dishes.Count > 0 || request.Drinks.Count > 0 || request.Toppings.Count > 0;
            if (!hasAny)
                return BadRequest("Корзина пуста");

            if (request.Dishes.Any(x => x.Quantity <= 0))
                return BadRequest("Quantity у блюда должен быть > 0");

            if (request.Drinks.Any(x => x.Quantity <= 0))
                return BadRequest("Quantity у напитка должен быть > 0");

            if (request.Toppings.Any(x => x.Quantity <= 0))
                return BadRequest("Quantity у добавки должен быть > 0");

            var isClientOrder = User.IsInRole("Client");
            var isWpfGuestOrder = !isClientOrder;
            Client client;

            if (isClientOrder)
            {
                // ---- ClientId из cookie/claims ----
                var clientIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(clientIdStr, out var clientId))
                    return Unauthorized("Некорректный ClientId в cookie");

                // ---- проверяем клиента ----
                var loadedClient = await _db.Clients.FirstOrDefaultAsync(x => x.Id == clientId);
                if (loadedClient == null)
                    return Unauthorized("Клиент не найден");

                client = loadedClient;
            }
            else
            {
                client = await ResolveWpfGuestClientAsync();
            }

            var discountResolution = await ResolveDiscountForOrderAsync(client, request.DiscountId);
            if (!discountResolution.Success)
                return BadRequest(discountResolution.Message);

            // ---- проверяем тип заказа ----
            var orderTypeExists = await _db.OrderTypes.AnyAsync(x => x.Id == request.OrderTypeId);
            if (!orderTypeExists)
                return BadRequest("Некорректный тип заказа");

            if (request.OrderTypeId != _pickupSchedulingService.TakeawayOrderTypeId)
            {
                request.PickupAt = null;
            }
            else if (request.PickupAt.HasValue)
            {
                var pickupAt = NormalizePickupAt(request.PickupAt.Value);
                if (!_pickupSchedulingService.IsPickupAtAllowed(pickupAt))
                {
                    return BadRequest("Выбранное время самовывоза недоступно. Выберите слот из списка.");
                }

                request.PickupAt = pickupAt;
            }

            // ---- соберём списки ID, чтобы разом подгрузить справочники ----
            var dishIds = request.Dishes.Select(x => x.DishId).Distinct().ToList();
            var drinkIds = request.Drinks.Select(x => x.DrinkId).Distinct().ToList();

            var toppingIdsFromDishes = request.Dishes
                .SelectMany(x => x.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                .Select(x => x.ToppingId);

            var toppingIdsFromDrinks = request.Drinks
                .SelectMany(x => x.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                .Select(x => x.ToppingId);

            var toppingIdsStandalone = request.Toppings.Select(x => x.ToppingId);

            var toppingIds = toppingIdsFromDishes
                .Concat(toppingIdsFromDrinks)
                .Concat(toppingIdsStandalone)
                .Distinct()
                .ToList();
            var portionLimitRequirements = BuildPortionLimitRequirements(request);

            await ApplyDefaultDrinkModifiersAsync(request.Drinks);

            var drinkModifierValidationError = await ValidateDrinkModifiersAsync(request.Drinks);
            if (drinkModifierValidationError != null)
                return BadRequest(drinkModifierValidationError);

            // ---- загрузим цены/ккал ----
            var dishes = await _db.Dishes
                .Where(x => dishIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.PriceRub,
                    x.CaloriesKcal,
                    CategoryName = x.Category != null ? x.Category.Name : null
                })
                .ToDictionaryAsync(x => x.Id, x => x);

            var drinks = await _db.Drinks
                .Where(x => drinkIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.PriceRub,
                    x.CaloriesKcal,
                    CategoryName = x.Category != null ? x.Category.Name : null
                })
                .ToDictionaryAsync(x => x.Id, x => x);

            var toppings = await _db.ToppingsAndSyrups
                .Where(x => toppingIds.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.PriceRub,
                    x.CaloriesKcal,
                    CategoryName = x.Category != null ? x.Category.Name : null
                })
                .ToDictionaryAsync(x => x.Id, x => x);

            // ---- проверим, что все ID реально существуют ----
            if (dishIds.Any() && dishes.Count != dishIds.Count)
                return BadRequest("В заказе есть несуществующие блюда");

            if (drinkIds.Any() && drinks.Count != drinkIds.Count)
                return BadRequest("В заказе есть несуществующие напитки");

            if (toppingIds.Any() && toppings.Count != toppingIds.Count)
                return BadRequest("В заказе есть несуществующие добавки");

            var hasInactiveCategoryItem =
                dishIds.Any(id => dishes.TryGetValue(id, out var dish) && IsInactiveCategory(dish.CategoryName))
                || drinkIds.Any(id => drinks.TryGetValue(id, out var drink) && IsInactiveCategory(drink.CategoryName))
                || toppingIds.Any(id => toppings.TryGetValue(id, out var topping) && IsInactiveCategory(topping.CategoryName));

            if (hasInactiveCategoryItem)
            {
                return Conflict(new OrderStockConflictResponse
                {
                    ErrorCode = "INACTIVE_CATEGORY",
                    Message = "В заказе есть позиции из категории \"Неактивные\". Обновите меню.",
                    Items = []
                });
            }

            var dishNamesById = dishes.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.Name) ? $"Блюдо #{x.Key}" : x.Value.Name!);
            var drinkNamesById = drinks.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.Name) ? $"Напиток #{x.Key}" : x.Value.Name!);
            var toppingNamesById = toppings.ToDictionary(
                x => x.Key,
                x => string.IsNullOrWhiteSpace(x.Value.Name) ? $"Добавка #{x.Key}" : x.Value.Name!);

            // ---- транзакция ----
            await using var tx = await _db.Database.BeginTransactionAsync(IsolationLevel.Serializable);

            try
            {
                // 1) категория/скидка ДО расчёта итоговой цены
                // (считаем скидку по текущей категории; после заказа мы обновим OrderCount/категорию)
                var discountId = discountResolution.DiscountId;
                var discountPercent = discountResolution.DiscountPercent;
                var createdStatus = await ResolveCreatedStatusAsync();
                if (createdStatus == null)
                    return StatusCode(500, "Не найден статус заказа для создания");

                // 2) создаём Order
                var order = new Order
                {
                    CreatedAt = DateTime.Now,
                    ClientId = client.Id,
                    StatusId = createdStatus.Value.Id,
                    OrderTypeId = request.OrderTypeId,
                    Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment,
                    PickupAt = request.PickupAt,
                    DiscountId = discountId,
                    TotalCalories = 0m,
                    TotalPrice = 0m
                };

                _db.Orders.Add(order);
                await _db.SaveChangesAsync(); // чтобы получить order.Id

                decimal totalPriceBeforeDiscount = 0m;
                decimal totalCalories = 0m;

                // 3) Блюда + их добавки
                foreach (var dishReq in request.Dishes)
                {
                    var dish = dishes[dishReq.DishId];

                    var dishBasePrice = (dish.PriceRub ?? 0m) * dishReq.Quantity;
                    var dishBaseCalories = (dish.CaloriesKcal ?? 0m) * dishReq.Quantity;

                    // создаём OrderDishItem
                    var odi = new OrderDishItem
                    {
                        OrderId = order.Id,
                        DishId = dishReq.DishId,
                        Quantity = dishReq.Quantity,
                        FinalPrice = 0m
                    };

                    _db.OrderDishItems.Add(odi);
                    await _db.SaveChangesAsync(); // нужен odi.Id для DishToppings

                    decimal toppingsTotalPrice = 0m;
                    decimal toppingsTotalCalories = 0m;

                    foreach (var topReq in dishReq.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                    {
                        if (topReq.Quantity <= 0)
                            continue;

                        var top = toppings[topReq.ToppingId];

                        // В UI topping.quantity — "на 1 блюдо", а итог умножается на item.quantity
                        var totalToppingQty = topReq.Quantity * dishReq.Quantity;

                        var topPrice = (top.PriceRub ?? 0m) * totalToppingQty;
                        var topCalories = (top.CaloriesKcal ?? 0m) * totalToppingQty;

                        toppingsTotalPrice += topPrice;
                        toppingsTotalCalories += topCalories;

                        var dt = new DishTopping
                        {
                            ToppingId = topReq.ToppingId,
                            OrderDishItemId = odi.Id,
                            Quantity = totalToppingQty,
                            FinalPrice = topPrice
                        };

                        _db.DishToppings.Add(dt);
                    }

                    odi.FinalPrice = dishBasePrice + toppingsTotalPrice;

                    totalPriceBeforeDiscount += odi.FinalPrice ?? 0m;
                    totalCalories += dishBaseCalories + toppingsTotalCalories;

                    _db.OrderDishItems.Update(odi);
                    await _db.SaveChangesAsync();
                }

                // 4) Напитки + их добавки
                foreach (var drinkReq in request.Drinks)
                {
                    var drink = drinks[drinkReq.DrinkId];

                    var drinkBasePrice = (drink.PriceRub ?? 0m) * drinkReq.Quantity;
                    var drinkBaseCalories = (drink.CaloriesKcal ?? 0m) * drinkReq.Quantity;

                    var odi = new OrderDrinkItem
                    {
                        OrderId = order.Id,
                        DrinkId = drinkReq.DrinkId,
                        Quantity = drinkReq.Quantity,
                        FinalPrice = 0m
                    };

                    _db.OrderDrinkItems.Add(odi);
                    await _db.SaveChangesAsync(); // нужен odi.Id для DrinkToppings

                    _db.OrderDrinkItemModifiers.Add(new OrderDrinkItemModifier
                    {
                        OrderDrinkItemId = odi.Id,
                        MilkIngredientId = drinkReq.MilkIngredientId,
                        CoffeeIngredientId = drinkReq.CoffeeIngredientId
                    });

                    decimal toppingsTotalPrice = 0m;
                    decimal toppingsTotalCalories = 0m;

                    foreach (var topReq in drinkReq.Toppings ?? Enumerable.Empty<OrderItemToppingRequest>())
                    {
                        if (topReq.Quantity <= 0)
                            continue;

                        var top = toppings[topReq.ToppingId];

                        var totalToppingQty = topReq.Quantity * drinkReq.Quantity;

                        var topPrice = (top.PriceRub ?? 0m) * totalToppingQty;
                        var topCalories = (top.CaloriesKcal ?? 0m) * totalToppingQty;

                        toppingsTotalPrice += topPrice;
                        toppingsTotalCalories += topCalories;

                        var dt = new DrinkTopping
                        {
                            ToppingId = topReq.ToppingId,
                            OrderDrinkItemId = odi.Id,
                            Quantity = totalToppingQty,
                            FinalPrice = topPrice
                        };

                        _db.DrinkToppings.Add(dt);
                    }

                    odi.FinalPrice = drinkBasePrice + toppingsTotalPrice;

                    totalPriceBeforeDiscount += odi.FinalPrice ?? 0m;
                    totalCalories += drinkBaseCalories + toppingsTotalCalories;

                    _db.OrderDrinkItems.Update(odi);
                    await _db.SaveChangesAsync();
                }

                // 5) Отдельные добавки (OrderToppingItems)
                foreach (var topReq in request.Toppings)
                {
                    var top = toppings[topReq.ToppingId];

                    var linePrice = (top.PriceRub ?? 0m) * topReq.Quantity;
                    var lineCalories = (top.CaloriesKcal ?? 0m) * topReq.Quantity;

                    var oti = new OrderToppingItem
                    {
                        OrderId = order.Id,
                        ToppingId = topReq.ToppingId,
                        Quantity = topReq.Quantity,
                        TotalPrice = linePrice
                    };

                    _db.OrderToppingItems.Add(oti);

                    totalPriceBeforeDiscount += linePrice;
                    totalCalories += lineCalories;
                }

                // 6) применяем скидку
                var totalAfterDiscount = ApplyDiscount(totalPriceBeforeDiscount, discountPercent);

                order.TotalPrice = Round2(totalAfterDiscount);
                order.TotalCalories = Round2(totalCalories);

                _db.Orders.Update(order);
                await _db.SaveChangesAsync();

                StockConsumptionResult consumeResult;
                try
                {
                    consumeResult = await _stockService.TryConsumeForOrderAsync(order.Id);
                }
                catch (InvalidOperationException ex)
                {
                    await tx.RollbackAsync();
                    return BadRequest(ex.Message);
                }

                if (!consumeResult.IsSuccess)
                {
                    await tx.RollbackAsync();
                    return Conflict(new OrderStockConflictResponse
                    {
                        Message = "Недостаточно заготовок для оформления заказа. Обновите меню.",
                        Items = consumeResult.Items
                            .Select(x => new StockConflictItem
                            {
                                SemiFinishedId = x.SemiFinishedId,
                                SemiFinishedName = x.SemiFinishedName,
                                Required = x.Required,
                                Available = x.Available
                            })
                            .ToList()
                    });
                }

                var portionLimitsResult = await TryConsumePortionLimitsAsync(
                    portionLimitRequirements,
                    dishNamesById,
                    drinkNamesById,
                    toppingNamesById);
                if (!portionLimitsResult.IsSuccess)
                {
                    await tx.RollbackAsync();
                    return Conflict(new OrderStockConflictResponse
                    {
                        ErrorCode = "PORTION_LIMIT_CONFLICT",
                        Message = "Недостаточно лимита порций для некоторых позиций. Обновите меню.",
                        Items = portionLimitsResult.Items
                    });
                }

                await _db.SaveChangesAsync();

                // 7) обновляем OrderCount/категорию реального клиента (служебного гостя WPF не трогаем)
                if (!isWpfGuestOrder)
                {
                    await UpdateClientCategoryAfterOrderAsync(client);
                }

                await _stockService.RefreshMenuAvailabilityAsync();

                await tx.CommitAsync();

                return Ok(new OrderResponse
                {
                    OrderId = order.Id,
                    Status = createdStatus.Value.Name,
                    TotalPrice = order.TotalPrice ?? 0m,
                    TotalCalories = order.TotalCalories ?? 0m,
                    DiscountId = order.DiscountId,
                    DiscountPercent = discountPercent
                });
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }

        private async Task<Client> ResolveWpfGuestClientAsync()
        {
            var existing = await _db.Clients
                .FirstOrDefaultAsync(x => x.PhoneNumber == WpfGuestClientPhone);

            if (existing != null)
            {
                return existing;
            }

            var guest = new Client
            {
                FullName = WpfGuestClientFullName,
                PhoneNumber = WpfGuestClientPhone,
                Password = WpfGuestClientPassword,
                ClientCategoryId = ClientCategoryNoneId,
                OrderCount = 0
            };

            _db.Clients.Add(guest);
            await _db.SaveChangesAsync();
            return guest;
        }

        private async Task<string?> ValidateHistoryUpdateRequestAsync(OrderHistoryUpdateRequest request)
        {
            if (request.OrderTypeId <= 0)
                return "Тип заказа обязателен.";

            var hasAny = request.Dishes.Count > 0 || request.Drinks.Count > 0 || request.Toppings.Count > 0;
            if (!hasAny)
                return "Состав заказа не может быть пустым.";

            if (request.Dishes.Any(x => x.DishId <= 0 || x.Quantity <= 0m))
                return "У каждого блюда должен быть выбран товар и количество больше нуля.";

            if (request.Drinks.Any(x => x.DrinkId <= 0 || x.Quantity <= 0m))
                return "У каждого напитка должен быть выбран товар и количество больше нуля.";

            if (request.Toppings.Any(x => x.ToppingId <= 0 || x.Quantity <= 0))
                return "У каждой добавки должен быть выбран товар и количество больше нуля.";

            if (request.Dishes.SelectMany(x => x.Toppings ?? []).Any(x => x.ToppingId <= 0 || x.Quantity <= 0m) ||
                request.Drinks.SelectMany(x => x.Toppings ?? []).Any(x => x.ToppingId <= 0 || x.Quantity <= 0m))
                return "У добавок к позициям количество должно быть больше нуля.";

            var orderTypeExists = await _db.OrderTypes.AnyAsync(x => x.Id == request.OrderTypeId);
            if (!orderTypeExists)
                return "Некорректный тип заказа.";

            if (request.StatusId.HasValue)
            {
                var statusExists = await _db.OrderStatuses.AnyAsync(x => x.Id == request.StatusId.Value);
                if (!statusExists)
                    return "Некорректный статус заказа.";
            }

            if (request.DiscountId.HasValue)
            {
                var discountExists = await _db.Discounts.AnyAsync(x => x.Id == request.DiscountId.Value);
                if (!discountExists)
                    return "Некорректная скидка.";
            }

            return null;
        }

        private async Task<ActionResult<OrderHistoryDetailsDto>?> RebuildOrderCompositionAsync(
            Order order,
            OrderRequest request,
            int? statusId)
        {
            var dishIds = request.Dishes.Select(x => x.DishId).Distinct().ToList();
            var drinkIds = request.Drinks.Select(x => x.DrinkId).Distinct().ToList();
            var toppingIds = request.Dishes
                .SelectMany(x => x.Toppings ?? [])
                .Select(x => x.ToppingId)
                .Concat(request.Drinks.SelectMany(x => x.Toppings ?? []).Select(x => x.ToppingId))
                .Concat(request.Toppings.Select(x => x.ToppingId))
                .Distinct()
                .ToList();

            var dishes = await _db.Dishes
                .Where(x => dishIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.PriceRub, x.CaloriesKcal, CategoryName = x.Category != null ? x.Category.Name : null })
                .ToDictionaryAsync(x => x.Id, x => x);

            var drinks = await _db.Drinks
                .Where(x => drinkIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.PriceRub, x.CaloriesKcal, CategoryName = x.Category != null ? x.Category.Name : null })
                .ToDictionaryAsync(x => x.Id, x => x);

            var toppings = await _db.ToppingsAndSyrups
                .Where(x => toppingIds.Contains(x.Id))
                .Select(x => new { x.Id, x.Name, x.PriceRub, x.CaloriesKcal, CategoryName = x.Category != null ? x.Category.Name : null })
                .ToDictionaryAsync(x => x.Id, x => x);

            if (dishIds.Count != dishes.Count)
                return BadRequest("В заказе есть несуществующие блюда.");

            if (drinkIds.Count != drinks.Count)
                return BadRequest("В заказе есть несуществующие напитки.");

            if (toppingIds.Count != toppings.Count)
                return BadRequest("В заказе есть несуществующие добавки.");

            var hasInactiveCategoryItem =
                dishes.Values.Any(x => IsInactiveCategory(x.CategoryName)) ||
                drinks.Values.Any(x => IsInactiveCategory(x.CategoryName)) ||
                toppings.Values.Any(x => IsInactiveCategory(x.CategoryName));
            if (hasInactiveCategoryItem)
            {
                return Conflict(new OrderStockConflictResponse
                {
                    ErrorCode = "INACTIVE_CATEGORY",
                    Message = "В заказе есть позиции из категории \"Неактивные\". Обновите меню.",
                    Items = []
                });
            }

            var discount = request.DiscountId.HasValue
                ? await _db.Discounts.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.DiscountId.Value)
                : null;
            var discountPercent = discount?.DiscountPercent ?? 0m;
            decimal totalPriceBeforeDiscount = 0m;
            decimal totalCalories = 0m;

            order.OrderTypeId = request.OrderTypeId;
            order.StatusId = statusId ?? order.StatusId;
            order.Comment = string.IsNullOrWhiteSpace(request.Comment) ? null : request.Comment.Trim();
            order.PickupAt = request.PickupAt;
            order.DiscountId = request.DiscountId;

            foreach (var dishReq in request.Dishes)
            {
                var dish = dishes[dishReq.DishId];
                var item = new OrderDishItem
                {
                    OrderId = order.Id,
                    DishId = dishReq.DishId,
                    Quantity = dishReq.Quantity,
                    FinalPrice = 0m,
                    IsCompleted = false
                };

                _db.OrderDishItems.Add(item);
                await _db.SaveChangesAsync();

                var basePrice = (dish.PriceRub ?? 0m) * dishReq.Quantity;
                var baseCalories = (dish.CaloriesKcal ?? 0m) * dishReq.Quantity;
                decimal toppingsPrice = 0m;
                decimal toppingsCalories = 0m;

                foreach (var topReq in dishReq.Toppings ?? [])
                {
                    var topping = toppings[topReq.ToppingId];
                    var totalQty = topReq.Quantity * dishReq.Quantity;
                    var linePrice = (topping.PriceRub ?? 0m) * totalQty;
                    var lineCalories = (topping.CaloriesKcal ?? 0m) * totalQty;
                    toppingsPrice += linePrice;
                    toppingsCalories += lineCalories;

                    _db.DishToppings.Add(new DishTopping
                    {
                        ToppingId = topReq.ToppingId,
                        OrderDishItemId = item.Id,
                        Quantity = totalQty,
                        FinalPrice = linePrice
                    });
                }

                item.FinalPrice = basePrice + toppingsPrice;
                totalPriceBeforeDiscount += item.FinalPrice ?? 0m;
                totalCalories += baseCalories + toppingsCalories;
            }

            foreach (var drinkReq in request.Drinks)
            {
                var drink = drinks[drinkReq.DrinkId];
                var item = new OrderDrinkItem
                {
                    OrderId = order.Id,
                    DrinkId = drinkReq.DrinkId,
                    Quantity = drinkReq.Quantity,
                    FinalPrice = 0m,
                    IsCompleted = false
                };

                _db.OrderDrinkItems.Add(item);
                await _db.SaveChangesAsync();

                _db.OrderDrinkItemModifiers.Add(new OrderDrinkItemModifier
                {
                    OrderDrinkItemId = item.Id,
                    MilkIngredientId = drinkReq.MilkIngredientId,
                    CoffeeIngredientId = drinkReq.CoffeeIngredientId
                });

                var basePrice = (drink.PriceRub ?? 0m) * drinkReq.Quantity;
                var baseCalories = (drink.CaloriesKcal ?? 0m) * drinkReq.Quantity;
                decimal toppingsPrice = 0m;
                decimal toppingsCalories = 0m;

                foreach (var topReq in drinkReq.Toppings ?? [])
                {
                    var topping = toppings[topReq.ToppingId];
                    var totalQty = topReq.Quantity * drinkReq.Quantity;
                    var linePrice = (topping.PriceRub ?? 0m) * totalQty;
                    var lineCalories = (topping.CaloriesKcal ?? 0m) * totalQty;
                    toppingsPrice += linePrice;
                    toppingsCalories += lineCalories;

                    _db.DrinkToppings.Add(new DrinkTopping
                    {
                        ToppingId = topReq.ToppingId,
                        OrderDrinkItemId = item.Id,
                        Quantity = totalQty,
                        FinalPrice = linePrice
                    });
                }

                item.FinalPrice = basePrice + toppingsPrice;
                totalPriceBeforeDiscount += item.FinalPrice ?? 0m;
                totalCalories += baseCalories + toppingsCalories;
            }

            foreach (var topReq in request.Toppings)
            {
                var topping = toppings[topReq.ToppingId];
                var linePrice = (topping.PriceRub ?? 0m) * topReq.Quantity;
                var lineCalories = (topping.CaloriesKcal ?? 0m) * topReq.Quantity;

                _db.OrderToppingItems.Add(new OrderToppingItem
                {
                    OrderId = order.Id,
                    ToppingId = topReq.ToppingId,
                    Quantity = topReq.Quantity,
                    TotalPrice = linePrice,
                    IsCompleted = false
                });

                totalPriceBeforeDiscount += linePrice;
                totalCalories += lineCalories;
            }

            order.TotalPrice = Round2(ApplyDiscount(totalPriceBeforeDiscount, discountPercent));
            order.TotalCalories = Round2(totalCalories);
            _db.Orders.Update(order);
            await _db.SaveChangesAsync();

            return null;
        }

        private async Task ClearOrderCompositionAsync(int orderId)
        {
            var dishItems = await _db.OrderDishItems
                .Where(x => x.OrderId == orderId)
                .Include(x => x.DishToppings)
                .ToListAsync();
            var drinkItems = await _db.OrderDrinkItems
                .Where(x => x.OrderId == orderId)
                .Include(x => x.DrinkToppings)
                .Include(x => x.OrderDrinkItemModifier)
                .ToListAsync();
            var toppingItems = await _db.OrderToppingItems
                .Where(x => x.OrderId == orderId)
                .ToListAsync();

            _db.DishToppings.RemoveRange(dishItems.SelectMany(x => x.DishToppings));
            _db.DrinkToppings.RemoveRange(drinkItems.SelectMany(x => x.DrinkToppings));
            _db.OrderDrinkItemModifiers.RemoveRange(drinkItems
                .Select(x => x.OrderDrinkItemModifier)
                .Where(x => x != null)!);
            _db.OrderDishItems.RemoveRange(dishItems);
            _db.OrderDrinkItems.RemoveRange(drinkItems);
            _db.OrderToppingItems.RemoveRange(toppingItems);
            await _db.SaveChangesAsync();
        }

        private async Task<OrderRequest> BuildOrderRequestFromSavedOrderAsync(int orderId)
        {
            var request = new OrderRequest
            {
                Dishes = new List<OrderDishItemRequest>(),
                Drinks = new List<OrderDrinkItemRequest>(),
                Toppings = new List<OrderToppingItemRequest>()
            };

            var dishItems = await _db.OrderDishItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId && x.DishId.HasValue && x.Quantity.HasValue)
                .Select(x => new { x.Id, DishId = x.DishId!.Value, Quantity = x.Quantity!.Value })
                .ToListAsync();
            var dishItemIds = dishItems.Select(x => x.Id).ToList();
            var dishToppings = dishItemIds.Count == 0
                ? []
                : await _db.DishToppings
                    .AsNoTracking()
                    .Where(x => x.OrderDishItemId.HasValue && dishItemIds.Contains(x.OrderDishItemId.Value) && x.ToppingId.HasValue && x.Quantity.HasValue)
                    .Select(x => new { ParentId = x.OrderDishItemId!.Value, ToppingId = x.ToppingId!.Value, Quantity = x.Quantity!.Value })
                    .ToListAsync();

            foreach (var item in dishItems)
            {
                request.Dishes.Add(new OrderDishItemRequest
                {
                    DishId = item.DishId,
                    Quantity = item.Quantity,
                    Toppings = dishToppings
                        .Where(x => x.ParentId == item.Id)
                        .Select(x => new OrderItemToppingRequest
                        {
                            ToppingId = x.ToppingId,
                            Quantity = item.Quantity > DecimalEpsilon ? x.Quantity / item.Quantity : x.Quantity
                        })
                        .ToList()
                });
            }

            var drinkItems = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId && x.DrinkId.HasValue && x.Quantity.HasValue)
                .Select(x => new { x.Id, DrinkId = x.DrinkId!.Value, Quantity = x.Quantity!.Value })
                .ToListAsync();
            var drinkItemIds = drinkItems.Select(x => x.Id).ToList();
            var drinkToppings = drinkItemIds.Count == 0
                ? []
                : await _db.DrinkToppings
                    .AsNoTracking()
                    .Where(x => x.OrderDrinkItemId.HasValue && drinkItemIds.Contains(x.OrderDrinkItemId.Value) && x.ToppingId.HasValue && x.Quantity.HasValue)
                    .Select(x => new { ParentId = x.OrderDrinkItemId!.Value, ToppingId = x.ToppingId!.Value, Quantity = x.Quantity!.Value })
                    .ToListAsync();
            var modifiers = drinkItemIds.Count == 0
                ? new Dictionary<int, OrderDrinkItemModifier>()
                : await _db.OrderDrinkItemModifiers
                    .AsNoTracking()
                    .Where(x => drinkItemIds.Contains(x.OrderDrinkItemId))
                    .ToDictionaryAsync(x => x.OrderDrinkItemId, x => x);

            foreach (var item in drinkItems)
            {
                modifiers.TryGetValue(item.Id, out var modifier);
                request.Drinks.Add(new OrderDrinkItemRequest
                {
                    DrinkId = item.DrinkId,
                    Quantity = item.Quantity,
                    MilkIngredientId = modifier?.MilkIngredientId,
                    CoffeeIngredientId = modifier?.CoffeeIngredientId,
                    Toppings = drinkToppings
                        .Where(x => x.ParentId == item.Id)
                        .Select(x => new OrderItemToppingRequest
                        {
                            ToppingId = x.ToppingId,
                            Quantity = item.Quantity > DecimalEpsilon ? x.Quantity / item.Quantity : x.Quantity
                        })
                        .ToList()
                });
            }

            request.Toppings = await _db.OrderToppingItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .Select(x => new OrderToppingItemRequest
                {
                    ToppingId = x.ToppingId,
                    Quantity = x.Quantity
                })
                .ToListAsync();

            return request;
        }

        private async Task RestorePortionLimitsAsync(OrderRequest savedOrder)
        {
            var requirements = BuildPortionLimitRequirements(savedOrder);
            if (requirements.Count == 0)
            {
                return;
            }

            var keys = requirements.Keys.ToList();
            var itemTypes = keys.Select(x => x.ItemType).Distinct().ToList();
            var itemIds = keys.Select(x => x.ItemId).Distinct().ToList();
            var rows = await _db.MenuItemPortionLimits
                .Where(x => itemTypes.Contains(x.ItemType) && itemIds.Contains(x.ItemId))
                .ToListAsync();
            var rowsByKey = rows.ToDictionary(x => new MenuItemLimitKey(x.ItemType.Trim().ToLowerInvariant(), x.ItemId), x => x);
            var now = DateTime.Now;

            foreach (var pair in requirements)
            {
                if (rowsByKey.TryGetValue(pair.Key, out var row))
                {
                    row.RemainingPortions = Round2(Math.Max(0m, row.RemainingPortions) + pair.Value);
                    row.UpdatedAt = now;
                }
            }
        }

        private async Task<Dictionary<int, string>> LoadDishNamesAsync(OrderRequest request)
        {
            var ids = request.Dishes.Select(x => x.DishId).Distinct().ToList();
            return ids.Count == 0
                ? new Dictionary<int, string>()
                : await _db.Dishes
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name ?? $"Блюдо #{x.Id}");
        }

        private async Task<Dictionary<int, string>> LoadDrinkNamesAsync(OrderRequest request)
        {
            var ids = request.Drinks.Select(x => x.DrinkId).Distinct().ToList();
            return ids.Count == 0
                ? new Dictionary<int, string>()
                : await _db.Drinks
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name ?? $"Напиток #{x.Id}");
        }

        private async Task<Dictionary<int, string>> LoadToppingNamesAsync(OrderRequest request)
        {
            var ids = request.Dishes.SelectMany(x => x.Toppings ?? []).Select(x => x.ToppingId)
                .Concat(request.Drinks.SelectMany(x => x.Toppings ?? []).Select(x => x.ToppingId))
                .Concat(request.Toppings.Select(x => x.ToppingId))
                .Distinct()
                .ToList();
            return ids.Count == 0
                ? new Dictionary<int, string>()
                : await _db.ToppingsAndSyrups
                    .AsNoTracking()
                    .Where(x => ids.Contains(x.Id))
                    .ToDictionaryAsync(x => x.Id, x => x.Name ?? $"Добавка #{x.Id}");
        }

        private async Task<Dictionary<int, string>> BuildCompositionSummariesAsync(IReadOnlyCollection<int> orderIds)
        {
            var result = orderIds.Distinct().ToDictionary(x => x, _ => new List<string>());
            if (result.Count == 0)
            {
                return new Dictionary<int, string>();
            }

            var ids = result.Keys.ToList();
            var dishes = await _db.OrderDishItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && ids.Contains(x.OrderId.Value))
                .Select(x => new { OrderId = x.OrderId!.Value, Name = x.Dish != null ? x.Dish.Name : null, Quantity = x.Quantity ?? 0m })
                .ToListAsync();
            foreach (var item in dishes)
            {
                result[item.OrderId].Add($"{item.Name ?? "Блюдо"} x{FormatQuantity(item.Quantity)}");
            }

            var drinks = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(x => x.OrderId.HasValue && ids.Contains(x.OrderId.Value))
                .Select(x => new { OrderId = x.OrderId!.Value, Name = x.Drink != null ? x.Drink.Name : null, Quantity = x.Quantity ?? 0m })
                .ToListAsync();
            foreach (var item in drinks)
            {
                result[item.OrderId].Add($"{item.Name ?? "Напиток"} x{FormatQuantity(item.Quantity)}");
            }

            var toppings = await _db.OrderToppingItems
                .AsNoTracking()
                .Where(x => ids.Contains(x.OrderId))
                .Select(x => new { x.OrderId, Name = x.Topping != null ? x.Topping.Name : null, Quantity = (decimal)x.Quantity })
                .ToListAsync();
            foreach (var item in toppings)
            {
                result[item.OrderId].Add($"{item.Name ?? "Добавка"} x{FormatQuantity(item.Quantity)}");
            }

            return result.ToDictionary(
                x => x.Key,
                x => x.Value.Count == 0 ? "Состав не указан" : string.Join(", ", x.Value.Take(4)) + (x.Value.Count > 4 ? "..." : string.Empty));
        }

        private async Task<OrderHistoryDetailsDto?> BuildOrderHistoryDetailsAsync(int orderId, bool includeOptions)
        {
            var order = await _db.Orders
                .AsNoTracking()
                .Where(o => o.Id == orderId)
                .Select(o => new OrderHistoryDetailsDto
                {
                    OrderId = o.Id,
                    CreatedAt = o.CreatedAt,
                    PickupAt = o.PickupAt,
                    ClientId = o.ClientId,
                    ClientName = o.Client != null ? (o.Client.FullName ?? string.Empty) : string.Empty,
                    ClientPhone = o.Client != null ? (o.Client.PhoneNumber ?? string.Empty) : string.Empty,
                    OrderTypeId = o.OrderTypeId,
                    OrderType = o.OrderType != null ? (o.OrderType.Name ?? string.Empty) : string.Empty,
                    StatusId = o.StatusId,
                    Status = o.Status != null ? (o.Status.Name ?? string.Empty) : string.Empty,
                    DiscountId = o.DiscountId,
                    DiscountName = o.Discount != null ? (o.Discount.Name ?? string.Empty) : string.Empty,
                    DiscountPercent = o.Discount != null ? (o.Discount.DiscountPercent ?? 0m) : 0m,
                    TotalPrice = o.TotalPrice ?? 0m,
                    TotalCalories = o.TotalCalories ?? 0m,
                    Comment = o.Comment ?? string.Empty
                })
                .FirstOrDefaultAsync();

            if (order == null)
            {
                return null;
            }

            order.Dishes = await LoadHistoryDishesAsync(orderId);
            order.Drinks = await LoadHistoryDrinksAsync(orderId);
            order.Toppings = await LoadHistoryToppingsAsync(orderId);
            order.CompositionSummary = string.Join(", ", order.Dishes.Select(x => $"{x.Name} x{FormatQuantity(x.Quantity)}")
                .Concat(order.Drinks.Select(x => $"{x.Name} x{FormatQuantity(x.Quantity)}"))
                .Concat(order.Toppings.Select(x => $"{x.Name} x{FormatQuantity(x.Quantity)}")));

            if (includeOptions)
            {
                order.OrderTypes = await _db.OrderTypes
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new OrderHistoryOptionDto { Id = x.Id, Name = x.Name ?? $"Тип #{x.Id}" })
                    .ToListAsync();
                order.Statuses = await _db.OrderStatuses
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new OrderHistoryOptionDto { Id = x.Id, Name = x.Name ?? $"Статус #{x.Id}" })
                    .ToListAsync();
                order.Discounts = await _db.Discounts
                    .AsNoTracking()
                    .OrderBy(x => x.Id)
                    .Select(x => new OrderHistoryDiscountOptionDto
                    {
                        Id = x.Id,
                        Name = x.Name ?? $"Скидка #{x.Id}",
                        DiscountPercent = x.DiscountPercent ?? 0m
                    })
                    .ToListAsync();
            }

            return order;
        }

        private async Task<List<OrderHistoryDishItemDto>> LoadHistoryDishesAsync(int orderId)
        {
            var dishes = await _db.OrderDishItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId && x.DishId.HasValue)
                .OrderBy(x => x.Id)
                .Select(x => new OrderHistoryDishItemDto
                {
                    ItemId = x.Id,
                    DishId = x.DishId!.Value,
                    Name = x.Dish != null ? (x.Dish.Name ?? string.Empty) : string.Empty,
                    Quantity = x.Quantity ?? 0m,
                    FinalPrice = x.FinalPrice ?? 0m
                })
                .ToListAsync();
            var itemIds = dishes.Select(x => x.ItemId).ToList();
            var toppings = itemIds.Count == 0
                ? []
                : await _db.DishToppings
                    .AsNoTracking()
                    .Where(x => x.OrderDishItemId.HasValue && itemIds.Contains(x.OrderDishItemId.Value) && x.ToppingId.HasValue)
                    .OrderBy(x => x.Id)
                    .Select(x => new
                    {
                        ParentId = x.OrderDishItemId!.Value,
                        Topping = new OrderHistoryLinkedToppingDto
                        {
                            ToppingId = x.ToppingId!.Value,
                            Name = x.Topping != null ? (x.Topping.Name ?? string.Empty) : string.Empty,
                            Quantity = x.Quantity ?? 0m,
                            FinalPrice = x.FinalPrice ?? 0m
                        }
                    })
                    .ToListAsync();

            foreach (var dish in dishes)
            {
                dish.Toppings = toppings.Where(x => x.ParentId == dish.ItemId).Select(x => x.Topping).ToList();
            }

            return dishes;
        }

        private async Task<List<OrderHistoryDrinkItemDto>> LoadHistoryDrinksAsync(int orderId)
        {
            var drinks = await _db.OrderDrinkItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId && x.DrinkId.HasValue)
                .OrderBy(x => x.Id)
                .Select(x => new OrderHistoryDrinkItemDto
                {
                    ItemId = x.Id,
                    DrinkId = x.DrinkId!.Value,
                    Name = x.Drink != null ? (x.Drink.Name ?? string.Empty) : string.Empty,
                    Quantity = x.Quantity ?? 0m,
                    FinalPrice = x.FinalPrice ?? 0m,
                    MilkIngredientId = x.OrderDrinkItemModifier != null ? x.OrderDrinkItemModifier.MilkIngredientId : null,
                    MilkIngredientName = x.OrderDrinkItemModifier != null && x.OrderDrinkItemModifier.MilkIngredient != null ? (x.OrderDrinkItemModifier.MilkIngredient.Name ?? string.Empty) : string.Empty,
                    CoffeeIngredientId = x.OrderDrinkItemModifier != null ? x.OrderDrinkItemModifier.CoffeeIngredientId : null,
                    CoffeeIngredientName = x.OrderDrinkItemModifier != null && x.OrderDrinkItemModifier.CoffeeIngredient != null ? (x.OrderDrinkItemModifier.CoffeeIngredient.Name ?? string.Empty) : string.Empty
                })
                .ToListAsync();
            var itemIds = drinks.Select(x => x.ItemId).ToList();
            var toppings = itemIds.Count == 0
                ? []
                : await _db.DrinkToppings
                    .AsNoTracking()
                    .Where(x => x.OrderDrinkItemId.HasValue && itemIds.Contains(x.OrderDrinkItemId.Value) && x.ToppingId.HasValue)
                    .OrderBy(x => x.Id)
                    .Select(x => new
                    {
                        ParentId = x.OrderDrinkItemId!.Value,
                        Topping = new OrderHistoryLinkedToppingDto
                        {
                            ToppingId = x.ToppingId!.Value,
                            Name = x.Topping != null ? (x.Topping.Name ?? string.Empty) : string.Empty,
                            Quantity = x.Quantity ?? 0m,
                            FinalPrice = x.FinalPrice ?? 0m
                        }
                    })
                    .ToListAsync();

            foreach (var drink in drinks)
            {
                drink.Toppings = toppings.Where(x => x.ParentId == drink.ItemId).Select(x => x.Topping).ToList();
            }

            return drinks;
        }

        private async Task<List<OrderHistoryToppingItemDto>> LoadHistoryToppingsAsync(int orderId)
        {
            return await _db.OrderToppingItems
                .AsNoTracking()
                .Where(x => x.OrderId == orderId)
                .OrderBy(x => x.Id)
                .Select(x => new OrderHistoryToppingItemDto
                {
                    ItemId = x.Id,
                    ToppingId = x.ToppingId,
                    Name = x.Topping != null ? (x.Topping.Name ?? string.Empty) : string.Empty,
                    Quantity = x.Quantity,
                    TotalPrice = x.TotalPrice
                })
                .ToListAsync();
        }

        private static string FormatQuantity(decimal quantity)
        {
            return quantity == decimal.Truncate(quantity)
                ? quantity.ToString("0", CultureInfo.CurrentCulture)
                : quantity.ToString("0.##", CultureInfo.CurrentCulture);
        }

        private async Task<DiscountResolution> ResolveDiscountForOrderAsync(Client client, int? requestedDiscountId)
        {
            if (requestedDiscountId.HasValue)
            {
                var requested = await _db.Discounts
                    .AsNoTracking()
                    .Where(x => x.Id == requestedDiscountId.Value)
                    .Select(x => new
                    {
                        x.Id,
                        Percent = x.DiscountPercent ?? 0m
                    })
                    .FirstOrDefaultAsync();

                if (requested == null)
                {
                    return DiscountResolution.Fail("Выбранная скидка не найдена.");
                }

                return DiscountResolution.Ok(requested.Id, requested.Percent);
            }

            var (discountId, discountPercent) = await ResolveDiscountByClientAsync(client);
            return DiscountResolution.Ok(discountId, discountPercent);
        }

        private async Task<(int? DiscountId, decimal DiscountPercent)> ResolveDiscountByClientAsync(Client client)
        {
            // если категория null — считаем "Без категории" (без скидки)
            var catId = client.ClientCategoryId ?? ClientCategoryNoneId;

            int? discountId = null;

            if (catId == ClientCategoryNewId) discountId = DiscountNewId;
            else if (catId == ClientCategoryRegularId) discountId = DiscountRegularId;
            else if (catId == ClientCategorySpecialId) discountId = DiscountSpecialId;

            if (discountId == null)
                return (null, 0m);

            var percent = await _db.Discounts
                .Where(x => x.Id == discountId.Value)
                .Select(x => x.DiscountPercent ?? 0m)
                .FirstOrDefaultAsync();

            return (discountId, percent);
        }

        private async Task UpdateClientCategoryAfterOrderAsync(Client client)
        {
            var currentCount = client.OrderCount ?? 0;
            currentCount += 1;
            client.OrderCount = currentCount;

            //  Если админ уже поставил "Особый" — НЕ трогаем категорию автоматически
            if (client.ClientCategoryId == ClientCategorySpecialId)
            {
                _db.Clients.Update(client);
                await _db.SaveChangesAsync();
                return;
            }

            //  Автоматом только "Новый" -> "Постоянный"
            int newCategoryId;

            if (currentCount >= RegularFromOrdersCount)
                newCategoryId = ClientCategoryRegularId;
            else
                newCategoryId = ClientCategoryNewId;

            client.ClientCategoryId = newCategoryId;

            _db.Clients.Update(client);
            await _db.SaveChangesAsync();
        }

        private static decimal ApplyDiscount(decimal total, decimal discountPercent)
        {
            if (discountPercent <= 0m) return total;
            var coef = 1m - (discountPercent / 100m);
            return total * coef;
        }

        private async Task<(int Id, string Name)?> ResolveCreatedStatusAsync()
        {
            var status = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name != null && EF.Functions.Like(s.Name.ToLower(), $"%{OrderStatusCreatedToken}%"))
                .OrderBy(s => s.Id)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (status != null)
                return (status.Id, status.Name ?? OrderStatusCreatedName);

            var fallback = await _db.OrderStatuses
                .AsNoTracking()
                .Where(s => s.Name == OrderStatusCreatedName)
                .Select(s => new { s.Id, s.Name })
                .FirstOrDefaultAsync();

            if (fallback != null)
                return (fallback.Id, fallback.Name ?? OrderStatusCreatedName);

            return null;
        }

        private static bool IsInactiveCategory(string? categoryName)
        {
            return !string.IsNullOrWhiteSpace(categoryName)
                && categoryName.Trim().Equals(InactiveCategoryName, StringComparison.OrdinalIgnoreCase);
        }

        private static Dictionary<MenuItemLimitKey, decimal> BuildPortionLimitRequirements(OrderRequest request)
        {
            var requirements = new Dictionary<MenuItemLimitKey, decimal>();

            foreach (var dish in request.Dishes ?? [])
            {
                AddPortionRequirement(requirements, KitchenItemTypes.Dish, dish.DishId, dish.Quantity);

                foreach (var topping in dish.Toppings ?? [])
                {
                    var totalToppingQuantity = dish.Quantity * topping.Quantity;
                    AddPortionRequirement(requirements, KitchenItemTypes.Topping, topping.ToppingId, totalToppingQuantity);
                }
            }

            foreach (var drink in request.Drinks ?? [])
            {
                AddPortionRequirement(requirements, KitchenItemTypes.Drink, drink.DrinkId, drink.Quantity);

                foreach (var topping in drink.Toppings ?? [])
                {
                    var totalToppingQuantity = drink.Quantity * topping.Quantity;
                    AddPortionRequirement(requirements, KitchenItemTypes.Topping, topping.ToppingId, totalToppingQuantity);
                }
            }

            foreach (var topping in request.Toppings ?? [])
            {
                AddPortionRequirement(requirements, KitchenItemTypes.Topping, topping.ToppingId, topping.Quantity);
            }

            return requirements;
        }

        private static void AddPortionRequirement(
            IDictionary<MenuItemLimitKey, decimal> target,
            string itemType,
            int itemId,
            decimal quantity)
        {
            if (itemId <= 0 || quantity <= 0m)
            {
                return;
            }

            var key = new MenuItemLimitKey(itemType.Trim().ToLowerInvariant(), itemId);
            if (target.TryGetValue(key, out var current))
            {
                target[key] = current + quantity;
            }
            else
            {
                target[key] = quantity;
            }
        }

        private async Task<PortionLimitConsumeResult> TryConsumePortionLimitsAsync(
            IReadOnlyDictionary<MenuItemLimitKey, decimal> requirementsByItem,
            IReadOnlyDictionary<int, string> dishNamesById,
            IReadOnlyDictionary<int, string> drinkNamesById,
            IReadOnlyDictionary<int, string> toppingNamesById)
        {
            if (requirementsByItem.Count == 0)
            {
                return PortionLimitConsumeResult.Success();
            }

            var itemTypes = requirementsByItem.Keys
                .Select(x => x.ItemType)
                .Distinct()
                .ToList();
            var itemIds = requirementsByItem.Keys
                .Select(x => x.ItemId)
                .Distinct()
                .ToList();

            var limitRows = await _db.MenuItemPortionLimits
                .Where(x =>
                    itemTypes.Contains(x.ItemType) &&
                    itemIds.Contains(x.ItemId))
                .ToListAsync();

            var limitsByKey = limitRows.ToDictionary(
                x => new MenuItemLimitKey(x.ItemType.Trim().ToLowerInvariant(), x.ItemId),
                x => x);

            var conflicts = new List<StockConflictItem>();

            foreach (var pair in requirementsByItem
                .OrderBy(x => x.Key.ItemType)
                .ThenBy(x => x.Key.ItemId))
            {
                if (!limitsByKey.TryGetValue(pair.Key, out var limitRow))
                {
                    continue;
                }

                var available = Math.Max(0m, limitRow.RemainingPortions);
                var required = pair.Value;

                if (available + DecimalEpsilon >= required)
                {
                    continue;
                }

                conflicts.Add(new StockConflictItem
                {
                    SemiFinishedId = pair.Key.ItemId,
                    SemiFinishedName = ResolveMenuItemName(pair.Key, dishNamesById, drinkNamesById, toppingNamesById),
                    Required = Round2(required),
                    Available = Round2(available)
                });
            }

            if (conflicts.Count > 0)
            {
                return PortionLimitConsumeResult.Fail(conflicts);
            }

            var now = DateTime.Now;
            var depletedDishIds = new HashSet<int>();
            var depletedDrinkIds = new HashSet<int>();
            var depletedToppingIds = new HashSet<int>();

            foreach (var pair in requirementsByItem)
            {
                if (!limitsByKey.TryGetValue(pair.Key, out var limitRow))
                {
                    continue;
                }

                var current = Math.Max(0m, limitRow.RemainingPortions);
                var remaining = current - pair.Value;
                var roundedRemaining = Round2(Math.Max(0m, remaining));

                if (remaining <= DecimalEpsilon || roundedRemaining <= 0m)
                {
                    limitRow.RemainingPortions = 0m;

                    if (pair.Key.ItemType == KitchenItemTypes.Dish)
                    {
                        depletedDishIds.Add(pair.Key.ItemId);
                    }
                    else if (pair.Key.ItemType == KitchenItemTypes.Drink)
                    {
                        depletedDrinkIds.Add(pair.Key.ItemId);
                    }
                    else if (pair.Key.ItemType == KitchenItemTypes.Topping)
                    {
                        depletedToppingIds.Add(pair.Key.ItemId);
                    }
                }
                else
                {
                    limitRow.RemainingPortions = roundedRemaining;
                }

                limitRow.UpdatedAt = now;
            }

            if (depletedDishIds.Count > 0)
            {
                var dishes = await _db.Dishes
                    .Where(x => depletedDishIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var dish in dishes)
                {
                    dish.IsAvailable = false;
                }
            }

            if (depletedDrinkIds.Count > 0)
            {
                var drinks = await _db.Drinks
                    .Where(x => depletedDrinkIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var drink in drinks)
                {
                    drink.IsAvailable = false;
                }
            }

            if (depletedToppingIds.Count > 0)
            {
                var toppings = await _db.ToppingsAndSyrups
                    .Where(x => depletedToppingIds.Contains(x.Id))
                    .ToListAsync();

                foreach (var topping in toppings)
                {
                    topping.IsAvailable = false;
                }
            }

            return PortionLimitConsumeResult.Success();
        }

        private static string ResolveMenuItemName(
            MenuItemLimitKey key,
            IReadOnlyDictionary<int, string> dishNamesById,
            IReadOnlyDictionary<int, string> drinkNamesById,
            IReadOnlyDictionary<int, string> toppingNamesById)
        {
            if (key.ItemType == KitchenItemTypes.Dish)
            {
                var name = dishNamesById.TryGetValue(key.ItemId, out var dishName)
                    ? dishName
                    : $"Блюдо #{key.ItemId}";
                return $"Блюдо: {name}";
            }

            if (key.ItemType == KitchenItemTypes.Drink)
            {
                var name = drinkNamesById.TryGetValue(key.ItemId, out var drinkName)
                    ? drinkName
                    : $"Напиток #{key.ItemId}";
                return $"Напиток: {name}";
            }

            var resolvedToppingName = toppingNamesById.TryGetValue(key.ItemId, out var toppingName)
                ? toppingName
                : $"Добавка #{key.ItemId}";
            return $"Добавка: {resolvedToppingName}";
        }

        private async Task ApplyDefaultDrinkModifiersAsync(IReadOnlyCollection<OrderDrinkItemRequest> drinks)
        {
            if (drinks.Count == 0)
            {
                return;
            }

            var targetDrinks = drinks
                .Where(x => !ModifierExcludedDrinkIds.Contains(x.DrinkId))
                .ToList();
            if (targetDrinks.Count == 0)
            {
                return;
            }

            var needsMilkDefault = targetDrinks.Any(x => !x.MilkIngredientId.HasValue);
            var needsCoffeeDefault = targetDrinks.Any(x => !x.CoffeeIngredientId.HasValue);
            if (!needsMilkDefault && !needsCoffeeDefault)
            {
                return;
            }

            var idsToLoad = new HashSet<int>();
            if (needsMilkDefault)
            {
                foreach (var id in MilkModifierIngredientIds)
                {
                    idsToLoad.Add(id);
                }
            }

            if (needsCoffeeDefault)
            {
                foreach (var id in CoffeeModifierIngredientIds)
                {
                    idsToLoad.Add(id);
                }
            }

            if (idsToLoad.Count == 0)
            {
                return;
            }

            var modifierInfoById = await _db.Ingredients
                .AsNoTracking()
                .Where(i => idsToLoad.Contains(i.Id))
                .Select(i => new
                {
                    i.Id,
                    i.Name,
                    i.CategoryId
                })
                .ToDictionaryAsync(
                    i => i.Id,
                    i => new ModifierInfo(i.Name, i.CategoryId));

            var defaultMilkModifierId = needsMilkDefault
                ? ResolveDefaultModifierId(
                    MilkModifierIngredientIds,
                    MilkCategoryId,
                    PreferredMilkModifierName,
                    modifierInfoById)
                : null;

            var defaultCoffeeModifierId = needsCoffeeDefault
                ? ResolveDefaultModifierId(
                    CoffeeModifierIngredientIds,
                    CoffeeCategoryId,
                    PreferredCoffeeModifierName,
                    modifierInfoById)
                : null;

            foreach (var drink in targetDrinks)
            {
                if (!drink.MilkIngredientId.HasValue && defaultMilkModifierId.HasValue)
                {
                    drink.MilkIngredientId = defaultMilkModifierId.Value;
                }

                if (!drink.CoffeeIngredientId.HasValue && defaultCoffeeModifierId.HasValue)
                {
                    drink.CoffeeIngredientId = defaultCoffeeModifierId.Value;
                }
            }
        }

        private static int? ResolveDefaultModifierId(
            IReadOnlyList<int> orderedIds,
            int categoryId,
            string preferredName,
            IReadOnlyDictionary<int, ModifierInfo> modifierInfoById)
        {
            var normalizedPreferredName = NormalizeModifierName(preferredName);
            int? firstAvailableId = null;

            foreach (var id in orderedIds)
            {
                if (!modifierInfoById.TryGetValue(id, out var modifierInfo) || modifierInfo.CategoryId != categoryId)
                {
                    continue;
                }

                firstAvailableId ??= id;

                if (NormalizeModifierName(modifierInfo.Name) == normalizedPreferredName)
                {
                    return id;
                }
            }

            return firstAvailableId;
        }

        private static string NormalizeModifierName(string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            var normalizedSpaces = string.Join(" ", value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
            return normalizedSpaces.ToUpperInvariant();
        }

        private async Task<string?> ValidateDrinkModifiersAsync(IReadOnlyCollection<OrderDrinkItemRequest> drinks)
        {
            var allModifierIds = new HashSet<int>();

            foreach (var drink in drinks)
            {
                if (drink.MilkIngredientId.HasValue)
                {
                    var milkIngredientId = drink.MilkIngredientId.Value;
                    if (!MilkModifierIngredientIds.Contains(milkIngredientId))
                    {
                        return $"MilkIngredientId={milkIngredientId} не входит в список разрешённых модификаторов.";
                    }

                    allModifierIds.Add(milkIngredientId);
                }

                if (drink.CoffeeIngredientId.HasValue)
                {
                    var coffeeIngredientId = drink.CoffeeIngredientId.Value;
                    if (!CoffeeModifierIngredientIds.Contains(coffeeIngredientId))
                    {
                        return $"CoffeeIngredientId={coffeeIngredientId} не входит в список разрешённых модификаторов.";
                    }

                    allModifierIds.Add(coffeeIngredientId);
                }
            }

            if (allModifierIds.Count == 0)
            {
                return null;
            }

            var ingredientCategories = await _db.Ingredients
                .Where(i => allModifierIds.Contains(i.Id))
                .Select(i => new { i.Id, i.CategoryId })
                .ToDictionaryAsync(i => i.Id, i => i.CategoryId);

            foreach (var modifierId in allModifierIds)
            {
                if (!ingredientCategories.ContainsKey(modifierId))
                {
                    return $"Ингредиент-модификатор Id={modifierId} не найден.";
                }
            }

            foreach (var drink in drinks)
            {
                if (drink.MilkIngredientId.HasValue)
                {
                    var milkIngredientId = drink.MilkIngredientId.Value;
                    if (!ingredientCategories.TryGetValue(milkIngredientId, out var milkCategoryId) || milkCategoryId != MilkCategoryId)
                    {
                        return $"Ингредиент Id={milkIngredientId} не относится к категории молока.";
                    }
                }

                if (drink.CoffeeIngredientId.HasValue)
                {
                    var coffeeIngredientId = drink.CoffeeIngredientId.Value;
                    if (!ingredientCategories.TryGetValue(coffeeIngredientId, out var coffeeCategoryId) || coffeeCategoryId != CoffeeCategoryId)
                    {
                        return $"Ингредиент Id={coffeeIngredientId} не относится к категории кофе.";
                    }
                }
            }

            return null;
        }

        private static DateTime NormalizePickupAt(DateTime value)
        {
            var local = value.Kind == DateTimeKind.Utc
                ? value.ToLocalTime()
                : value;

            return new DateTime(
                local.Year,
                local.Month,
                local.Day,
                local.Hour,
                local.Minute,
                local.Second,
                DateTimeKind.Unspecified);
        }

        private readonly record struct MenuItemLimitKey(string ItemType, int ItemId);

        private sealed class PortionLimitConsumeResult
        {
            public bool IsSuccess { get; private set; }
            public List<StockConflictItem> Items { get; private set; } = new List<StockConflictItem>();

            public static PortionLimitConsumeResult Success()
            {
                return new PortionLimitConsumeResult
                {
                    IsSuccess = true
                };
            }

            public static PortionLimitConsumeResult Fail(List<StockConflictItem> items)
            {
                return new PortionLimitConsumeResult
                {
                    IsSuccess = false,
                    Items = items ?? new List<StockConflictItem>()
                };
            }
        }

        private sealed class DiscountResolution
        {
            public bool Success { get; private set; }
            public string Message { get; private set; } = string.Empty;
            public int? DiscountId { get; private set; }
            public decimal DiscountPercent { get; private set; }

            public static DiscountResolution Ok(int? discountId, decimal discountPercent)
            {
                return new DiscountResolution
                {
                    Success = true,
                    DiscountId = discountId,
                    DiscountPercent = discountPercent
                };
            }

            public static DiscountResolution Fail(string message)
            {
                return new DiscountResolution
                {
                    Success = false,
                    Message = message
                };
            }
        }

        private sealed record ModifierInfo(string? Name, int? CategoryId);

        private static decimal Round2(decimal value)
            => Math.Round(value, 2, MidpointRounding.AwayFromZero);
    }
}
