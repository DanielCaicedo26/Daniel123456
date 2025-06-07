using Entity.Model;

public class Reserva : BaseEntity
{
    public int ClienteId { get; set; }
    public int VehiculoId { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public decimal MontoTotal { get; set; }
    public string Estado { get; set; } = "Pendiente"; // Pendiente, Confirmada, EnCurso, Completada, Cancelada
    public string? Observaciones { get; set; }

    // Navegación
    public virtual Cliente Cliente { get; set; } = null!;
    public virtual Vehiculo Vehiculo { get; set; } = null!;
}