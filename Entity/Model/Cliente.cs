using Entity.Model;

public class Cliente : BaseEntity
{
    public string Nombre { get; set; } = string.Empty;
    public string Apellido { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string DNI { get; set; } = string.Empty;
    public DateTime FechaNacimiento { get; set; }

    // Navegación
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}