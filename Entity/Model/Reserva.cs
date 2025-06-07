namespace Entity.Model
{
    public class Reserva : BaseEntity
    {
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoTotal { get; set; }
        public EstadoReserva Estado { get; set; }
        public string Observaciones { get; set; }

        // Navegación
        public virtual Cliente Cliente { get; set; }
        public virtual Vehiculo Vehiculo { get; set; }
    }
}