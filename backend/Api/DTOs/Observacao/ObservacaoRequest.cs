using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Observacao;

public class ObservacaoRequest
{
    [Required]
    public DateOnly? Data { get; set; }
    [Required]
    public string Descricao { get; set; } = string.Empty;
}
