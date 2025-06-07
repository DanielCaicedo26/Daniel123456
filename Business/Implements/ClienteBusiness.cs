using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Model;
using FluentValidation;


namespace Business.Implements
{
    public class ClienteBusiness : BaseBusiness<Cliente, ClienteDto>, IClienteBusiness
    {
        private readonly IClienteData _clienteData;
        private readonly IValidator<ClienteCreateDto> _createValidator;
        private readonly IValidator<ClienteUpdateDto> _updateValidator;

        public ClienteBusiness(
            IClienteData clienteData,
            IMapper mapper,
            IValidator<ClienteCreateDto> createValidator,
            IValidator<ClienteUpdateDto> updateValidator)
            : base(clienteData, mapper)
        {
            _clienteData = clienteData;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<ClienteDto?> GetByEmailAsync(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    throw new ValidationException("El email no puede estar vacío");

                var cliente = await _clienteData.GetByEmailAsync(email);
                return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al buscar cliente por email: {ex.Message}", ex);
            }
        }

        public async Task<ClienteDto?> GetByDNIAsync(string dni)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dni))
                    throw new ValidationException("El DNI no puede estar vacío");

                var cliente = await _clienteData.GetByDNIAsync(dni);
                return cliente != null ? _mapper.Map<ClienteDto>(cliente) : null;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al buscar cliente por DNI: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ClienteDto>> GetActiveClientsAsync()
        {
            try
            {
                var clientes = await _clienteData.GetActiveClientsAsync();
                return _mapper.Map<IEnumerable<ClienteDto>>(clientes);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener clientes activos: {ex.Message}", ex);
            }
        }

        public async Task<ClienteDto> CreateClienteAsync(ClienteCreateDto dto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                // Validar email único
                if (await _clienteData.ExistsEmailAsync(dto.Email))
                    throw new BusinessRuleViolationException("EMAIL_EXISTS", "Ya existe un cliente con este email");

                // Validar DNI único
                if (await _clienteData.ExistsDNIAsync(dto.DNI))
                    throw new BusinessRuleViolationException("DNI_EXISTS", "Ya existe un cliente con este DNI");

                // Validar edad mínima
                var edad = DateTime.Now.Year - dto.FechaNacimiento.Year;
                if (dto.FechaNacimiento > DateTime.Now.AddYears(-edad)) edad--;

                if (edad < 18)
                    throw new BusinessRuleViolationException("MINOR_AGE", "El cliente debe ser mayor de edad");

                var cliente = _mapper.Map<Cliente>(dto);
                var createdCliente = await _clienteData.CreateAsync(cliente);
                return _mapper.Map<ClienteDto>(createdCliente);
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
                throw new BusinessException($"Error al crear cliente: {ex.Message}", ex);
            }
        }

        public async Task<ClienteDto> UpdateClienteAsync(int id, ClienteUpdateDto dto)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                var existingCliente = await _clienteData.GetByIdAsync(id);
                if (existingCliente == null)
                    throw new EntityNotFoundException("Cliente", id);

                // Validar email único si se está actualizando
                if (!string.IsNullOrEmpty(dto.Email) && dto.Email != existingCliente.Email)
                {
                    if (await _clienteData.ExistsEmailAsync(dto.Email, id))
                        throw new BusinessRuleViolationException("EMAIL_EXISTS", "Ya existe un cliente con este email");
                }

                // Validar edad si se está actualizando fecha de nacimiento
                if (dto.FechaNacimiento.HasValue)
                {
                    var edad = DateTime.Now.Year - dto.FechaNacimiento.Value.Year;
                    if (dto.FechaNacimiento.Value > DateTime.Now.AddYears(-edad)) edad--;

                    if (edad < 18)
                        throw new BusinessRuleViolationException("MINOR_AGE", "El cliente debe ser mayor de edad");
                }

                _mapper.Map(dto, existingCliente);
                var updatedCliente = await _clienteData.UpdateAsync(existingCliente);
                return _mapper.Map<ClienteDto>(updatedCliente);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (BusinessRuleViolationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al actualizar cliente: {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidateUniqueEmailAsync(string email, int? excludeId = null)
        {
            try
            {
                return !await _clienteData.ExistsEmailAsync(email, excludeId);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al validar unicidad del email: {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidateUniqueDNIAsync(string dni, int? excludeId = null)
        {
            try
            {
                return !await _clienteData.ExistsDNIAsync(dni, excludeId);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al validar unicidad del DNI: {ex.Message}", ex);
            }
        }
    }
}