using Entity.Dtos;

namespace Business.Interfaces
{
    public interface IVehiculoBusiness : IBaseBusiness<Vehiculo, VehiculoDto>
    {
        Task<IEnumerable<VehiculoDto>> GetAvailableVehiclesAsync();
        Task<IEnumerable<VehiculoDto>> GetVehiclesByTypeAsync(string tipo);
        Task<VehiculoDto?> GetByPlacaAsync(string placa);
        Task<VehiculoDto> CreateVehiculoAsync(VehiculoCreateDto dto);
        Task<VehiculoDto> UpdateVehiculoAsync(int id, VehiculoUpdateDto dto);
        Task<bool> ValidateUniquePlacaAsync(string placa, int? excludeId = null);
        Task<bool> IsVehicleAvailableAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin);
        Task<IEnumerable<VehiculoDto>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice);
        Task<bool> SetAvailabilityAsync(int id, bool available);
    }
}