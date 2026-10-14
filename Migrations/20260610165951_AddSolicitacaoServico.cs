using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLitoralPolimentos.Migrations
{
    /// <inheritdoc />
    public partial class AddSolicitacaoServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SolicitacoesServico",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Cidade = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    DataServico = table.Column<DateTime>(type: "TEXT", nullable: false),
                    DescricaoProblema = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    Lido = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolicitacoesServico", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SolicitacoesServico");
        }
    }
}
