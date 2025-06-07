using Entity.Dtos;

namespace Data.Interfaces
{
    public interface IReservaData : IBaseModelData<Reserva>
    {
        Task<IEnumerable<Reserva>> GetReservasByClienteAsync(int clienteId);
        Task<IEnumerable<Reserva>> GetReservasByVehiculoAsync(int vehiculoId);
        Task<IEnumerable<Reserva>> GetReservasByDateRangeAsync(DateTime startDate, DateTime endDate);
        Task<IEnumerable<Reserva>> GetActiveReservationsAsync();
        Task<IEnumerable<Reserva>> GetReservasByEstadoAsync(string estado);
        Task<bool> HasConflictingReservationAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin, int? excludeReservaId = null);
        Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id);
        Task<IEnumerable<ReservaDetalleDto>> GetReservasDetalleAsync();
    }
}
