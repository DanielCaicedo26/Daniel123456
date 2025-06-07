namespace Entity.Model
{
    public class Vehiculo : BaseEntity
    {
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public string Placa { get; set; }
        public TipoVehiculo Tipo { get; set; }
        public decimal PrecioPorDia { get; set; }
        public bool Disponible { get; set; } = true;
        public string Color { get; set; }
        public int Kilometraje { get; set; }

        // Navegación
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}