namespace Api.Models;

public class ItemForaDieta
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateTime DataHora { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public string? QuantidadeObservacao { get; set; }
}
