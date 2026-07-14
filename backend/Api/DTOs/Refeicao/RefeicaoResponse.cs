namespace Api.DTOs.Refeicao;

// DiferencaGramas = QuantidadeGramas - QuantidadePorRefeicaoGramas da dieta padrão do pet.
// Positivo: serviu mais que o esperado. Negativo: serviu menos. Null: não há dieta padrão cadastrada.
public record RefeicaoResponse(
    Guid Id,
    DateTime DataHora,
    decimal QuantidadeGramas,
    decimal? DiferencaGramas);
