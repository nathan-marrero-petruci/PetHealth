namespace Api.DTOs.ConsultaVeterinaria;

public record ConsultaVeterinariaResponse(
    Guid Id,
    DateOnly Data,
    string Motivo,
    string? VeterinarioClinica,
    string? Observacoes);
