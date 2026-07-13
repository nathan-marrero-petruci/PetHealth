using System.Security.Claims;
using Api.Data;
using Api.DTOs.Pet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetModel = Api.Models.Pet;

namespace Api.Controllers;

[ApiController]
[Route("api/pet")]
[Authorize]
public class PetController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var pet = await db.Pets.SingleOrDefaultAsync(p => p.TutorId == GetTutorId());

        if (pet is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(pet));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert(PetRequest request)
    {
        if (request.DataNascimento > DateOnly.FromDateTime(DateTime.UtcNow))
        {
            return BadRequest(new { message = "Data de nascimento não pode ser futura." });
        }

        if (request.PesoReferencia <= 0)
        {
            return BadRequest(new { message = "Peso de referência deve ser um valor positivo." });
        }

        var tutorId = GetTutorId();
        var pet = await db.Pets.SingleOrDefaultAsync(p => p.TutorId == tutorId);

        if (pet is null)
        {
            pet = new PetModel { Id = Guid.NewGuid(), TutorId = tutorId };
            db.Pets.Add(pet);
        }

        pet.Nome = request.Nome;
        pet.Raca = request.Raca;
        pet.Porte = request.Porte!.Value;
        pet.DataNascimento = request.DataNascimento;
        pet.Sexo = request.Sexo!.Value;
        pet.FotoUrl = request.FotoUrl;
        pet.PesoReferencia = request.PesoReferencia;

        await db.SaveChangesAsync();

        return Ok(ToResponse(pet));
    }

    private Guid GetTutorId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private static PetResponse ToResponse(PetModel pet) => new(
        pet.Id, pet.Nome, pet.Raca, pet.Porte, pet.DataNascimento, pet.Sexo, pet.FotoUrl, pet.PesoReferencia);
}
