namespace Api.Models;

public class ConsultaVeterinaria
{
    public Guid Id { get; set; }
    public Guid PetId { get; set; }
    public Pet? Pet { get; set; }
    public DateOnly Data { get; set; }
    public string Motivo { get; set; } = string.Empty;
    public string? VeterinarioClinica { get; set; }
    public string? Observacoes { get; set; }
}
