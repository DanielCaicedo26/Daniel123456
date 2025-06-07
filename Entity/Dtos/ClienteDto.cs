namespace Entity.Dtos
{
    public class ClienteDto : BaseDto
    {
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string NombreCompleto => $"{Nombre} {Apellido}";
        public string Email { get; set; }
        public string Telefono { get; set; }
        public string DocumentoIdentidad { get; set; }
        public DateTime FechaNacimiento { get; set; }
        public int Edad => DateTime.Now.Year - FechaNacimiento.Year;
        public List<ReservaDto> Reservas { get; set; } = new List<ReservaDto>();
    }
}