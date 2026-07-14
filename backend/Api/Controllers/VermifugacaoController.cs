using System.Security.Claims;
using Api.Data;
using Api.DTOs.Vermifugacao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VermifugacaoModel = Api.Models.Vermifugacao;

namespace Api.Controllers;

[ApiController]
[Route("api/vermifugacoes")]
[Authorize]
public class VermifugacaoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vermifugações." });
        }

        var vermifugacoes = await db.Vermifugacoes
            .Where(v => v.PetId == petId)
            .OrderByDescending(v => v.DataAplicacao)
            .ToListAsync();

        return Ok(vermifugacoes.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(VermifugacaoRequest request)
    {
        if (request.DataProximaAplicacao is not null && request.DataProximaAplicacao < request.DataAplicacao)
        {
            return BadRequest(new { message = "Data da próxima aplicação não pode ser anterior à data de aplicação." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vermifugações." });
        }

        var vermifugacao = new VermifugacaoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Produto = request.Produto,
            DataAplicacao = request.DataAplicacao!.Value,
            DataProximaAplicacao = request.DataProximaAplicacao
        };

        db.Vermifugacoes.Add(vermifugacao);
        await db.SaveChangesAsync();

        return Ok(ToResponse(vermifugacao));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, VermifugacaoRequest request)
    {
        if (request.DataProximaAplicacao is not null && request.DataProximaAplicacao < request.DataAplicacao)
        {
            return BadRequest(new { message = "Data da próxima aplicação não pode ser anterior à data de aplicação." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vermifugações." });
        }

        var vermifugacao = await db.Vermifugacoes.SingleOrDefaultAsync(v => v.Id == id && v.PetId == petId);

        if (vermifugacao is null)
        {
            return NotFound();
        }

        vermifugacao.Produto = request.Produto;
        vermifugacao.DataAplicacao = request.DataAplicacao!.Value;
        vermifugacao.DataProximaAplicacao = request.DataProximaAplicacao;

        await db.SaveChangesAsync();

        return Ok(ToResponse(vermifugacao));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vermifugações." });
        }

        var vermifugacao = await db.Vermifugacoes.SingleOrDefaultAsync(v => v.Id == id && v.PetId == petId);

        if (vermifugacao is null)
        {
            return NotFound();
        }

        db.Vermifugacoes.Remove(vermifugacao);
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

    private static VermifugacaoResponse ToResponse(VermifugacaoModel vermifugacao) => new(
        vermifugacao.Id,
        vermifugacao.Produto,
        vermifugacao.DataAplicacao,
        vermifugacao.DataProximaAplicacao);
}
