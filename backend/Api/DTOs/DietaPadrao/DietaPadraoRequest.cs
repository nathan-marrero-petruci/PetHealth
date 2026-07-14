using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.DietaPadrao;

public class DietaPadraoRequest
{
    [Required]
    public string NomeMarca { get; set; } = string.Empty;
    public decimal? QuantidadeDiariaGramas { get; set; }
    public decimal? QuantidadePorRefeicaoGramas { get; set; }
}
