namespace Api.DTOs.RegistroPeso;

public record RegistroPesoResponse(
    Guid Id,
    decimal Peso,
    DateOnly Data);
