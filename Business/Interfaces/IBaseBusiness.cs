using Entity.Dtos;
using Entity.Model;

namespace Business.Interfaces
{
    public interface IBaseBusiness<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        Task<IEnumerable<TDto>> GetAllAsync();
        Task<TDto?> GetByIdAsync(int id);
        Task<TDto> CreateAsync(TDto dto);
        Task<TDto> UpdateAsync(int id, TDto dto);
        Task<bool> DeleteAsync(int id);
        Task<bool> SoftDeleteAsync(int id);
        Task<IEnumerable<TDto>> GetActiveAsync();
        Task<bool> ExistsAsync(int id);
        Task<int> CountAsync();
        Task<IEnumerable<TDto>> GetPagedAsync(int page, int pageSize);
    }
}