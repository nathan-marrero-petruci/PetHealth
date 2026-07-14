using System.Security.Claims;
using Api.Data;
using Api.DTOs.Refeicao;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DietaPadraoModel = Api.Models.DietaPadrao;
using RefeicaoModel = Api.Models.Refeicao;

namespace Api.Controllers;

[ApiController]
[Route("api/refeicoes")]
[Authorize]
public class RefeicaoController(AppDbContext db) : ControllerBase
{
    private const decimal QuantidadeMaximaGramas = 9999.99m;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar refeições." });
        }

        var refeicoes = await db.Refeicoes
            .Where(r => r.PetId == petId)
            .OrderByDescending(r => r.DataHora)
            .ToListAsync();

        var dieta = await GetDietaAsync(petId.Value);

        return Ok(refeicoes.Select(r => ToResponse(r, dieta)));
    }

    [HttpPost]
    public async Task<IActionResult> Create(RefeicaoRequest request)
    {
        var validationError = Validate(request);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar refeições." });
        }

        var refeicao = new RefeicaoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            DataHora = NormalizeToUtc(request.DataHora!.Value),
            QuantidadeGramas = Math.Round(request.QuantidadeGramas!.Value, 2)
        };

        db.Refeicoes.Add(refeicao);
        await db.SaveChangesAsync();

        var dieta = await GetDietaAsync(petId.Value);

        // Requery sem tracking para refletir a escala decimal de numeric(6,2) vinda do
        // Postgres, igual ao GET (mesmo ajuste feito em DietaPadraoController).
        var refeicaoSalva = await db.Refeicoes.AsNoTracking().SingleAsync(r => r.Id == refeicao.Id);

        return Ok(ToResponse(refeicaoSalva, dieta));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, RefeicaoRequest request)
    {
        var validationError = Validate(request);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar refeições." });
        }

        var refeicao = await db.Refeicoes.SingleOrDefaultAsync(r => r.Id == id && r.PetId == petId);

        if (refeicao is null)
        {
            return NotFound();
        }

        refeicao.DataHora = NormalizeToUtc(request.DataHora!.Value);
        refeicao.QuantidadeGramas = Math.Round(request.QuantidadeGramas!.Value, 2);

        await db.SaveChangesAsync();

        var dieta = await GetDietaAsync(petId.Value);

        var refeicaoSalva = await db.Refeicoes.AsNoTracking().SingleAsync(r => r.Id == refeicao.Id);

        return Ok(ToResponse(refeicaoSalva, dieta));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar refeições." });
        }

        var refeicao = await db.Refeicoes.SingleOrDefaultAsync(r => r.Id == id && r.PetId == petId);

        if (refeicao is null)
        {
            return NotFound();
        }

        db.Refeicoes.Remove(refeicao);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static string? Validate(RefeicaoRequest request)
    {
        if (request.QuantidadeGramas is null or <= 0)
        {
            return "Quantidade servida é obrigatória e deve ser um valor positivo.";
        }

        if (request.QuantidadeGramas > QuantidadeMaximaGramas)
        {
            return $"Quantidade servida não pode ser maior que {QuantidadeMaximaGramas} g.";
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

    private async Task<DietaPadraoModel?> GetDietaAsync(Guid petId) =>
        await db.DietasPadrao.AsNoTracking().SingleOrDefaultAsync(d => d.PetId == petId);

    // System.Text.Json marca o Kind conforme o offset recebido: "Z" ou nenhum offset numérico
    // vira Utc; um offset explícito (ex: +05:00) é convertido para o fuso local da máquina e
    // marcado como Local; ausência total de offset vira Unspecified. Para persistir sempre em
    // UTC de forma determinística (independente do fuso do servidor), tratamos os três casos:
    // Utc mantém, Local converte de volta para UTC (recupera o instante correto), e Unspecified
    // assume que o valor já é UTC (só relabela, sem converter).
    private static DateTime NormalizeToUtc(DateTime value) => value.Kind switch
    {
        DateTimeKind.Utc => value,
        DateTimeKind.Local => value.ToUniversalTime(),
        _ => DateTime.SpecifyKind(value, DateTimeKind.Utc)
    };

    private static RefeicaoResponse ToResponse(RefeicaoModel refeicao, DietaPadraoModel? dieta) => new(
        refeicao.Id,
        refeicao.DataHora,
        refeicao.QuantidadeGramas,
        dieta is null ? null : refeicao.QuantidadeGramas - dieta.QuantidadePorRefeicaoGramas);
}
