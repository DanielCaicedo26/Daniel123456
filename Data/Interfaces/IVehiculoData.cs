using Entity.Dtos;
using Entity.Model;

namespace Data.Interfaces
{
    public interface IVehiculoData : IBaseModelData<Vehiculo>
    {
        Task<IEnumerable<Vehiculo>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehiculo>> GetVehiclesByTypeAsync(string tipo);
        Task<Vehiculo?> GetByPlacaAsync(string placa);
        Task<bool> ExistsPlacaAsync(string placa, int? excludeId = null);
        Task<bool> IsVehicleAvailableAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<Vehiculo>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
    }
}