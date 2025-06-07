using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implements
{
    public class ClienteData : BaseModelData<Cliente>, IClienteData
    {
        public ClienteData(ApplicationDbContext context) : base(context) { }

        public async Task<Cliente?> GetByEmailAsync(string email)
        {
            return await _dbSet
                .Include(c => c.Reservas)
                .FirstOrDefaultAsync(c => c.Email == email && c.Activo);
        }

        public async Task<Cliente?> GetByDNIAsync(string dni)
        {
            return await _dbSet
                .Include(c => c.Reservas)
                .FirstOrDefaultAsync(c => c.DNI == dni && c.Activo);
        }

        public async Task<IEnumerable<Cliente>> GetActiveClientsAsync()
        {
            return await _dbSet
                .Include(c => c.Reservas)
                .Where(c => c.Activo)
                .OrderBy(c => c.Nombre)
                .ThenBy(c => c.Apellido)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsEmailAsync(string email, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Email == email && c.Activo);

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsDNIAsync(string dni, int? excludeId = null)
        {
            var query = _dbSet.Where(c => c.DNI == dni && c.Activo);

            if (excludeId.HasValue)
                query = query.Where(c => c.Id != excludeId.Value);

            return await query.AnyAsync();
        }

        public override async Task<Cliente?> GetByIdAsync(int id)
        {
            return await _dbSet
                .Include(c => c.Reservas)
                    .ThenInclude(r => r.Vehiculo)
                .FirstOrDefaultAsync(c => c.Id == id);
        }
    }
}