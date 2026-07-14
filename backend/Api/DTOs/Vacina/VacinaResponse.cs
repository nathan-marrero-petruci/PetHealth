namespace Api.DTOs.Vacina;

public record VacinaResponse(
    Guid Id,
    string Nome,
    DateOnly DataAplicacao,
    DateOnly? DataProximaDose);
