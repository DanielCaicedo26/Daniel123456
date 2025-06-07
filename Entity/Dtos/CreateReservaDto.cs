using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class CreateReservaDto
    {
       
        public int ClienteId { get; set; }
        public int VehiculoId { get; set; }  
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }   
        public string Observaciones { get; set; }
    }
}
