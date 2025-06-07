using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class ReservaDto
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }
        public bool EsActivo { get; set; }

        // Propiedades de navegación
        public ClienteDto? Cliente { get; set; }
        public VehiculoDto? Vehiculo { get; set; }

        // Propiedades calculadas
        public int DiasDuracion => (FechaFin - FechaInicio).Days;
        public bool EstaVencida => FechaFin < DateTime.Now;
    }
}
