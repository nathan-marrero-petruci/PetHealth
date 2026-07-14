using System.Security.Claims;
using Api.Data;
using Api.DTOs.Observacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ObservacaoModel = Api.Models.Observacao;

namespace Api.Controllers;

[ApiController]
[Route("api/observacoes")]
[Authorize]
public class ObservacaoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar observações." });
        }

        var observacoes = await db.Observacoes
            .Where(o => o.PetId == petId)
            .OrderByDescending(o => o.Data)
            .ToListAsync();

        return Ok(observacoes.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ObservacaoRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar observações." });
        }

        var observacao = new ObservacaoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Data = request.Data!.Value,
            Descricao = request.Descricao
        };

        db.Observacoes.Add(observacao);
        await db.SaveChangesAsync();

        return Ok(ToResponse(observacao));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ObservacaoRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar observações." });
        }

        var observacao = await db.Observacoes.SingleOrDefaultAsync(o => o.Id == id && o.PetId == petId);

        if (observacao is null)
        {
            return NotFound();
        }

        observacao.Data = request.Data!.Value;
        observacao.Descricao = request.Descricao;

        await db.SaveChangesAsync();

        return Ok(ToResponse(observacao));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar observações." });
        }

        var observacao = await db.Observacoes.SingleOrDefaultAsync(o => o.Id == id && o.PetId == petId);

        if (observacao is null)
        {
            return NotFound();
        }

        db.Observacoes.Remove(observacao);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private Guid GetTutorId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<Guid?> GetPetIdAsync()
    {
        var tutorId = GetTutorId();
        var pet = await db.Pets.SingleOrDefaultAsync(p => p.TutorId == tutorId);

        return pet?.Id;
    }

    private static ObservacaoResponse ToResponse(ObservacaoModel observacao) => new(
        observacao.Id,
        observacao.Data,
        observacao.Descricao);
}
