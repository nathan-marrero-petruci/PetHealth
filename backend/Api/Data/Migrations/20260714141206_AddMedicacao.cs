using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMedicacao : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Medicacoes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Nome = table.Column<string>(type: "text", nullable: false),
                    DosagemValor = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    DosagemUnidade = table.Column<string>(type: "text", nullable: false),
                    VezesPorDia = table.Column<int>(type: "integer", nullable: false),
                    IntervaloHoras = table.Column<int>(type: "integer", nullable: false),
                    DataInicio = table.Column<DateOnly>(type: "date", nullable: false),
                    DataTermino = table.Column<DateOnly>(type: "date", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicacoes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Medicacoes_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Medicacoes_PetId",
                table: "Medicacoes",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Medicacoes");
        }
    }
}
