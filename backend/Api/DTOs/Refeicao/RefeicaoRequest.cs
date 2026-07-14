using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Refeicao;

public class RefeicaoRequest
{
    [Required]
    public DateTime? DataHora { get; set; }
    [Required]
    public decimal? QuantidadeGramas { get; set; }
}
