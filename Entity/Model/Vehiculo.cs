using Entity.Model;

public class Vehiculo : BaseEntity
{
    public string Marca { get; set; } = string.Empty;
    public string Modelo { get; set; } = string.Empty;
    public int Año { get; set; }
    public string Placa { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty; // Sedan, SUV, Hatchback, etc.
    public decimal PrecioPorDia { get; set; }
    public bool EstaDisponible { get; set; } = true;

    // Navegación
    public virtual ICollection<Reserva> Reservas { get; set; } = new List<Reserva>();
}