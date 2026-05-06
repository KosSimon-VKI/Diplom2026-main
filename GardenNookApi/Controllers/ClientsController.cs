using GardenNookApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Clients;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/clients")]
    [Authorize(Roles = "Администратор")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ClientsController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<ClientManagementDto>>> GetClients([FromQuery] string? search)
        {
            var query = _db.Clients
                .AsNoTracking()
                .Include(x => x.ClientCategory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";
                query = query.Where(x =>
                    (x.FullName != null && EF.Functions.Like(x.FullName, pattern)) ||
                    (x.PhoneNumber != null && EF.Functions.Like(x.PhoneNumber, pattern)) ||
                    (x.ClientCategory != null && x.ClientCategory.Name != null && EF.Functions.Like(x.ClientCategory.Name, pattern)));
            }

            return Ok(await query
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.PhoneNumber)
                .ThenBy(x => x.Id)
                .Select(x => new ClientManagementDto
                {
                    Id = x.Id,
                    FullName = x.FullName ?? string.Empty,
                    PhoneNumber = x.PhoneNumber ?? string.Empty,
                    ClientCategoryId = x.ClientCategoryId,
                    ClientCategoryName = x.ClientCategory != null ? x.ClientCategory.Name ?? string.Empty : string.Empty,
                    OrderCount = x.Orders.Count
                })
                .ToListAsync());
        }

        [HttpGet("edit-options")]
        public async Task<ActionResult<ClientEditOptionsResponse>> GetEditOptions()
        {
            var categories = await _db.ClientCategories
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new ClientCategoryOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            return Ok(new ClientEditOptionsResponse { Categories = categories });
        }

        [HttpPut("{id:int}/category")]
        public async Task<IActionResult> UpdateClientCategory(int id, ClientCategoryUpdateRequest request)
        {
            if (request == null)
            {
                return BadRequest("Заполните данные клиента.");
            }

            if (!request.ClientCategoryId.HasValue || request.ClientCategoryId.Value <= 0)
            {
                return BadRequest("Выберите категорию клиента.");
            }

            var categoryExists = await _db.ClientCategories
                .AsNoTracking()
                .AnyAsync(x => x.Id == request.ClientCategoryId.Value);

            if (!categoryExists)
            {
                return BadRequest("Выбранная категория клиента не найдена.");
            }

            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null)
            {
                return NotFound("Клиент не найден.");
            }

            client.ClientCategoryId = request.ClientCategoryId.Value;
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteClient(int id)
        {
            var client = await _db.Clients.FirstOrDefaultAsync(x => x.Id == id);
            if (client == null)
            {
                return NotFound("Клиент не найден.");
            }

            var ordersCount = await _db.Orders.CountAsync(x => x.ClientId == id);
            if (ordersCount > 0)
            {
                return Conflict($"Нельзя удалить клиента: у него есть связанные заказы ({ordersCount}).");
            }

            _db.Clients.Remove(client);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить клиента: он используется в связанных данных.");
            }
        }
    }
}
