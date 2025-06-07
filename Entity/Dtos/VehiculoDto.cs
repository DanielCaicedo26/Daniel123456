using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class VehiculoDto
    {
        public int Id { get; set; }
        public string Marca { get; set; } = string.Empty;
        public string Modelo { get; set; } = string.Empty;
        public int Año { get; set; }
        public string Placa { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal PrecioPorDia { get; set; }
        public bool EstaDisponible { get; set; }
        public bool EsActivo { get; set; }
        public string DescripcionCompleta => $"{Marca} {Modelo} {Año}";
    }
}
