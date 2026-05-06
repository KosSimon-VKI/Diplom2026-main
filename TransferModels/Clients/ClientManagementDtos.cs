using System.Collections.Generic;

namespace TransferModels.Clients
{
    public class ClientManagementDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int? ClientCategoryId { get; set; }
        public string ClientCategoryName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
    }

    public class ClientCategoryOptionDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class ClientEditOptionsResponse
    {
        public List<ClientCategoryOptionDto> Categories { get; set; } = new List<ClientCategoryOptionDto>();
    }

    public class ClientCategoryUpdateRequest
    {
        public int? ClientCategoryId { get; set; }
    }
}
