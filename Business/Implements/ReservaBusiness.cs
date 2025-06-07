using AutoMapper;
using Business.Interfaces;
using Data.Interfaces;
using Entity.Dtos;
using Entity.Model;
using FluentValidation;
using Utilities.Exceptions;

namespace Business.Implements
{
    public class ReservaBusiness : BaseBusiness<Reserva, ReservaDto>, IReservaBusiness
    {
        private readonly IReservaData _reservaData;
        private readonly IVehiculoData _vehiculoData;
        private readonly IClienteData _clienteData;
        private readonly IValidator<ReservaCreateDto> _createValidator;
        private readonly IValidator<ReservaUpdateDto> _updateValidator;

        public ReservaBusiness(
            IReservaData reservaData,
            IVehiculoData vehiculoData,
            IClienteData clienteData,
            IMapper mapper,
            IValidator<ReservaCreateDto> createValidator,
            IValidator<ReservaUpdateDto> updateValidator)
            : base(reservaData, mapper)
        {
            _reservaData = reservaData;
            _vehiculoData = vehiculoData;
            _clienteData = clienteData;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<ReservaDto>> GetReservasByClienteAsync(int clienteId)
        {
            try
            {
                var reservas = await _reservaData.GetReservasByClienteAsync(clienteId);
                return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener reservas del cliente: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetReservasByVehiculoAsync(int vehiculoId)
        {
            try
            {
                var reservas = await _reservaData.GetReservasByVehiculoAsync(vehiculoId);
                return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener reservas del vehículo: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetReservasByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate >= endDate)
                    throw new ValidationException("La fecha de inicio debe ser anterior a la fecha de fin");

                var reservas = await _reservaData.GetReservasByDateRangeAsync(startDate, endDate);
                return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener reservas por rango de fechas: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetActiveReservationsAsync()
        {
            try
            {
                var reservas = await _reservaData.GetActiveReservationsAsync();
                return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener reservas activas: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReservaDto>> GetReservasByEstadoAsync(string estado)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(estado))
                    throw new ValidationException("El estado no puede estar vacío");

                var reservas = await _reservaData.GetReservasByEstadoAsync(estado);
                return _mapper.Map<IEnumerable<ReservaDto>>(reservas);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener reservas por estado: {ex.Message}", ex);
            }
        }

        public async Task<ReservaDto> CreateReservaAsync(ReservaCreateDto dto)
        {
            try
            {
                var validationResult = await _createValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                // Validar que el cliente existe y está activo
                if (!await _clienteData.ExistsAsync(dto.ClienteId))
                    throw new EntityNotFoundException("Cliente", dto.ClienteId);

                // Validar que el vehículo existe y está disponible
                var vehiculo = await _vehiculoData.GetByIdAsync(dto.VehiculoId);
                if (vehiculo == null)
                    throw new EntityNotFoundException("Vehiculo", dto.VehiculoId);

                if (!vehiculo.EstaDisponible)
                    throw new BusinessRuleViolationException("VEHICLE_NOT_AVAILABLE", "El vehículo no está disponible");

                // Validar disponibilidad en las fechas solicitadas
                if (!await ValidateReservationDatesAsync(dto.VehiculoId, dto.FechaInicio, dto.FechaFin))
                    throw new BusinessRuleViolationException("DATE_CONFLICT", "El vehículo ya está reservado en las fechas seleccionadas");

                // Calcular monto total
                var montoTotal = await CalculateMontoTotalAsync(dto.VehiculoId, dto.FechaInicio, dto.FechaFin);

                var reserva = _mapper.Map<Reserva>(dto);
                reserva.MontoTotal = montoTotal;
                reserva.Estado = "Pendiente";

                var createdReserva = await _reservaData.CreateAsync(reserva);
                return _mapper.Map<ReservaDto>(createdReserva);
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
                throw new BusinessException($"Error al crear reserva: {ex.Message}", ex);
            }
        }

        public async Task<ReservaDto> UpdateReservaAsync(int id, ReservaUpdateDto dto)
        {
            try
            {
                var validationResult = await _updateValidator.ValidateAsync(dto);
                if (!validationResult.IsValid)
                    throw new ValidationException(string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage)));

                var existingReserva = await _reservaData.GetByIdAsync(id);
                if (existingReserva == null)
                    throw new EntityNotFoundException("Reserva", id);

                // Si se están cambiando las fechas, validar disponibilidad
                if (dto.FechaInicio.HasValue || dto.FechaFin.HasValue)
                {
                    var nuevaFechaInicio = dto.FechaInicio ?? existingReserva.FechaInicio;
                    var nuevaFechaFin = dto.FechaFin ?? existingReserva.FechaFin;

                    if (!await ValidateReservationDatesAsync(existingReserva.VehiculoId, nuevaFechaInicio, nuevaFechaFin, id))
                        throw new BusinessRuleViolationException("DATE_CONFLICT", "El vehículo ya está reservado en las fechas seleccionadas");

                    // Recalcular monto si cambian las fechas
                    var nuevoMonto = await CalculateMontoTotalAsync(existingReserva.VehiculoId, nuevaFechaInicio, nuevaFechaFin);
                    existingReserva.MontoTotal = nuevoMonto;
                }

                _mapper.Map(dto, existingReserva);
                var updatedReserva = await _reservaData.UpdateAsync(existingReserva);
                return _mapper.Map<ReservaDto>(updatedReserva);
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
                throw new BusinessException($"Error al actualizar reserva: {ex.Message}", ex);
            }
        }

        public async Task<ReservaDetalleDto?> GetReservaDetalleAsync(int id)
        {
            try
            {
                return await _reservaData.GetReservaDetalleAsync(id);
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener detalle de la reserva: {ex.Message}", ex);
            }
        }

        public async Task<IEnumerable<ReservaDetalleDto>> GetReservasDetalleAsync()
        {
            try
            {
                return await _reservaData.GetReservasDetalleAsync();
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al obtener reservas detalladas: {ex.Message}", ex);
            }
        }

        public async Task<ReservaDto> ChangeEstadoAsync(int id, string nuevoEstado)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nuevoEstado))
                    throw new ValidationException("El nuevo estado no puede estar vacío");

