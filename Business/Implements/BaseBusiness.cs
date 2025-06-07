using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Model;
using Utilities.Exceptions;

namespace Business.Implements
{
    public class BaseBusiness<TEntity, TDto> : IBaseBusiness<TEntity, TDto>
        where TEntity : BaseEntity
        where TDto : BaseDto
    {
        protected readonly IBaseModelData<TEntity> _data;
        protected readonly IMapper _mapper;

        public BaseBusiness(IBaseModelData<TEntity> data, IMapper mapper)
        {
            _data = data;
            _mapper = mapper;
        }

        public virtual async Task<IEnumerable<TDto>> GetAllAsync()
        {
            try
            {
                var entities = await _data.GetAllAsync();
                return _mapper.Map<IEnumerable<TDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener todos los registros: {ex.Message}", ex);
            }
        }

        public virtual async Task<TDto?> GetByIdAsync(int id)
        {
            try
            {
                var entity = await _data.GetByIdAsync(id);
                return entity != null ? _mapper.Map<TDto>(entity) : null;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener el registro con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<TDto> CreateAsync(TDto dto)
        {
            try
            {
                var entity = _mapper.Map<TEntity>(dto);
                var createdEntity = await _data.CreateAsync(entity);
                return _mapper.Map<TDto>(createdEntity);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al crear el registro: {ex.Message}", ex);
            }
        }

        public virtual async Task<TDto> UpdateAsync(int id, TDto dto)
        {
            try
            {
                var existingEntity = await _data.GetByIdAsync(id);
                if (existingEntity == null)
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);

                _mapper.Map(dto, existingEntity);
                var updatedEntity = await _data.UpdateAsync(existingEntity);
                return _mapper.Map<TDto>(updatedEntity);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al actualizar el registro con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                if (!await _data.ExistsAsync(id))
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);

                return await _data.DeleteAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al eliminar el registro con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<bool> SoftDeleteAsync(int id)
        {
            try
            {
                if (!await _data.ExistsAsync(id))
                    throw new EntityNotFoundException(typeof(TEntity).Name, id);

                return await _data.SoftDeleteAsync(id);
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al realizar eliminación lógica del registro con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<TDto>> GetActiveAsync()
        {
            try
            {
                var entities = await _data.GetActiveAsync();
                return _mapper.Map<IEnumerable<TDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener registros activos: {ex.Message}", ex);
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _data.ExistsAsync(id);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al verificar existencia del registro con ID {id}: {ex.Message}", ex);
            }
        }

        public virtual async Task<int> CountAsync()
        {
            try
            {
                return await _data.CountAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al contar registros: {ex.Message}", ex);
            }
        }

        public virtual async Task<IEnumerable<TDto>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                if (page <= 0) page = 1;
                if (pageSize <= 0) pageSize = 10;

                var entities = await _data.GetPagedAsync(page, pageSize);
                return _mapper.Map<IEnumerable<TDto>>(entities);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener registros paginados: {ex.Message}", ex);
            }
        }
    }
}