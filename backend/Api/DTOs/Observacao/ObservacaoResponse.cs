namespace Api.DTOs.Observacao;

public record ObservacaoResponse(
    Guid Id,
    DateOnly Data,
    string Descricao);
