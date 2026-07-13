using System.Security.Claims;
using Api.Data;
using Api.DTOs.RegistroPeso;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RegistroPesoModel = Api.Models.RegistroPeso;

namespace Api.Controllers;

[ApiController]
[Route("api/pesos")]
[Authorize]
public class RegistroPesoController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar o peso." });
        }

        var registros = await db.RegistrosPeso
            .Where(r => r.PetId == petId)
            .OrderByDescending(r => r.Data)
            .ToListAsync();

        return Ok(registros.Select(ToResponse));
    }

    private const decimal PesoMaximo = 999.99m;

    [HttpPost]
    public async Task<IActionResult> Create(RegistroPesoRequest request)
    {
        if (request.Peso <= 0)
        {
            return BadRequest(new { message = "Peso deve ser um valor positivo." });
        }

        if (request.Peso > PesoMaximo)
        {
            return BadRequest(new { message = $"Peso não pode ser maior que {PesoMaximo} kg." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar o peso." });
        }

        var registro = new RegistroPesoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Peso = Math.Round(request.Peso, 2),
            Data = request.Data!.Value
        };

        db.RegistrosPeso.Add(registro);
        await db.SaveChangesAsync();

        return Ok(ToResponse(registro));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, RegistroPesoRequest request)
    {
        if (request.Peso <= 0)
        {
            return BadRequest(new { message = "Peso deve ser um valor positivo." });
        }

        if (request.Peso > PesoMaximo)
        {
            return BadRequest(new { message = $"Peso não pode ser maior que {PesoMaximo} kg." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar o peso." });
        }

        var registro = await db.RegistrosPeso.SingleOrDefaultAsync(r => r.Id == id && r.PetId == petId);

        if (registro is null)
        {
            return NotFound();
        }

        registro.Peso = Math.Round(request.Peso, 2);
        registro.Data = request.Data!.Value;

        await db.SaveChangesAsync();

        return Ok(ToResponse(registro));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar o peso." });
        }

        var registro = await db.RegistrosPeso.SingleOrDefaultAsync(r => r.Id == id && r.PetId == petId);

        if (registro is null)
        {
            return NotFound();
        }

        db.RegistrosPeso.Remove(registro);
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

    private static RegistroPesoResponse ToResponse(RegistroPesoModel registro) => new(
        registro.Id, registro.Peso, registro.Data);
}
