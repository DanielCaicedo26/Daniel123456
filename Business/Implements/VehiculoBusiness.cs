using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Model;
using FluentValidation;
using Utilities.Exceptions;

namespace Business.Implements
{
    public class VehiculoBusiness : BaseBusiness<Vehiculo, VehiculoDto>, IVehiculoBusiness
    {
        private readonly IVehiculoData _vehiculoData;
        private readonly IValidator<VehiculoCreateDto> _createValidator;
        private readonly IValidator<VehiculoUpdateDto> _updateValidator;

        public VehiculoBusiness(
            IVehiculoData vehiculoData,
            IMapper mapper,
            IValidator<VehiculoCreateDto> createValidator,
            IValidator<VehiculoUpdateDto> updateValidator)
            : base(vehiculoData, mapper)
        {
            _vehiculoData = vehiculoData;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<VehiculoDto>> GetAvailableVehiclesAsync()
        {
            try
            {
                var vehiculos = await _vehiculoData.GetAvailableVehiclesAsync();
                return _mapper.Map<IEnumerable<VehiculoDto>>(vehiculos);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener vehículos disponibles: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<VehiculoDto>> GetVehiclesByTypeAsync(string tipo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tipo))
                    throw new ValidationException("El tipo de vehículo no puede estar vacío");

                var vehiculos = await _vehiculoData.GetVehiclesByTypeAsync(tipo);
                return _mapper.Map<IEnumerable<VehiculoDto>>(vehiculos);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener vehículos por tipo: {ex.Message}", ex);
            }
        }

        public async Task<VehiculoDto?> GetByPlacaAsync(string placa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(placa))
                    throw new ValidationException("La placa no puede estar vacía");

                var vehiculo = await _vehiculoData.GetByPlacaAsync(placa);
                return vehiculo != null ? _mapper.Map<VehiculoDto>(vehiculo) : null;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al buscar vehículo por placa: {ex.Message}", ex);
            }
        }

        public async Task<VehiculoDto> CreateVehiculoAsync(VehiculoCreateDto dto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                if (await _vehiculoData.ExistsPlacaAsync(dto.Placa))
                    throw new BusinessRuleViolationException("PLACA_EXISTS", "Ya existe un vehículo con esta placa");

                var vehiculo = _mapper.Map<Vehiculo>(dto);
                var createdVehiculo = await _vehiculoData.CreateAsync(vehiculo);
                return _mapper.Map<VehiculoDto>(createdVehiculo);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (BusinessRuleViolationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al crear vehículo: {ex.Message}", ex);
            }
        }

        public async Task<VehiculoDto> UpdateVehiculoAsync(int id, VehiculoUpdateDto dto)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                var existingVehiculo = await _vehiculoData.GetByIdAsync(id);
                if (existingVehiculo == null)
                    throw new EntityNotFoundException("Vehiculo", id);

                _mapper.Map(dto, existingVehiculo);
                var updatedVehiculo = await _vehiculoData.UpdateAsync(existingVehiculo);
                return _mapper.Map<VehiculoDto>(updatedVehiculo);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al actualizar vehículo: {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidateUniquePlacaAsync(string placa, int? excludeId = null)
        {
            try
            {
                return !await _vehiculoData.ExistsPlacaAsync(placa, excludeId);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al validar unicidad de la placa: {ex.Message}", ex);
            }
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                if (fechaInicio >= fechaFin)
                    throw new ValidationException("La fecha de inicio debe ser anterior a la fecha de fin");

                if (fechaInicio < DateTime.Today)
                    throw new ValidationException("La fecha de inicio no puede ser anterior a hoy");

                return await _vehiculoData.IsVehicleAvailableAsync(vehiculoId, fechaInicio, fechaFin);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al verificar disponibilidad del vehículo: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<VehiculoDto>> GetVehiclesByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            try
            {
                if (minPrice < 0 || maxPrice < 0)
                    throw new ValidationException("Los precios no pueden ser negativos");

                if (minPrice > maxPrice)
                    throw new ValidationException("El precio mínimo no puede ser mayor al precio máximo");

                var vehiculos = await _vehiculoData.GetVehiclesByPriceRangeAsync(minPrice, maxPrice);
                return _mapper.Map<IEnumerable<VehiculoDto>>(vehiculos);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener vehículos por rango de precio: {ex.Message}", ex);
            }
        }

        public async Task<bool> SetAvailabilityAsync(int id, bool available)
        {
            try
            {
                var vehiculo = await _vehiculoData.GetByIdAsync(id);
                if (vehiculo == null)
                    throw new EntityNotFoundException("Vehiculo", id);

                vehiculo.EstaDisponible = available;
                await _vehiculoData.UpdateAsync(vehiculo);
                return true;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al cambiar disponibilidad del vehículo: {ex.Message}", ex);
            }
        }
    }
}