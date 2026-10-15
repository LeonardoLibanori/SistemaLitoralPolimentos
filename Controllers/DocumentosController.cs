using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaLitoralPolimentos.Data;

namespace SistemaLitoralPolimentos.Controllers
{
    [Authorize]
    public class DocumentosController : Controller
    {
        private readonly AppDbContext _db;

        public DocumentosController(AppDbContext db)
        {
            _db = db;
        }

        // =====================================================
        // HISTÓRICO
        // =====================================================

        public async Task<IActionResult> Historico(
            string? tipo, string? status, string? busca, string? data)
        {
            ViewBag.Tipo = tipo;
            ViewBag.Status = status;
            ViewBag.Busca = busca;
            ViewBag.Data = data;

            var docs = _db.Documentos
                .AsNoTracking()
                .Include(d => d.Cliente)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(tipo))
                docs = docs.Where(d => d.TipoDocumento == tipo);

            if (!string.IsNullOrWhiteSpace(status))
                docs = docs.Where(d => d.Status == status);

            if (!string.IsNullOrWhiteSpace(busca))
                docs = docs.Where(d =>
                    d.Cliente != null && d.Cliente.Nome.Contains(busca));

            if (!string.IsNullOrWhiteSpace(data) &&
                DateTime.TryParse(data, out var dataFiltro))
                docs = docs.Where(d => d.DataCriacao.Date == dataFiltro.Date);

            var lista = await docs
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            // Remove duplicados (gerados por duplo clique ao criar o PDF):
            // mantém apenas o registro mais recente de cada ConteudoJson
            lista = lista
                .GroupBy(d => string.IsNullOrWhiteSpace(d.ConteudoJson)
                    ? $"unico_{d.Id}"
                    : d.ConteudoJson)
                .Select(g => g.First())
                .ToList();

            return View(lista);
        }

        // =====================================================
        // ATUALIZAR STATUS (Feito / Pendente / Cancelado)
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AtualizarStatus(int id, string status)
        {
            var documento = await _db.Documentos.FindAsync(id);
            if (documento == null) return NotFound();

            var statusValidos = new[] { "Feito", "Pendente", "Cancelado" };
            if (!statusValidos.Contains(status)) return BadRequest();

            documento.Status = status;

            if (status == "Feito")
                documento.DataAprovacao = DateTime.Now;
            else
                documento.DataAprovacao = null;

            await _db.SaveChangesAsync();

            return RedirectToAction(nameof(Historico));
        }

        // =====================================================
        // DELETAR DOCUMENTO
        // =====================================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Deletar(int id)
        {
            var documento = await _db.Documentos.FindAsync(id);
            if (documento == null)
                return NotFound();

            _db.Documentos.Remove(documento);
            await _db.SaveChangesAsync();

            // Requisição AJAX (fetch) recebe JSON; formulário clássico volta ao histórico
            if (string.Equals(
                Request.Headers["X-Requested-With"],
                "XMLHttpRequest",
                StringComparison.OrdinalIgnoreCase))
            {
                return Ok(new { mensagem = "Documento excluído com sucesso." });
            }

            TempData["Sucesso"] = "Documento excluído com sucesso.";
            return RedirectToAction(nameof(Historico));
        }
    }
}