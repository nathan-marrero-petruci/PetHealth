using System.Security.Claims;
using Api.Data;
using Api.DTOs.Vacina;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VacinaModel = Api.Models.Vacina;

namespace Api.Controllers;

[ApiController]
[Route("api/vacinas")]
[Authorize]
public class VacinaController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vacinas." });
        }

        var vacinas = await db.Vacinas
            .Where(v => v.PetId == petId)
            .OrderByDescending(v => v.DataAplicacao)
            .ToListAsync();

        return Ok(vacinas.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(VacinaRequest request)
    {
        if (request.DataProximaDose is not null && request.DataProximaDose < request.DataAplicacao)
        {
            return BadRequest(new { message = "Data da próxima dose não pode ser anterior à data de aplicação." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vacinas." });
        }

        var vacina = new VacinaModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Nome = request.Nome,
            DataAplicacao = request.DataAplicacao!.Value,
            DataProximaDose = request.DataProximaDose,
            AntecedenciaLembreteDias = request.AntecedenciaLembreteDias!.Value
        };

        db.Vacinas.Add(vacina);
        await db.SaveChangesAsync();

        return Ok(ToResponse(vacina));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, VacinaRequest request)
    {
        if (request.DataProximaDose is not null && request.DataProximaDose < request.DataAplicacao)
        {
            return BadRequest(new { message = "Data da próxima dose não pode ser anterior à data de aplicação." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vacinas." });
        }

        var vacina = await db.Vacinas.SingleOrDefaultAsync(v => v.Id == id && v.PetId == petId);

        if (vacina is null)
        {
            return NotFound();
        }

        vacina.Nome = request.Nome;
        vacina.DataAplicacao = request.DataAplicacao!.Value;
        vacina.DataProximaDose = request.DataProximaDose;
        vacina.AntecedenciaLembreteDias = request.AntecedenciaLembreteDias!.Value;

        await db.SaveChangesAsync();

        return Ok(ToResponse(vacina));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar vacinas." });
        }

        var vacina = await db.Vacinas.SingleOrDefaultAsync(v => v.Id == id && v.PetId == petId);

        if (vacina is null)
        {
            return NotFound();
        }

        db.Vacinas.Remove(vacina);
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

    private static VacinaResponse ToResponse(VacinaModel vacina) => new(
        vacina.Id,
        vacina.Nome,
        vacina.DataAplicacao,
        vacina.DataProximaDose,
        vacina.AntecedenciaLembreteDias,
        GetStatus(vacina));

    // Vencida: data da próxima dose já passou. Proxima: hoje já está dentro da janela de
    // antecedência (DataProximaDose - AntecedenciaLembreteDias) mas a data ainda não passou.
    private static VacinaStatus GetStatus(VacinaModel vacina)
    {
        if (vacina.DataProximaDose is null)
        {
            return VacinaStatus.SemProximaDose;
        }

        var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
        var dataProximaDose = vacina.DataProximaDose.Value;

        if (dataProximaDose < hoje)
        {
            return VacinaStatus.Vencida;
        }

        var inicioJanelaLembrete = dataProximaDose.AddDays(-vacina.AntecedenciaLembreteDias);

        return hoje >= inicioJanelaLembrete ? VacinaStatus.Proxima : VacinaStatus.EmDia;
    }
}
