using System.Security.Claims;
using Api.Data;
using Api.DTOs.ComandoTreino;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComandoTreinoModel = Api.Models.ComandoTreino;

namespace Api.Controllers;

[ApiController]
[Route("api/comandos-treino")]
[Authorize]
public class ComandoTreinoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de cadastrar comandos de treino." });
        }

        var comandos = await db.ComandosTreino
            .Where(c => c.PetId == petId)
            .OrderBy(c => c.Nome)
            .ToListAsync();

        return Ok(comandos.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ComandoTreinoRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de cadastrar comandos de treino." });
        }

        var comando = new ComandoTreinoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Nome = request.Nome,
            Descricao = request.Descricao,
            Ativo = true
        };

        db.ComandosTreino.Add(comando);
        await db.SaveChangesAsync();

        return Ok(ToResponse(comando));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ComandoTreinoRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de cadastrar comandos de treino." });
        }

        var comando = await db.ComandosTreino.SingleOrDefaultAsync(c => c.Id == id && c.PetId == petId);

        if (comando is null)
        {
            return NotFound();
        }

        comando.Nome = request.Nome;
        comando.Descricao = request.Descricao;

        await db.SaveChangesAsync();

        return Ok(ToResponse(comando));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de cadastrar comandos de treino." });
        }

        var comando = await db.ComandosTreino.SingleOrDefaultAsync(c => c.Id == id && c.PetId == petId);

        if (comando is null)
        {
            return NotFound();
        }

        // Soft delete: comandos podem ser referenciados por sessões de treino (TRE-02), então
        // nunca são removidos fisicamente, apenas inativados.
        comando.Ativo = false;

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

    private static ComandoTreinoResponse ToResponse(ComandoTreinoModel comando) => new(
        comando.Id,
        comando.Nome,
        comando.Descricao,
        comando.Ativo);
}
