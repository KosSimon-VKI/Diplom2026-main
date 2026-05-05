using System.Security.Claims;
using GardenNookApi.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TransferModels.Staff;

namespace GardenNookApi.Controllers
{
    [ApiController]
    [Route("api/staff")]
    [Authorize(Roles = "Администратор")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _db;

        public StaffController(AppDbContext db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<List<StaffManagementDto>>> GetStaff([FromQuery] string? search)
        {
            var query = _db.Staff
                .AsNoTracking()
                .Include(x => x.Role)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var pattern = $"%{search.Trim()}%";
                query = query.Where(x =>
                    (x.FullName != null && EF.Functions.Like(x.FullName, pattern)) ||
                    (x.Login != null && EF.Functions.Like(x.Login, pattern)) ||
                    (x.Role != null && x.Role.Name != null && EF.Functions.Like(x.Role.Name, pattern)));
            }

            return Ok(await query
                .OrderBy(x => x.FullName)
                .ThenBy(x => x.Login)
                .ThenBy(x => x.Id)
                .Select(x => new StaffManagementDto
                {
                    Id = x.Id,
                    FullName = x.FullName ?? string.Empty,
                    Login = x.Login ?? string.Empty,
                    RoleId = x.RoleId,
                    RoleName = x.Role != null ? x.Role.Name ?? string.Empty : string.Empty
                })
                .ToListAsync());
        }

        [HttpGet("edit-options")]
        public async Task<ActionResult<StaffEditOptionsResponse>> GetEditOptions()
        {
            var roles = await _db.StaffRoles
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .ThenBy(x => x.Id)
                .Select(x => new StaffRoleOptionDto
                {
                    Id = x.Id,
                    Name = x.Name ?? string.Empty
                })
                .ToListAsync();

            return Ok(new StaffEditOptionsResponse { Roles = roles });
        }

        [HttpPost]
        public async Task<ActionResult<StaffManagementDto>> CreateStaff(StaffUpsertRequest request)
        {
            var validationError = await ValidateStaffRequestAsync(request, null, true);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var staff = new Staff();
            ApplyStaffRequest(staff, request, true);

            _db.Staff.Add(staff);
            await _db.SaveChangesAsync();

            return Ok(await LoadStaffAsync(staff.Id));
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateStaff(int id, StaffUpsertRequest request)
        {
            var validationError = await ValidateStaffRequestAsync(request, id, false);
            if (validationError != null)
            {
                return BadRequest(validationError);
            }

            var staff = await _db.Staff.FirstOrDefaultAsync(x => x.Id == id);
            if (staff == null)
            {
                return NotFound("Сотрудник не найден.");
            }

            ApplyStaffRequest(staff, request, false);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            var staff = await _db.Staff.FirstOrDefaultAsync(x => x.Id == id);
            if (staff == null)
            {
                return NotFound("Сотрудник не найден.");
            }

            if (IsCurrentStaff(id))
            {
                return Conflict("Нельзя удалить текущего авторизованного сотрудника.");
            }

            var writeOffActsCount = await _db.WriteOffActs.CountAsync(x => x.StaffId == id);
            if (writeOffActsCount > 0)
            {
                return Conflict($"Нельзя удалить сотрудника: он используется в связанных данных ({writeOffActsCount}).");
            }

            _db.Staff.Remove(staff);
            try
            {
                await _db.SaveChangesAsync();
                return NoContent();
            }
            catch (DbUpdateException)
            {
                return Conflict("Нельзя удалить сотрудника: он используется в связанных данных.");
            }
        }

        private async Task<StaffManagementDto> LoadStaffAsync(int id)
        {
            return await _db.Staff
                .AsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new StaffManagementDto
                {
                    Id = x.Id,
                    FullName = x.FullName ?? string.Empty,
                    Login = x.Login ?? string.Empty,
                    RoleId = x.RoleId,
                    RoleName = x.Role != null ? x.Role.Name ?? string.Empty : string.Empty
                })
                .FirstAsync();
        }

        private async Task<string?> ValidateStaffRequestAsync(StaffUpsertRequest? request, int? exceptId, bool isCreate)
        {
            if (request == null)
            {
                return "Заполните данные сотрудника.";
            }

            var fullName = NormalizeName(request.FullName);
            if (string.IsNullOrWhiteSpace(fullName))
            {
                return "Введите ФИО сотрудника.";
            }

            if (fullName.Length > 255)
            {
                return "ФИО сотрудника не должно быть длиннее 255 символов.";
            }

            var login = NormalizeLogin(request.Login);
            if (string.IsNullOrWhiteSpace(login))
            {
                return "Введите логин сотрудника.";
            }

            if (login.Length > 100)
            {
                return "Логин сотрудника не должен быть длиннее 100 символов.";
            }

            var password = request.Password?.Trim() ?? string.Empty;
            if (isCreate && string.IsNullOrWhiteSpace(password))
            {
                return "Введите пароль сотрудника.";
            }

            if (password.Length > 100)
            {
                return "Пароль сотрудника не должен быть длиннее 100 символов.";
            }

            if (!request.RoleId.HasValue || request.RoleId.Value <= 0)
            {
                return "Выберите роль сотрудника.";
            }

            if (!await _db.StaffRoles.AsNoTracking().AnyAsync(x => x.Id == request.RoleId.Value))
            {
                return "Выбранная роль сотрудника не найдена.";
            }

            if (await LoginExistsAsync(login, exceptId))
            {
                return "Сотрудник с таким логином уже существует.";
            }

            return null;
        }

        private async Task<bool> LoginExistsAsync(string login, int? exceptId)
        {
            var normalizedLogin = NormalizeLogin(login);
            var rows = await _db.Staff
                .AsNoTracking()
                .Select(x => new { x.Id, x.Login })
                .ToListAsync();

            return rows.Any(x =>
                (!exceptId.HasValue || x.Id != exceptId.Value) &&
                string.Equals(NormalizeLogin(x.Login), normalizedLogin, StringComparison.OrdinalIgnoreCase));
        }

        private static void ApplyStaffRequest(Staff staff, StaffUpsertRequest request, bool isCreate)
        {
            staff.FullName = NormalizeName(request.FullName);
            staff.Login = NormalizeLogin(request.Login);
            staff.RoleId = request.RoleId;

            var password = request.Password?.Trim() ?? string.Empty;
            if (isCreate || !string.IsNullOrWhiteSpace(password))
            {
                staff.Password = password;
            }
        }

        private bool IsCurrentStaff(int staffId)
        {
            var raw = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(raw, out var currentStaffId) && currentStaffId == staffId;
        }

        private static string NormalizeName(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }

        private static string NormalizeLogin(string? value)
        {
            return string.Join(" ", (value ?? string.Empty)
                .Trim()
                .Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));
        }
    }
}
