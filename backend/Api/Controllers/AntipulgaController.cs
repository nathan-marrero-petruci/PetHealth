using System.Security.Claims;
using Api.Data;
using Api.DTOs.Antipulga;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AntipulgaModel = Api.Models.Antipulga;

namespace Api.Controllers;

[ApiController]
[Route("api/antipulgas")]
[Authorize]
public class AntipulgaController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar antipulgas." });
        }

        var antipulgas = await db.Antipulgas
            .Where(a => a.PetId == petId)
            .OrderByDescending(a => a.DataAplicacao)
            .ToListAsync();

        return Ok(antipulgas.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(AntipulgaRequest request)
    {
        if (request.DataProximaAplicacao is not null && request.DataProximaAplicacao < request.DataAplicacao)
        {
            return BadRequest(new { message = "Data da próxima aplicação não pode ser anterior à data de aplicação." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar antipulgas." });
        }

        var antipulga = new AntipulgaModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Produto = request.Produto,
            DataAplicacao = request.DataAplicacao!.Value,
            DataProximaAplicacao = request.DataProximaAplicacao
        };

        db.Antipulgas.Add(antipulga);
        await db.SaveChangesAsync();

        return Ok(ToResponse(antipulga));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, AntipulgaRequest request)
    {
        if (request.DataProximaAplicacao is not null && request.DataProximaAplicacao < request.DataAplicacao)
        {
            return BadRequest(new { message = "Data da próxima aplicação não pode ser anterior à data de aplicação." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar antipulgas." });
        }

        var antipulga = await db.Antipulgas.SingleOrDefaultAsync(a => a.Id == id && a.PetId == petId);

        if (antipulga is null)
        {
            return NotFound();
        }

        antipulga.Produto = request.Produto;
        antipulga.DataAplicacao = request.DataAplicacao!.Value;
        antipulga.DataProximaAplicacao = request.DataProximaAplicacao;

        await db.SaveChangesAsync();

        return Ok(ToResponse(antipulga));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar antipulgas." });
        }

        var antipulga = await db.Antipulgas.SingleOrDefaultAsync(a => a.Id == id && a.PetId == petId);

        if (antipulga is null)
        {
            return NotFound();
        }

        db.Antipulgas.Remove(antipulga);
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

    private static AntipulgaResponse ToResponse(AntipulgaModel antipulga) => new(
        antipulga.Id,
        antipulga.Produto,
        antipulga.DataAplicacao,
        antipulga.DataProximaAplicacao);
}
