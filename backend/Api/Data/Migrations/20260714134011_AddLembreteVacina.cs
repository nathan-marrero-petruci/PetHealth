using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLembreteVacina : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Default de 15 dias aplicado apenas às vacinas já cadastradas antes desta migration
            // (a coluna é NOT NULL e não pode ficar sem valor). Cadastros/edições novos passam a
            // exigir o campo explicitamente via validação da API (VacinaRequest).
            migrationBuilder.AddColumn<int>(
                name: "AntecedenciaLembreteDias",
                table: "Vacinas",
                type: "integer",
                nullable: false,
                defaultValue: 15);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AntecedenciaLembreteDias",
                table: "Vacinas");
        }
    }
}
