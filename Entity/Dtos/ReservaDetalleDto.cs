using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class ReservaDetalleDto
    {
        public int Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public decimal MontoTotal { get; set; }
        public string Estado { get; set; } = string.Empty;
        public string? Observaciones { get; set; }

        // Cliente
        public string ClienteNombre { get; set; } = string.Empty;
        public string ClienteEmail { get; set; } = string.Empty;
        public string ClienteTelefono { get; set; } = string.Empty;

        // Vehículo
        public string VehiculoMarca { get; set; } = string.Empty;
        public string VehiculoModelo { get; set; } = string.Empty;
        public string VehiculoPlaca { get; set; } = string.Empty;
        public decimal VehiculoPrecioPorDia { get; set; }

        public int DiasDuracion => (FechaFin - FechaInicio).Days;
        public string VehiculoDescripcion => $"{VehiculoMarca} {VehiculoModelo} ({VehiculoPlaca})";
    }
}
