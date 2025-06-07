namespace Entity.Dtos
{
    public abstract class BaseDto
    {
        public int Id { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool Activo { get; set; }
    }
}