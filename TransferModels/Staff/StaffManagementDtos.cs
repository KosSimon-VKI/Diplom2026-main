using System.Collections.Generic;

namespace TransferModels.Staff
{
    public class StaffManagementDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public int? RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty;
    }

    public class StaffUpsertRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Login { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int? RoleId { get; set; }
    }

    public class StaffRoleOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class StaffEditOptionsResponse
    {
        public List<StaffRoleOptionDto> Roles { get; set; } = new List<StaffRoleOptionDto>();
    }
}
