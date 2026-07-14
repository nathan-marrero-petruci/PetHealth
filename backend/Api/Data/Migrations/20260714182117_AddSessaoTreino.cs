using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddSessaoTreino : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SessoesTreino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    PetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Data = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessoesTreino", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessoesTreino_Pets_PetId",
                        column: x => x.PetId,
                        principalTable: "Pets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SessaoTreinoComandos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SessaoTreinoId = table.Column<Guid>(type: "uuid", nullable: false),
                    ComandoTreinoId = table.Column<Guid>(type: "uuid", nullable: false),
                    NivelSucesso = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SessaoTreinoComandos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SessaoTreinoComandos_ComandosTreino_ComandoTreinoId",
                        column: x => x.ComandoTreinoId,
                        principalTable: "ComandosTreino",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SessaoTreinoComandos_SessoesTreino_SessaoTreinoId",
                        column: x => x.SessaoTreinoId,
                        principalTable: "SessoesTreino",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SessaoTreinoComandos_ComandoTreinoId",
                table: "SessaoTreinoComandos",
                column: "ComandoTreinoId");

            migrationBuilder.CreateIndex(
                name: "IX_SessaoTreinoComandos_SessaoTreinoId",
                table: "SessaoTreinoComandos",
                column: "SessaoTreinoId");

            migrationBuilder.CreateIndex(
                name: "IX_SessoesTreino_PetId",
                table: "SessoesTreino",
                column: "PetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SessaoTreinoComandos");

            migrationBuilder.DropTable(
                name: "SessoesTreino");
        }
    }
}
