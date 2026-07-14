namespace Api.Models;

public class Refeicao
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateTime DataHora { get; set; }
    public decimal QuantidadeGramas { get; set; }
}
