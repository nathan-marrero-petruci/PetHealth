namespace Api.DTOs.Antipulga;

public record AntipulgaResponse(
    Guid Id,
    string Produto,
    DateOnly DataAplicacao,
    DateOnly? DataProximaAplicacao);
