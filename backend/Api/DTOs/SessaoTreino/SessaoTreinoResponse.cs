namespace Api.DTOs.SessaoTreino;

public record SessaoTreinoResponse(
    Guid Id,
    DateOnly Data,
    List<SessaoTreinoComandoResponse> Comandos);
