using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tutor> Tutores => Set<Tutor>();

    // Demais DbSets (Pet, RegistroPeso, Vacina, etc.) serão adicionados
    // conforme os módulos forem implementados — ver docs/requisitos.md.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.HasIndex(t => t.Email).IsUnique();

            // Seed fixo do único tutor do sistema (FUND-01) — hash BCrypt (work factor 11).
            // Senha padrão documentada no README.
            entity.HasData(new Tutor
            {
                Id = Guid.Parse("6f2b6c2b-6b8d-4a7d-9d1e-2f6a8d3c9e10"),
                Email = "tutor@pethealth.local",
                PasswordHash = "$2a$11$2CbxT6s6PjSGYCKvC9GgfetVfI12qwRRj4oA0dpiDDretaad/6j0S"
            });
        });
    }
}
