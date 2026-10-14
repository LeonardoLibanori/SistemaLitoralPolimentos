using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLitoralPolimentos.Migrations
{
    /// <inheritdoc />
    public partial class CriacaoInicial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clientes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Nome = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    CpfCnpj = table.Column<string>(type: "TEXT", maxLength: 18, nullable: true),
                    Telefone = table.Column<string>(type: "TEXT", maxLength: 20, nullable: true),
                    Email = table.Column<string>(type: "TEXT", maxLength: 200, nullable: true),
                    Endereco = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    DataCadastro = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clientes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Documentos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TipoDocumento = table.Column<string>(type: "TEXT", nullable: false),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "TEXT", nullable: false, defaultValueSql: "datetime('now')"),
                    Valor = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    ConteudoJson = table.Column<string>(type: "TEXT", nullable: false),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Pendente"),
                    DataAprovacao = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documentos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documentos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Servicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClienteId = table.Column<int>(type: "INTEGER", nullable: false),
                    DocumentoId = table.Column<int>(type: "INTEGER", nullable: true),
                    DataServico = table.Column<DateTime>(type: "TEXT", nullable: false),
                    HoraServico = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    EnderecoServico = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Latitude = table.Column<double>(type: "REAL", nullable: true),
                    Longitude = table.Column<double>(type: "REAL", nullable: true),
                    Observacoes = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: true),
                    Status = table.Column<string>(type: "TEXT", nullable: false, defaultValue: "Agendado")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servicos", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Servicos_Clientes_ClienteId",
                        column: x => x.ClienteId,
                        principalTable: "Clientes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Servicos_Documentos_DocumentoId",
                        column: x => x.DocumentoId,
                        principalTable: "Documentos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documentos_ClienteId",
                table: "Documentos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_ClienteId",
                table: "Servicos",
                column: "ClienteId");

            migrationBuilder.CreateIndex(
                name: "IX_Servicos_DocumentoId",
                table: "Servicos",
                column: "DocumentoId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Servicos");

            migrationBuilder.DropTable(
                name: "Documentos");

            migrationBuilder.DropTable(
                name: "Clientes");
        }
    }
}
