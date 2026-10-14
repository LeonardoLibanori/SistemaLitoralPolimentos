using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLitoralPolimentos.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBase64FromSolicitacoes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ArquivoBase64",
                table: "SolicitacoesServico",
                newName: "CaminhoArquivo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CaminhoArquivo",
                table: "SolicitacoesServico",
                newName: "ArquivoBase64");
        }
    }
}
