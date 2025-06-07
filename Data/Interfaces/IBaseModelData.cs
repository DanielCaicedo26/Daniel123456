using Entity.Model; // Cambia el using para que apunte al namespace correcto donde está BaseEntity

namespace Data.Interfaces
{
    public interface IBaseModelData<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> CreateAsync(T entity);
        Task<T> UpdateAsync(T entity);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<T>> GetActiveAsync();
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task<IEnumerable<T>> GetPagedAsync(int page, int pageSize);
    }
}
