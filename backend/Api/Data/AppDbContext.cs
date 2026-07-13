using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    // DbSets das entidades (Pet, RegistroPeso, Vacina, etc.) serão adicionados
    // conforme os módulos forem implementados — ver docs/requisitos.md.
}
