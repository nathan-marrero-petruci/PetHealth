using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Medicacao;

public class MedicacaoRequest
{
    [Required]
    public string Nome { get; set; } = string.Empty;
    [Required]
    public decimal? DosagemValor { get; set; }
    [Required]
    public string DosagemUnidade { get; set; } = string.Empty;
    [Required]
    public int? VezesPorDia { get; set; }
    [Required]
    public int? IntervaloHoras { get; set; }
    [Required]
    public DateOnly? DataInicio { get; set; }
    public DateOnly? DataTermino { get; set; }
}
