using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.DTOs.SessaoTreino;

public class SessaoTreinoComandoRequest
{
    [Required]
    public Guid? ComandoTreinoId { get; set; }
    [Required]
    public NivelSucesso? NivelSucesso { get; set; }
}
