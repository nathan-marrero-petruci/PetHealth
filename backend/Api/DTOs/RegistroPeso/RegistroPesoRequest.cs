using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.RegistroPeso;

public class RegistroPesoRequest
{
    public decimal Peso { get; set; }
    [Required]
    public DateOnly? Data { get; set; }
}
