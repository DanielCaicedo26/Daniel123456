using Data.Interfaces;
using Entity.Context;
using Entity.Dtos;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implements
{
    public class ReservaData : BaseModelData<Reserva>, IReservaData
    {
        public ReservaData(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Reserva>> GetReservasByClienteAsync(int clienteId)
        {
            return await _dbSet
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r => r.ClienteId == clienteId && r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetReservasByVehiculoAsync(int vehiculoId)
        {
            return await _dbSet
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r => r.VehiculoId == vehiculoId && r.Activo)
                .OrderByDescending(r => r.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetReservasByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r => r.FechaInicio >= startDate &&
                           r.FechaFin <= endDate &&
                           r.Activo)
                .OrderBy(r => r.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetActiveReservationsAsync()
        {
            var today = DateTime.Today;
            return await _dbSet
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r => r.FechaFin >= today &&
                           r.Estado != "Cancelada" &&
                           r.Activo)
                .OrderBy(r => r.FechaInicio)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Reserva>> GetReservasByEstadoAsync(string estado)
        {
            return await _dbSet
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .Where(r => r.Estado == estado && r.Activo)
                .OrderByDescending(r => r.FechaCreacion)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> HasConflictingReservationAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin, int? excludeReservaId = null)
        {
            var query = _dbSet.Where(r => r.VehiculoId == vehiculoId &&
                                         r.Activo &&
                                         r.Estado != "Cancelada" &&
                                         ((fechaInicio >= r.FechaInicio && fechaInicio < r.FechaFin) ||
                                          (fechaFin > r.FechaInicio && fechaFin <= r.FechaFin) ||
                                          (fechaInicio <= r.FechaInicio && fechaFin >= r.FechaFin)));

            if (excludeReservaId.HasValue)
                query = query.Where(r => r.Id != excludeReservaId.Value);

            return await query.AnyAsync();
        }

        public async Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id)
        {
            return await _dbSet
                .Where(r => r.Id == id && r.Activo)
                .Select(r => new ReservaDetalleDto
                {
                    Id = r.Id,
                    FechaInicio = r.FechaInicio,
                    FechaFin = r.FechaFin,
                    MontoTotal = r.MontoTotal,
                    Estado = r.Estado,
                    Observaciones = r.Observaciones,
                    ClienteNombre = $"{r.Cliente.Nombre} {r.Cliente.Apellido}",
                    ClienteEmail = r.Cliente.Email,
                    ClienteTelefono = r.Cliente.Telefono,
                    VehiculoMarca = r.Vehiculo.Marca,
                    VehiculoModelo = r.Vehiculo.Modelo,
                    VehiculoPlaca = r.Vehiculo.Placa,
                    VehiculoPrecioPorDia = r.Vehiculo.PrecioPorDia
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ReservaDetalleDto>> GetReservasDetalleAsync()
        {
            return await _dbSet
                .Where(r => r.Activo)
                .Select(r => new ReservaDetalleDto
                {
                    Id = r.Id,
                    FechaInicio = r.FechaInicio,
                    FechaFin = r.FechaFin,
                    MontoTotal = r.MontoTotal,
                    Estado = r.Estado,
                    Observaciones = r.Observaciones,
                    ClienteNombre = $"{r.Cliente.Nombre} {r.Cliente.Apellido}",
                    ClienteEmail = r.Cliente.Email,
                    ClienteTelefono = r.Cliente.Telefono,
                    VehiculoMarca = r.Vehiculo.Marca,
                    VehiculoModelo = r.Vehiculo.Modelo,
                    VehiculoPlaca = r.Vehiculo.Placa,
                    VehiculoPrecioPorDia = r.Vehiculo.PrecioPorDia
                })
                .OrderByDescending(r => r.Id)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Reserva?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(r => r.Cliente)
                .Include(r => r.Vehiculo)
                .FirstOrDefaultAsync(r => r.Id == id);
        }
    }
}