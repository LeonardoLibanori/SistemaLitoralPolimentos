using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLitoralPolimentos.Migrations
{
    /// <inheritdoc />
    public partial class AddArquivoToSolicitacaoServico : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArquivoBase64",
                table: "SolicitacoesServico",
                type: "TEXT",
                maxLength: 5242880,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NomeArquivo",
                table: "SolicitacoesServico",
                type: "TEXT",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArquivoBase64",
                table: "SolicitacoesServico");

            migrationBuilder.DropColumn(
                name: "NomeArquivo",
                table: "SolicitacoesServico");
        }
    }
}
