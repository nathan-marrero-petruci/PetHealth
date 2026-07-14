using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.Antipulga;

public class AntipulgaRequest
{
    [Required]
    public string Produto { get; set; } = string.Empty;
    [Required]
    public DateOnly? DataAplicacao { get; set; }
    public DateOnly? DataProximaAplicacao { get; set; }
}
