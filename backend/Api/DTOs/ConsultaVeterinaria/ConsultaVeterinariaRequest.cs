using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.ConsultaVeterinaria;

public class ConsultaVeterinariaRequest
{
    [Required]
    public DateOnly? Data { get; set; }
    [Required]
    public string Motivo { get; set; } = string.Empty;
    public string? VeterinarioClinica { get; set; }
    public string? Observacoes { get; set; }
}
