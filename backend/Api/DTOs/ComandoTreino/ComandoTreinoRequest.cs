using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.ComandoTreino;

public class ComandoTreinoRequest
{
    [Required]
    public string Nome { get; set; } = string.Empty;
    public string? Descricao { get; set; }
}
