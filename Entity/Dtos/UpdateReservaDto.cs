using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Dtos
{
    public class UpdateReservaDto
    {
        [Required(ErrorMessage = "El cliente es requerido")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "El vehículo es requerido")]
        public int VehiculoId { get; set; }

        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        public DateTime FechaInicio { get; set; }

        [Required(ErrorMessage = "La fecha de fin es requerida")]
        public DateTime FechaFin { get; set; }

        [Required(ErrorMessage = "El estado es requerido")]
        public EstadoReserva Estado { get; set; }

        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string Observaciones { get; set; }
    }
}
