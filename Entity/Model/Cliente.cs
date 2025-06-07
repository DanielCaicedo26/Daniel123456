namespace Entity.Model
{
    public class Cliente : BaseEntity
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string DocumentoIdentidad { get; set; }
        public DateTime FechaNacimiento { get; set; }

        // Navegación
        public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
    }
}