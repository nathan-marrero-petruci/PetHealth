namespace Api.Models;

public class Vermifugacao
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public string Produto { get; set; } = string.Empty;
    public DateOnly DataAplicacao { get; set; }
    public DateOnly? DataProximaAplicacao { get; set; }
}
