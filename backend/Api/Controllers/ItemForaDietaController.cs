using System.Security.Claims;
using Api.Data;
using Api.DTOs.ItemForaDieta;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ItemForaDietaModel = Api.Models.ItemForaDieta;

namespace Api.Controllers;

[ApiController]
[Route("api/itens-fora-dieta")]
[Authorize]
public class ItemForaDietaController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar itens fora da dieta." });
        }

        var itens = await db.ItensForaDieta
            .Where(i => i.PetId == petId)
            .OrderByDescending(i => i.DataHora)
            .ToListAsync();

        return Ok(itens.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(ItemForaDietaRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar itens fora da dieta." });
        }

        var item = new ItemForaDietaModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            DataHora = NormalizeToUtc(request.DataHora!.Value),
            Descricao = request.Descricao,
            QuantidadeObservacao = request.QuantidadeObservacao
        };

        db.ItensForaDieta.Add(item);
        await db.SaveChangesAsync();

        return Ok(ToResponse(item));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, ItemForaDietaRequest request)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar itens fora da dieta." });
        }

        var item = await db.ItensForaDieta.SingleOrDefaultAsync(i => i.Id == id && i.PetId == petId);

        if (item is null)
        {
            return NotFound();
        }

        item.DataHora = NormalizeToUtc(request.DataHora!.Value);
        item.Descricao = request.Descricao;
        item.QuantidadeObservacao = request.QuantidadeObservacao;

        await db.SaveChangesAsync();

        return Ok(ToResponse(item));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar itens fora da dieta." });
        }

        var item = await db.ItensForaDieta.SingleOrDefaultAsync(i => i.Id == id && i.PetId == petId);

        if (item is null)
        {
            return NotFound();
        }

        db.ItensForaDieta.Remove(item);
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

    private static ItemForaDietaResponse ToResponse(ItemForaDietaModel item) => new(
        item.Id,
        item.DataHora,
        item.Descricao,
        item.QuantidadeObservacao);
}
