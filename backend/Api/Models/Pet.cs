namespace Api.Models;

public enum PetPorte
{
    Pequeno,
    Medio,
    Grande
}

public enum PetSexo
{
    Macho,
    Femea
}

public class Pet
{
    public Guid Id { get; set; }
    public Guid TutorId { get; set; }
    public Tutor? Tutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public PetPorte Porte { get; set; }
    public DateOnly DataNascimento { get; set; }
    public PetSexo Sexo { get; set; }
    public string? FotoUrl { get; set; }
    public decimal PesoReferencia { get; set; }
}
