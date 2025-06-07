using Entity.Dtos;

namespace Business.Interfaces
{
    public interface IClienteBusiness : IBaseBusiness<Cliente, ClienteDto>
    {
        Task<ClienteDto?> GetByEmailAsync(string email);
        Task<ClienteDto?> GetByDNIAsync(string dni);
        Task<IEnumerable<ClienteDto>> GetActiveClientsAsync();
        Task<ClienteDto> CreateClienteAsync(ClienteCreateDto dto);
        Task<ClienteDto> UpdateClienteAsync(int id, ClienteUpdateDto dto);
        Task<bool> ValidateUniqueEmailAsync(string email, int? excludeId = null);
        Task<bool> ValidateUniqueDNIAsync(string dni, int? excludeId = null);
    }
}
