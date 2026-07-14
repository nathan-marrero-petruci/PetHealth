using System.Security.Claims;
using Api.Data;
using Api.DTOs.Medicacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MedicacaoModel = Api.Models.Medicacao;

namespace Api.Controllers;

[ApiController]
[Route("api/medicacoes")]
[Authorize]
public class MedicacaoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar medicações." });
        }

        var medicacoes = await db.Medicacoes
            .Where(m => m.PetId == petId)
            .OrderByDescending(m => m.DataInicio)
            .ToListAsync();

        return Ok(medicacoes.Select(ToResponse));
    }

    private const decimal DosagemValorMaximo = 999.99m;

    [HttpPost]
    public async Task<IActionResult> Create(MedicacaoRequest request)
    {
        var validationError = Validate(request);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar medicações." });
        }

        var medicacao = new MedicacaoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Nome = request.Nome,
            DosagemValor = Math.Round(request.DosagemValor!.Value, 2),
            DosagemUnidade = request.DosagemUnidade,
            VezesPorDia = request.VezesPorDia!.Value,
            IntervaloHoras = request.IntervaloHoras!.Value,
            DataInicio = request.DataInicio!.Value,
            DataTermino = request.DataTermino
        };

        db.Medicacoes.Add(medicacao);
        await db.SaveChangesAsync();

        return Ok(ToResponse(medicacao));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, MedicacaoRequest request)
    {
        var validationError = Validate(request);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar medicações." });
        }

        var medicacao = await db.Medicacoes.SingleOrDefaultAsync(m => m.Id == id && m.PetId == petId);

        if (medicacao is null)
        {
            return NotFound();
        }

        medicacao.Nome = request.Nome;
        medicacao.DosagemValor = Math.Round(request.DosagemValor!.Value, 2);
        medicacao.DosagemUnidade = request.DosagemUnidade;
        medicacao.VezesPorDia = request.VezesPorDia!.Value;
        medicacao.IntervaloHoras = request.IntervaloHoras!.Value;
        medicacao.DataInicio = request.DataInicio!.Value;
        medicacao.DataTermino = request.DataTermino;

        await db.SaveChangesAsync();

        return Ok(ToResponse(medicacao));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar medicações." });
        }

        var medicacao = await db.Medicacoes.SingleOrDefaultAsync(m => m.Id == id && m.PetId == petId);

        if (medicacao is null)
        {
            return NotFound();
        }

        db.Medicacoes.Remove(medicacao);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static string? Validate(MedicacaoRequest request)
    {
        if (request.DataTermino is not null && request.DataTermino < request.DataInicio)
        {
            return "Data de término não pode ser anterior à data de início.";
        }

        if (request.DosagemValor is <= 0)
        {
            return "Valor da dosagem deve ser maior que zero.";
        }

        if (request.DosagemValor > DosagemValorMaximo)
        {
            return $"Valor da dosagem não pode ser maior que {DosagemValorMaximo}.";
        }

        if (string.IsNullOrWhiteSpace(request.DosagemUnidade))
        {
            return "Unidade da dosagem é obrigatória.";
        }

        if (request.VezesPorDia is <= 0)
        {
            return "Quantidade de vezes por dia deve ser maior que zero.";
        }

        if (request.IntervaloHoras is <= 0)
        {
            return "Intervalo entre doses deve ser maior que zero.";
        }

        return null;
    }

    private Guid GetTutorId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<Guid?> GetPetIdAsync()
    {
        var tutorId = GetTutorId();
        var pet = await db.Pets.SingleOrDefaultAsync(p => p.TutorId == tutorId);

        return pet?.Id;
    }

    private static MedicacaoResponse ToResponse(MedicacaoModel medicacao) => new(
        medicacao.Id,
        medicacao.Nome,
        medicacao.DosagemValor,
        medicacao.DosagemUnidade,
        medicacao.VezesPorDia,
        medicacao.IntervaloHoras,
        medicacao.DataInicio,
        medicacao.DataTermino,
        GetStatus(medicacao));

    private static MedicacaoStatus GetStatus(MedicacaoModel medicacao)
    {
        if (medicacao.DataTermino is null)
        {
            return MedicacaoStatus.EmUso;
        }

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);

        return medicacao.DataTermino.Value < hoje ? MedicacaoStatus.Encerrada : MedicacaoStatus.EmUso;
    }
}
