using Api.Models;

namespace Api.DTOs.Pet;

public record PetResponse(
    Guid Id,
    string Nome,
    string? Raca,
    PetPorte Porte,
    DateOnly DataNascimento,
    PetSexo Sexo,
    string? FotoUrl,
    decimal PesoReferencia);
