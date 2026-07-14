using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Vacina;

public class VacinaRequest
{
    [Required]
    public string Nome { get; set; } = string.Empty;
    [Required]
    public DateOnly? DataAplicacao { get; set; }
    public DateOnly? DataProximaDose { get; set; }
}
