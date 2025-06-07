using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implements
{
    public class VehiculoData : BaseModelData<Vehiculo>, IVehiculoData
    {
        public VehiculoData(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<Vehiculo>> GetAvailableVehiclesAsync()
        {
            return await _dbSet
                .Where(v => v.EstaDisponible && v.Activo)
                .OrderBy(v => v.Marca)
                .ThenBy(v => v.Modelo)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehiculo>> GetVehiclesByTypeAsync(string tipo)
        {
            return await _dbSet
                .Where(v => v.Tipo.ToLower() == tipo.ToLower() && v.Activo)
                .OrderBy(v => v.PrecioPorDia)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Vehiculo?> GetByPlacaAsync(string placa)
        {
            return await _dbSet
                .Include(v => v.Reservas)
                .FirstOrDefaultAsync(v => v.Placa == placa && v.Activo);
        }

        public async Task<bool> ExistsPlacaAsync(string placa, int? excludeId = null)
        {
            var query = _dbSet.Where(v => v.Placa == placa && v.Activo);

            if (excludeId.HasValue)
                query = query.Where(v => v.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            var vehiculo = await _dbSet.FindAsync(vehiculoId);
            if (vehiculo == null || !vehiculo.EstaDisponible || !vehiculo.Activo)
                return false;

            var hasConflict = await _context.Reservas
                .AnyAsync(r => r.VehiculoId == vehiculoId &&
                              r.Activo &&
                              r.Estado != "Cancelada" &&
                              ((fechaInicio >= r.FechaInicio && fechaInicio < r.FechaFin) ||
                               (fechaFin > r.FechaInicio && fechaFin <= r.FechaFin) ||
                               (fechaInicio <= r.FechaInicio && fechaFin >= r.FechaFin)));

            return !hasConflict;
        }

        public async Task<IEnumerable<Vehiculo>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _dbSet
                .Where(v => v.PrecioPorDia >= minPrice &&
                           v.PrecioPorDia <= maxPrice &&
                           v.Activo)
                .OrderBy(v => v.PrecioPorDia)
                .AsNoTracking()
                .ToListAsync();
        }

        public override async Task<Vehiculo?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(v => v.Reservas)
                    .ThenInclude(r => r.Cliente)
                .FirstOrDefaultAsync(v => v.Id == id);
        }
    }
}
