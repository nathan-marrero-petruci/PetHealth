using System.ComponentModel.DataAnnotations;
using Api.Models;

namespace Api.DTOs.Pet;

public class PetRequest
{
    [Required]
    public string Nome { get; set; } = string.Empty;
    public string? Raca { get; set; }
    [Required]
    public PetPorte? Porte { get; set; }
    public DateOnly DataNascimento { get; set; }
    [Required]
    public PetSexo? Sexo { get; set; }
    public string? FotoUrl { get; set; }
    public decimal PesoReferencia { get; set; }
}
