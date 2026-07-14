namespace Api.DTOs.Vermifugacao;

public record VermifugacaoResponse(
    Guid Id,
    string Produto,
    DateOnly DataAplicacao,
    DateOnly? DataProximaAplicacao);
