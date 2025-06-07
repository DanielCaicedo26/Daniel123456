using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class VehiculoUpdateDto
    {
        public string? Marca { get; set; }
        public string? Modelo { get; set; }
        public int? Año { get; set; }
        public string? Color { get; set; }
        public string? Tipo { get; set; }
        public decimal? PrecioPorDia { get; set; }
        public bool? EstaDisponible { get; set; }
    }
}
