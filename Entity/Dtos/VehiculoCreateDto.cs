using System.ComponentModel.DataAnnotations;

public class VehiculoCreateDto
{
   
    public string Marca { get; set; } = string.Empty;

    public string Modelo { get; set; } = string.Empty;

    
    public int Año { get; set; }

    
    public string Placa { get; set; } = string.Empty;

    
    public string Color { get; set; } = string.Empty;

   
    public string Tipo { get; set; } = string.Empty;

    
    public decimal PrecioPorDia { get; set; }
}