using Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Tutor> Tutores => Set<Tutor>();
    public DbSet<Pet> Pets => Set<Pet>();
    public DbSet<RegistroPeso> RegistrosPeso => Set<RegistroPeso>();
    public DbSet<Vacina> Vacinas => Set<Vacina>();
    public DbSet<ConsultaVeterinaria> ConsultasVeterinarias => Set<ConsultaVeterinaria>();
    public DbSet<Medicacao> Medicacoes => Set<Medicacao>();
    public DbSet<Vermifugacao> Vermifugacoes => Set<Vermifugacao>();
    public DbSet<Antipulga> Antipulgas => Set<Antipulga>();
    public DbSet<Observacao> Observacoes => Set<Observacao>();
    public DbSet<DietaPadrao> DietasPadrao => Set<DietaPadrao>();
    public DbSet<Refeicao> Refeicoes => Set<Refeicao>();
    public DbSet<ItemForaDieta> ItensForaDieta => Set<ItemForaDieta>();

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

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.HasOne(p => p.Tutor)
                .WithMany()
                .HasForeignKey(p => p.TutorId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.TutorId).IsUnique();

            entity.Property(p => p.PesoReferencia).HasColumnType("numeric(5,2)");
        });

        modelBuilder.Entity<RegistroPeso>(entity =>
        {
            entity.HasOne(r => r.Pet)
                .WithMany()
                .HasForeignKey(r => r.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(r => r.Peso).HasColumnType("numeric(5,2)");
        });

        modelBuilder.Entity<Vacina>(entity =>
        {
            entity.HasOne(v => v.Pet)
                .WithMany()
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ConsultaVeterinaria>(entity =>
        {
            entity.HasOne(c => c.Pet)
                .WithMany()
                .HasForeignKey(c => c.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Medicacao>(entity =>
        {
            entity.HasOne(m => m.Pet)
                .WithMany()
                .HasForeignKey(m => m.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(m => m.DosagemValor).HasColumnType("numeric(5,2)");
        });

        modelBuilder.Entity<Vermifugacao>(entity =>
        {
            entity.HasOne(v => v.Pet)
                .WithMany()
                .HasForeignKey(v => v.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Antipulga>(entity =>
        {
            entity.HasOne(a => a.Pet)
                .WithMany()
                .HasForeignKey(a => a.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Observacao>(entity =>
        {
            entity.HasOne(o => o.Pet)
                .WithMany()
                .HasForeignKey(o => o.PetId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<DietaPadrao>(entity =>
        {
            entity.HasOne(d => d.Pet)
                .WithMany()
                .HasForeignKey(d => d.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(d => d.PetId).IsUnique();

            entity.Property(d => d.QuantidadeDiariaGramas).HasColumnType("numeric(6,2)");
            entity.Property(d => d.QuantidadePorRefeicaoGramas).HasColumnType("numeric(6,2)");
        });

        modelBuilder.Entity<Refeicao>(entity =>
        {
            entity.HasOne(r => r.Pet)
                .WithMany()
                .HasForeignKey(r => r.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Property(r => r.QuantidadeGramas).HasColumnType("numeric(6,2)");
            // timestamptz é o tipo idiomático do Postgres para guardar instantes em UTC;
            // o app sempre normaliza DataHora para Kind=Utc antes de persistir (ver
            // RefeicaoController.NormalizeToUtc).
            entity.Property(r => r.DataHora).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<ItemForaDieta>(entity =>
        {
            entity.HasOne(i => i.Pet)
                .WithMany()
                .HasForeignKey(i => i.PetId)
                .OnDelete(DeleteBehavior.Cascade);

            // timestamptz é o tipo idiomático do Postgres para guardar instantes em UTC;
            // o app sempre normaliza DataHora para Kind=Utc antes de persistir (ver
            // ItemForaDietaController.NormalizeToUtc).
            entity.Property(i => i.DataHora).HasColumnType("timestamp with time zone");
        });
    }
}
