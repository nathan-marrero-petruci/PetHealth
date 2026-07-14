namespace Api.Models;

public class Medicacao
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public string Nome { get; set; } = string.Empty;
    public decimal DosagemValor { get; set; }
    public string DosagemUnidade { get; set; } = string.Empty;
    public int VezesPorDia { get; set; }
    public int IntervaloHoras { get; set; }
    public DateOnly DataInicio { get; set; }
    public DateOnly? DataTermino { get; set; }
}
