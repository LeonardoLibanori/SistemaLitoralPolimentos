using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLitoralPolimentos.Migrations
{
    /// <inheritdoc />
    public partial class RemoverLimiteArquivoBase64 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
        CREATE TABLE SolicitacoesServico_new (
            Id INTEGER NOT NULL CONSTRAINT PK_SolicitacoesServico PRIMARY KEY AUTOINCREMENT,
            Nome TEXT NOT NULL,
            Telefone TEXT NOT NULL,
            Cidade TEXT NOT NULL,
            DataServico TEXT NOT NULL,
            DescricaoProblema TEXT NOT NULL,
            ArquivoBase64 TEXT,
            NomeArquivo TEXT,
            ExtensaoArquivo TEXT,
            DataCriacao TEXT NOT NULL DEFAULT (datetime('now')),
            Lido INTEGER NOT NULL DEFAULT 0
        );

        INSERT INTO SolicitacoesServico_new
        SELECT Id, Nome, Telefone, Cidade, DataServico, DescricaoProblema,
               ArquivoBase64, NomeArquivo, ExtensaoArquivo, DataCriacao, Lido
        FROM SolicitacoesServico;

        DROP TABLE SolicitacoesServico;

        ALTER TABLE SolicitacoesServico_new RENAME TO SolicitacoesServico;
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
