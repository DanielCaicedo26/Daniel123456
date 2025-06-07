using System.ComponentModel.DataAnnotations;

public class VehiculoCreateDto
{
    [Required]
    [StringLength(50)]
    public string Marca { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Modelo { get; set; } = string.Empty;

    [Required]
    [Range(1900, 2030)]
    public int Año { get; set; }

    [Required]
    [StringLength(10)]
    public string Placa { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Color { get; set; } = string.Empty;

    [Required]
    [StringLength(30)]
    public string Tipo { get; set; } = string.Empty;

    [Required]
    [Range(0.01, 9999.99)]
    public decimal PrecioPorDia { get; set; }
}