namespace Api.DTOs.ItemForaDieta;

public record ItemForaDietaResponse(
    Guid Id,
    DateTime DataHora,
    string Descricao,
    string? QuantidadeObservacao);
