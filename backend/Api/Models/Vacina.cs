namespace Api.Models;

public class Vacina
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateOnly DataAplicacao { get; set; }
    public DateOnly? DataProximaDose { get; set; }
}
