namespace Api.Models;

public class RegistroPeso
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public decimal Peso { get; set; }
    public DateOnly Data { get; set; }
}
