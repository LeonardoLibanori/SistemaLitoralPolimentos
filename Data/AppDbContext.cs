using Microsoft.EntityFrameworkCore;
using SistemaLitoralPolimentos.Models;

namespace SistemaLitoralPolimentos.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Cada DbSet representa uma tabela no banco
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Documento> Documentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configurações da tabela Cliente
            modelBuilder.Entity<Cliente>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Nome).IsRequired().HasMaxLength(200);
                entity.Property(e => e.DataCadastro).HasDefaultValueSql("datetime('now')");
            });

            // Configurações da tabela Documento
            modelBuilder.Entity<Documento>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasDefaultValue("Pendente");
                entity.Property(e => e.DataCriacao).HasDefaultValueSql("datetime('now')");

                // Um documento pertence a um cliente
                entity.HasOne(d => d.Cliente)
                      .WithMany(c => c.Documentos)
                      .HasForeignKey(d => d.ClienteId)
                      .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}