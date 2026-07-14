namespace Api.Models;

public class Observacao
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateOnly Data { get; set; }
    public string Descricao { get; set; } = string.Empty;
}
