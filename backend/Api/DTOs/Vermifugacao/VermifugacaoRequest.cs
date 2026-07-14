using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Vermifugacao;

public class VermifugacaoRequest
{
    [Required]
    public string Produto { get; set; } = string.Empty;
    [Required]
    public DateOnly? DataAplicacao { get; set; }
    public DateOnly? DataProximaAplicacao { get; set; }
}
