using GardenNookApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Loyalty;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/loyalty")]
    [Authorize(Roles = "Администратор")]
    public class LoyaltyController : ControllerBase
    {
        private readonly AppDbContext _db;

        public LoyaltyController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet("discounts")]
        public async Task<ActionResult<List<DiscountManagementDto>>> GetDiscounts()
        {
            var discounts = await _db.Discounts
                .AsNoTracking()
                .Select(x => new DiscountManagementDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    DiscountPercent = x.DiscountPercent ?? 0m,
                    OrdersCount = x.Orders.Count
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return Ok(discounts);
        }

        [HttpPost("discounts")]
        public async Task<ActionResult<DiscountManagementDto>> CreateDiscount(DiscountUpsertRequest request)
        {
            var validationError = await ValidateDiscountRequestAsync(request, null);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var discount = new Discount();
            ApplyDiscountRequest(discount, request);

            _db.Discounts.Add(discount);
            await _db.SaveChangesAsync();

            return Ok(await LoadDiscountAsync(discount.Id));
        }

        [HttpPut("discounts/{id:int}")]
        public async Task<IActionResult> UpdateDiscount(int id, DiscountUpsertRequest request)
        {
            var validationError = await ValidateDiscountRequestAsync(request, id);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var discount = await _db.Discounts.FirstOrDefaultAsync(x => x.Id == id);
            if (discount == null)
            {
                return NotFound("Скидка не найдена.");
            }

            ApplyDiscountRequest(discount, request);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("discounts/{id:int}")]
        public async Task<IActionResult> DeleteDiscount(int id)
        {
            var discount = await _db.Discounts.FirstOrDefaultAsync(x => x.Id == id);
            if (discount == null)
            {
                return NotFound("Скидка не найдена.");
            }

            var ordersCount = await _db.Orders.CountAsync(x => x.DiscountId == id);
            if (ordersCount > 0)
            {
                return Conflict($"Нельзя удалить скидку: она используется в заказах ({ordersCount}).");
            }

            _db.Discounts.Remove(discount);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить скидку: она используется в связанных данных.");
            }
        }

        [HttpGet("client-categories")]
        public async Task<ActionResult<List<ClientCategoryManagementDto>>> GetClientCategories()
        {
            var categories = await _db.ClientCategories
                .AsNoTracking()
                .Select(x => new ClientCategoryManagementDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    ClientsCount = x.Clients.Count
                })
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .ToListAsync();

            return Ok(categories);
        }

        [HttpPost("client-categories")]
        public async Task<ActionResult<ClientCategoryManagementDto>> CreateClientCategory(ClientCategoryUpsertRequest request)
        {
            var validationError = await ValidateClientCategoryRequestAsync(request, null);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var category = new ClientCategory
            {
                Name = NormalizeName(request.Name)
            };

            _db.ClientCategories.Add(category);
            await _db.SaveChangesAsync();

            return Ok(await LoadClientCategoryAsync(category.Id));
        }

        [HttpPut("client-categories/{id:int}")]
        public async Task<IActionResult> UpdateClientCategory(int id, ClientCategoryUpsertRequest request)
        {
            var validationError = await ValidateClientCategoryRequestAsync(request, id);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var category = await _db.ClientCategories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound("Категория клиента не найдена.");
            }

            category.Name = NormalizeName(request.Name);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("client-categories/{id:int}")]
        public async Task<IActionResult> DeleteClientCategory(int id)
        {
            var category = await _db.ClientCategories.FirstOrDefaultAsync(x => x.Id == id);
            if (category == null)
            {
                return NotFound("Категория клиента не найдена.");
            }

            var clientsCount = await _db.Clients.CountAsync(x => x.ClientCategoryId == id);
            if (clientsCount > 0)
            {
                return Conflict($"Нельзя удалить категорию клиента: она назначена клиентам ({clientsCount}).");
            }

            _db.ClientCategories.Remove(category);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить категорию клиента: она используется в связанных данных.");
            }
        }

        private async Task<DiscountManagementDto> LoadDiscountAsync(int id)
        {
            return await _db.Discounts
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new DiscountManagementDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    DiscountPercent = x.DiscountPercent ?? 0m,
                    OrdersCount = x.Orders.Count
                })
                .FirstAsync();
        }

        private async Task<ClientCategoryManagementDto> LoadClientCategoryAsync(int id)
        {
            return await _db.ClientCategories
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ClientCategoryManagementDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty,
                    ClientsCount = x.Clients.Count
                })
                .FirstAsync();
        }

        private async Task<string?> ValidateDiscountRequestAsync(DiscountUpsertRequest? request, int? exceptId)
        {
            if (request == null)
            {
                return "Заполните данные скидки.";
            }

            var nameError = ValidateName(NormalizeName(request.Name), "скидки");
            if (nameError != null)
            {
                return nameError;
            }

            if (request.DiscountPercent < 0m || request.DiscountPercent > 100m)
            {
                return "Процент скидки должен быть от 0 до 100.";
            }

            if (await DiscountNameExistsAsync(request.Name, exceptId))
            {
                return "Скидка с таким названием уже существует.";
            }

            return null;
        }

        private async Task<string?> ValidateClientCategoryRequestAsync(ClientCategoryUpsertRequest? request, int? exceptId)
        {
            if (request == null)
            {
                return "Заполните данные категории клиента.";
            }

            var nameError = ValidateName(NormalizeName(request.Name), "категории клиента");
            if (nameError != null)
            {
                return nameError;
            }

            if (await ClientCategoryNameExistsAsync(request.Name, exceptId))
            {
                return "Категория клиента с таким названием уже существует.";
            }

            return null;
        }

        private static void ApplyDiscountRequest(Discount discount, DiscountUpsertRequest request)
        {
            discount.Name = NormalizeName(request.Name);
            discount.DiscountPercent = Math.Round(request.DiscountPercent, 2, MidpointRounding.AwayFromZero);
        }

        private async Task<bool> DiscountNameExistsAsync(string name, int? exceptId)
        {
            var normalizedName = NormalizeName(name);
            var rows = await _db.Discounts
                .AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            return rows.Any(x =>
                (!exceptId.HasValue || x.Id != exceptId.Value) &&
                string.Equals(NormalizeName(x.Name), normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<bool> ClientCategoryNameExistsAsync(string name, int? exceptId)
        {
            var normalizedName = NormalizeName(name);
            var rows = await _db.ClientCategories
                .AsNoTracking()
                .Select(x => new { x.Id, x.Name })
                .ToListAsync();

            return rows.Any(x =>
                (!exceptId.HasValue || x.Id != exceptId.Value) &&
                string.Equals(NormalizeName(x.Name), normalizedName, StringComparison.OrdinalIgnoreCase));
        }

        private static string? ValidateName(string name, string entityName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return $"Введите название {entityName}.";
            }

            if (name.Length > 50)
            {
                return $"Название {entityName} не должно быть длиннее 50 символов.";
            }

            return null;
        }

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
