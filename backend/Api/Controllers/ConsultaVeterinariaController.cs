using System.Security.Claims;
using Api.Data;
using Api.DTOs.ConsultaVeterinaria;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ConsultaVeterinariaModel = Api.Models.ConsultaVeterinaria;

namespace Api.Controllers;

[ApiController]
[Route("api/consultas")]
[Authorize]
public class ConsultaVeterinariaController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List(DateOnly? dataInicio, DateOnly? dataFim)
    {
        if (dataInicio is not null && dataFim is not null && dataFim < dataInicio)
        {
            return BadRequest(new { message = "Data final não pode ser anterior à data inicial." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar consultas." });
        }

        var query = db.ConsultasVeterinarias.Where(c => c.PetId == petId);

        if (dataInicio is not null)
        {
            query = query.Where(c => c.Data >= dataInicio);
        }

        if (dataFim is not null)
        {
            query = query.Where(c => c.Data <= dataFim);
        }

        var consultas = await query.OrderByDescending(c => c.Data).ToListAsync();

        return Ok(consultas.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ConsultaVeterinariaRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar consultas." });
        }

        var consulta = new ConsultaVeterinariaModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Data = request.Data!.Value,
            Motivo = request.Motivo,
            VeterinarioClinica = request.VeterinarioClinica,
            Observacoes = request.Observacoes
        };

        db.ConsultasVeterinarias.Add(consulta);
        await db.SaveChangesAsync();

        return Ok(ToResponse(consulta));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ConsultaVeterinariaRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar consultas." });
        }

        var consulta = await db.ConsultasVeterinarias.SingleOrDefaultAsync(c => c.Id == id && c.PetId == petId);

        if (consulta is null)
        {
            return NotFound();
        }

        consulta.Data = request.Data!.Value;
        consulta.Motivo = request.Motivo;
        consulta.VeterinarioClinica = request.VeterinarioClinica;
        consulta.Observacoes = request.Observacoes;

        await db.SaveChangesAsync();

        return Ok(ToResponse(consulta));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar consultas." });
        }

        var consulta = await db.ConsultasVeterinarias.SingleOrDefaultAsync(c => c.Id == id && c.PetId == petId);

        if (consulta is null)
        {
            return NotFound();
        }

        db.ConsultasVeterinarias.Remove(consulta);
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

    private static ConsultaVeterinariaResponse ToResponse(ConsultaVeterinariaModel consulta) => new(
        consulta.Id,
        consulta.Data,
        consulta.Motivo,
        consulta.VeterinarioClinica,
        consulta.Observacoes);
}
