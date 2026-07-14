using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.ItemForaDieta;

public class ItemForaDietaRequest
{
    [Required]
    public DateTime? DataHora { get; set; }
    [Required]
    public string Descricao { get; set; } = string.Empty;
    public string? QuantidadeObservacao { get; set; }
}
