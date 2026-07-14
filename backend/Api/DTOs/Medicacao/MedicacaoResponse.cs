namespace Api.DTOs.Medicacao;

public record MedicacaoResponse(
    Guid Id,
    string Nome,
    decimal DosagemValor,
    string DosagemUnidade,
    int VezesPorDia,
    int IntervaloHoras,
    DateOnly DataInicio,
    DateOnly? DataTermino,
    MedicacaoStatus Status);
