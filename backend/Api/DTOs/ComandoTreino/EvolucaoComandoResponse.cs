using Api.Models;

namespace Api.DTOs.ComandoTreino;

public record EvolucaoComandoResponse(
    DateOnly Data,
    NivelSucesso NivelSucesso);
