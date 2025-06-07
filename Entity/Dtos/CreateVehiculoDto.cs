using System.ComponentModel.DataAnnotations;
using SistemaReservas.Entity.Enums;

namespace SistemaReservas.Entity.DTOs
{
    public class CreateVehiculoDto
    {
       
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public string Placa { get; set; }
        public TipoVehiculo Tipo { get; set; }
        public decimal PrecioPorDia { get; set; }
        public string Color { get; set; }
        public int Kilometraje { get; set; }
    }
}