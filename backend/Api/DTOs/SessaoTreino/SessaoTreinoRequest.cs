using System.ComponentModel.DataAnnotations;

namespace Api.DTOs.SessaoTreino;

public class SessaoTreinoRequest
{
    [Required]
    public DateOnly? Data { get; set; }
    public List<SessaoTreinoComandoRequest> Comandos { get; set; } = [];
}