                var validStates = new[] { "Pendiente", "Confirmada", "EnCurso", "Completada", "Cancelada" };
                if (!validStates.Contains(nuevoEstado))
                    throw new ValidationException($"Estado no válido. Estados permitidos: {string.Join(", ", validStates)}");

                var reserva = await _reservaData.GetByIdAsync(id);
                if (reserva == null)
                    throw new EntityNotFoundException("Reserva", id);

                // Validar transiciones de estado válidas
                if (!IsValidStateTransition(reserva.Estado, nuevoEstado))
                    throw new BusinessRuleViolationException("INVALID_STATE_TRANSITION",
                        $"No se puede cambiar de estado '{reserva.Estado}' a '{nuevoEstado}'");

                reserva.Estado = nuevoEstado;
                var updatedReserva = await _reservaData.UpdateAsync(reserva);
                return _mapper.Map<ReservaDto>(updatedReserva);
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
                throw new BusinessException($"Error al cambiar estado de la reserva: {ex.Message}", ex);
            }
        }

        public async Task<bool> ValidateReservationDatesAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin, int? excludeReservaId = null)
        {
            try
            {
                if (fechaInicio >= fechaFin)
                    throw new ValidationException("La fecha de inicio debe ser anterior a la fecha de fin");

                if (fechaInicio < DateTime.Today)
                    throw new ValidationException("La fecha de inicio no puede ser anterior a hoy");

                return !await _reservaData.HasConflictingReservationAsync(vehiculoId, fechaInicio, fechaFin, excludeReservaId);
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al validar fechas de reserva: {ex.Message}", ex);
            }
        }

        public async Task<decimal> CalculateMontoTotalAsync(int vehiculoId, DateTime fechaInicio, DateTime fechaFin)
        {
            try
            {
                var vehiculo = await _vehiculoData.GetByIdAsync(vehiculoId);
                if (vehiculo == null)
                    throw new EntityNotFoundException("Vehiculo", vehiculoId);

                var dias = (fechaFin - fechaInicio).Days;
                if (dias <= 0)
                    throw new ValidationException("El número de días debe ser mayor a cero");

                return vehiculo.PrecioPorDia * dias;
            }
            catch (EntityNotFoundException)
            {
                throw;
            }
            catch (ValidationException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new BusinessException($"Error al calcular monto total: {ex.Message}", ex);
            }
        }

        public async Task<bool> CancelReservaAsync(int id, string motivo = "")
        {
            try
            {
                var reserva = await _reservaData.GetByIdAsync(id);
                if (reserva == null)
                    throw new EntityNotFoundException("Reserva", id);

                if (reserva.Estado == "Cancelada")
                    throw new BusinessRuleViolationException("ALREADY_CANCELLED", "La reserva ya está cancelada");

                if (reserva.Estado == "Completada")
                    throw new BusinessRuleViolationException("CANNOT_CANCEL_COMPLETED", "No se puede cancelar una reserva completada");

                reserva.Estado = "Cancelada";
                if (!string.IsNullOrEmpty(motivo))
                {
                    reserva.Observaciones = string.IsNullOrEmpty(reserva.Observaciones)
                        ? $"Cancelada: {motivo}"
                        : $"{reserva.Observaciones} | Cancelada: {motivo}";
                }

                await _reservaData.UpdateAsync(reserva);
                return true;
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
                throw new BusinessException($"Error al cancelar reserva: {ex.Message}", ex);
            }
        }

        private bool IsValidStateTransition(string currentState, string newState)
        {
            var validTransitions = new Dictionary<string, string[]>
            {
                ["Pendiente"] = new[] { "Confirmada", "Cancelada" },
                ["Confirmada"] = new[] { "EnCurso", "Cancelada" },
                ["EnCurso"] = new[] { "Completada", "Cancelada" },
                ["Completada"] = new string[] { }, // No se puede cambiar desde completada
                ["Cancelada"] = new string[] { } // No se puede cambiar desde cancelada
            };

            return validTransitions.ContainsKey(currentState) &&
                   validTransitions[currentState].Contains(newState);
        }
    }
}