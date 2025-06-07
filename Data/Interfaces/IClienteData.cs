using Entity.Dtos;
using Entity.Model;

namespace Data.Interfaces
{
    public interface IClienteData : IBaseModelData<Cliente>
    {
        Task<Cliente?> GetByEmailAsync(string email);
        Task<Cliente?> GetByDNIAsync(string dni);
        Task<IEnumerable<Cliente>> GetActiveClientsAsync();
        Task<bool> ExistsEmailAsync(string email, int? excludeId = null);
        Task<bool> ExistsDNIAsync(string dni, int? excludeId = null);
    }
}