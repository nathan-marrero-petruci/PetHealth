namespace Api.DTOs.ComandoTreino;

public record ComandoTreinoResponse(
    Guid Id,
    string Nome,
    string? Descricao,
    bool Ativo);
