using Api.Models;

namespace Api.DTOs.SessaoTreino;

public record SessaoTreinoComandoResponse(
    Guid ComandoTreinoId,
    string ComandoTreinoNome,
    NivelSucesso NivelSucesso);
