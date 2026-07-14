using System.Security.Claims;
using Api.Data;
using Api.DTOs.DietaPadrao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietaPadraoModel = Api.Models.DietaPadrao;

namespace Api.Controllers;

[ApiController]
[Route("api/dieta")]
[Authorize]
public class DietaPadraoController(AppDbContext db) : ControllerBase
{
    private const decimal QuantidadeMaximaGramas = 9999.99m;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de cadastrar a dieta." });
        }

        var dieta = await db.DietasPadrao.SingleOrDefaultAsync(d => d.PetId == petId);

        if (dieta is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(dieta));
    }

    [HttpPut]
    public async Task<IActionResult> Upsert(DietaPadraoRequest request)
    {
        if (request.QuantidadeDiariaGramas is null or <= 0)
        {
            return BadRequest(new { message = "Quantidade diária recomendada é obrigatória e deve ser um valor positivo." });
        }

        if (request.QuantidadeDiariaGramas > QuantidadeMaximaGramas)
        {
            return BadRequest(new { message = $"Quantidade diária recomendada não pode ser maior que {QuantidadeMaximaGramas} g." });
        }

        if (request.QuantidadePorRefeicaoGramas is null or <= 0)
        {
            return BadRequest(new { message = "Quantidade por refeição é obrigatória e deve ser um valor positivo." });
        }

        if (request.QuantidadePorRefeicaoGramas > QuantidadeMaximaGramas)
        {
            return BadRequest(new { message = $"Quantidade por refeição não pode ser maior que {QuantidadeMaximaGramas} g." });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de cadastrar a dieta." });
        }

        var dieta = await db.DietasPadrao.SingleOrDefaultAsync(d => d.PetId == petId);

        if (dieta is null)
        {
            dieta = new DietaPadraoModel { Id = Guid.NewGuid(), PetId = petId.Value };
            db.DietasPadrao.Add(dieta);
        }

        dieta.NomeMarca = request.NomeMarca;
        dieta.QuantidadeDiariaGramas = Math.Round(request.QuantidadeDiariaGramas!.Value, 2);
        dieta.QuantidadePorRefeicaoGramas = Math.Round(request.QuantidadePorRefeicaoGramas!.Value, 2);

        await db.SaveChangesAsync();

        // Requery sem tracking para que a resposta reflita a escala decimal de numeric(6,2)
        // vinda do Postgres (ex: 200 vira 200.00), igual ao que o GET retorna. O objeto
        // rastreado em memória mantém a escala do valor de entrada (Math.Round não normaliza).
        var dietaSalva = await db.DietasPadrao.AsNoTracking().SingleAsync(d => d.Id == dieta.Id);

        return Ok(ToResponse(dietaSalva));
    }

    private Guid GetTutorId() =>
        Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    private async Task<Guid?> GetPetIdAsync()
    {
        var tutorId = GetTutorId();
        var pet = await db.Pets.SingleOrDefaultAsync(p => p.TutorId == tutorId);

        return pet?.Id;
    }

    private static DietaPadraoResponse ToResponse(DietaPadraoModel dieta) => new(
        dieta.Id, dieta.NomeMarca, dieta.QuantidadeDiariaGramas, dieta.QuantidadePorRefeicaoGramas);
}
