using System.Security.Claims;
using Api.Data;
using Api.DTOs.SessaoTreino;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SessaoTreinoModel = Api.Models.SessaoTreino;
using SessaoTreinoComandoModel = Api.Models.SessaoTreinoComando;

namespace Api.Controllers;

[ApiController]
[Route("api/sessoes-treino")]
[Authorize]
public class SessaoTreinoController(AppDbContext db) : ControllerBase
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
            return BadRequest(new { message = "Cadastre o pet antes de registrar sessões de treino." });
        }

        var query = db.SessoesTreino
            .Include(s => s.SessaoTreinoComandos)
            .ThenInclude(c => c.ComandoTreino)
            .Where(s => s.PetId == petId);

        if (dataInicio is not null)
        {
            query = query.Where(s => s.Data >= dataInicio);
        }

        if (dataFim is not null)
        {
            query = query.Where(s => s.Data <= dataFim);
        }

        var sessoes = await query.OrderByDescending(s => s.Data).ToListAsync();

        return Ok(sessoes.Select(ToResponse));
    }

    [HttpPost]
    public async Task<IActionResult> Create(SessaoTreinoRequest request)
    {
        var validationError = Validate(request);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar sessões de treino." });
        }

        var comandoIds = request.Comandos.Select(c => c.ComandoTreinoId!.Value).ToList();
        var comandosValidos = await db.ComandosTreino
            .Where(c => c.PetId == petId && comandoIds.Contains(c.Id))
            .ToListAsync();

        if (comandosValidos.Count != comandoIds.Count)
        {
            return BadRequest(new { message = "Um ou mais comandos de treino informados não existem ou não pertencem ao pet." });
        }

        var sessao = new SessaoTreinoModel
        {
            Id = Guid.NewGuid(),
            PetId = petId.Value,
            Data = request.Data!.Value,
            SessaoTreinoComandos = request.Comandos.Select(c => new SessaoTreinoComandoModel
            {
                Id = Guid.NewGuid(),
                ComandoTreinoId = c.ComandoTreinoId!.Value,
                NivelSucesso = c.NivelSucesso!.Value,
                ComandoTreino = comandosValidos.Single(cv => cv.Id == c.ComandoTreinoId)
            }).ToList()
        };

        db.SessoesTreino.Add(sessao);
        await db.SaveChangesAsync();

        return Ok(ToResponse(sessao));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, SessaoTreinoRequest request)
    {
        var validationError = Validate(request);

        if (validationError is not null)
        {
            return BadRequest(new { message = validationError });
        }

        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar sessões de treino." });
        }

        var sessao = await db.SessoesTreino.SingleOrDefaultAsync(s => s.Id == id && s.PetId == petId);

        if (sessao is null)
        {
            return NotFound();
        }

        var comandoIds = request.Comandos.Select(c => c.ComandoTreinoId!.Value).ToList();
        var comandosValidos = await db.ComandosTreino
            .Where(c => c.PetId == petId && comandoIds.Contains(c.Id))
            .ToListAsync();

        if (comandosValidos.Count != comandoIds.Count)
        {
            return BadRequest(new { message = "Um ou mais comandos de treino informados não existem ou não pertencem ao pet." });
        }

        sessao.Data = request.Data!.Value;

        // Substitui completamente a lista de comandos trabalhados (remove os antigos e cria
        // os novos direto no DbSet, via FK) em vez de fazer merge parcial entre o que já
        // existia e o que foi enviado. Mexer na coleção de navegação já carregada (Clear/Add)
        // confunde o change tracker do EF (ele reaproveita a identidade de uma linha removida
        // para o novo item), por isso a substituição é feita via SessaoTreinoComandos (DbSet).
        var comandosAntigos = await db.SessaoTreinoComandos
            .Where(c => c.SessaoTreinoId == sessao.Id)
            .ToListAsync();

        db.SessaoTreinoComandos.RemoveRange(comandosAntigos);

        var novosComandos = request.Comandos.Select(c => new SessaoTreinoComandoModel
        {
            Id = Guid.NewGuid(),
            SessaoTreinoId = sessao.Id,
            ComandoTreinoId = c.ComandoTreinoId!.Value,
            NivelSucesso = c.NivelSucesso!.Value,
            ComandoTreino = comandosValidos.Single(cv => cv.Id == c.ComandoTreinoId)
        }).ToList();

        db.SessaoTreinoComandos.AddRange(novosComandos);

        await db.SaveChangesAsync();

        return Ok(ToResponse(sessao.Id, sessao.Data, novosComandos));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var petId = await GetPetIdAsync();

        if (petId is null)
        {
            return BadRequest(new { message = "Cadastre o pet antes de registrar sessões de treino." });
        }

        var sessao = await db.SessoesTreino.SingleOrDefaultAsync(s => s.Id == id && s.PetId == petId);

        if (sessao is null)
        {
            return NotFound();
        }

        db.SessoesTreino.Remove(sessao);
        await db.SaveChangesAsync();

        return NoContent();
    }

    private static string? Validate(SessaoTreinoRequest request)
    {
        if (request.Comandos.Count == 0)
        {
            return "Selecione ao menos um comando de treino.";
        }

        if (request.Comandos.Any(c => c.ComandoTreinoId is null || c.NivelSucesso is null))
        {
            return "Informe o comando de treino e o nível de sucesso de cada item.";
        }

        var idsUnicos = request.Comandos.Select(c => c.ComandoTreinoId!.Value).Distinct().Count();

        if (idsUnicos != request.Comandos.Count)
        {
            return "O mesmo comando de treino não pode ser repetido na mesma sessão.";
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

    private static SessaoTreinoResponse ToResponse(SessaoTreinoModel sessao) =>
        ToResponse(sessao.Id, sessao.Data, sessao.SessaoTreinoComandos);

    private static SessaoTreinoResponse ToResponse(Guid id, DateOnly data, List<SessaoTreinoComandoModel> comandos) => new(
        id,
        data,
        comandos.Select(c => new SessaoTreinoComandoResponse(
            c.ComandoTreinoId,
            c.ComandoTreino!.Nome,
            c.NivelSucesso)).ToList());
}
