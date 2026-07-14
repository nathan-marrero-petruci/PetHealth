namespace Api.Models;

public class DietaPadrao
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public string NomeMarca { get; set; } = string.Empty;
    public decimal QuantidadeDiariaGramas { get; set; }
    public decimal QuantidadePorRefeicaoGramas { get; set; }
}
