using Entity.Dtos;

namespace Business.Interfaces
{
    public interface IReservaBusiness : IBaseBusiness<Reserva, ReservaDto>
    {
        Task<IEnumerable<ReservaDto>> GetReservasByClienteAsync(int clienteId);
        Task<IEnumerable<ReservaDto>> GetReservasByVehiculoAsync(int vehiculoId);
        Task<IEnumerable<ReservaDto>> GetReservasByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<ReservaDto>> GetActiveReservationsAsync();
        Task<IEnumerable<ReservaDto>> GetReservasByEstadoAsync(string estado);
        Task<ReservaDto> CreateReservaAsync(ReservaCreateDto dto);
        Task<ReservaDto> UpdateReservaAsync(int id, ReservaUpdateDto dto);
        Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id);
        Task<IEnumerable<ReservaDetalleDto>> GetReservasDetalleAsync();
        Task<ReservaDto> ChangeEstadoAsync(int id, string nuevoEstado);
        Task<bool> ValidateReservationDatesAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin, int? excludeReservaId = null);
        Task<decimal> CalculateMontoTotalAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin);
        Task<bool> CancelReservaAsync(int id, string motivo = "");
    }
}