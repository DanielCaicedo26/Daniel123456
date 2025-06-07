using Data.Interfaces;
using Entity.Context;
using Entity.Model;
using Microsoft.EntityFrameworkCore;

namespace Data.Implements
{
    public class BaseModelData<T> : IBaseModelData<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public BaseModelData(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<T> CreateAsync(T entity)
        {
            entity.FechaCreacion = DateTime.Now;
            entity.Activo = true;

            await _dbSet.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            entity.FechaModificacion = DateTime.Now;

            _dbSet.Update(entity);
            await _context.SaveChangesAsync();
            return entity;
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<bool> SoftDeleteAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            entity.Activo = false;
            entity.FechaModificacion = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }

        public virtual async Task<IEnumerable<T>> GetActiveAsync()
        {
            return await _dbSet.Where(x => x.Activo).AsNoTracking().ToListAsync();
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            return await _dbSet.AnyAsync(x => x.Id == id);
        }

        public virtual async Task<int> CountAsync()
        {
            return await _dbSet.CountAsync();
        }

        public virtual async Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize)
        {
            return await _dbSet
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}