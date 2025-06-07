using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class UpdateVehiculoDto
    {
        
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public int Año { get; set; }
        public string Placa { get; set; }
        public TipoVehiculo Tipo { get; set; }
        public decimal PrecioPorDia { get; set; }
        public bool Disponible { get; set; }
        public string Color { get; set; }
        public int Kilometraje { get; set; }
    }
}
}
