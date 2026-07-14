namespace Api.DTOs.DietaPadrao;

public record DietaPadraoResponse(
    Guid Id,
    string NomeMarca,
    decimal QuantidadeDiariaGramas,
    decimal QuantidadePorRefeicaoGramas);
