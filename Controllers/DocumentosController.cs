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
            string? tipo, string? status, string? busca, string? data, string? periodo)
        {
            // Recibo e Contrato não têm ciclo de aprovação: só Orçamento
            // usa Pendente/Cancelado. Se o filtro de status for Pendente ou
            // Cancelado sem tipo definido, restringe a Orçamentos; se o tipo
            // for Recibo/Contrato com esses status, ignora o status.
            if (status is "Pendente" or "Cancelado")
            {
                if (string.IsNullOrWhiteSpace(tipo))
                    tipo = "Orcamento";
                else if (tipo is "Recibo" or "Contrato")
                    status = null;
            }

            // Período vindo do Dashboard (semana/mes/ano): filtra documentos
            // criados desde o início do período corrente. A data exata tem
            // precedência quando preenchida.
            periodo = periodo is "semana" or "mes" or "ano" ? periodo : null;
            DateTime? periodoInicio = null;
            if (periodo != null && string.IsNullOrWhiteSpace(data))
            {
                var hoje = DateTime.Today;
                periodoInicio = periodo switch
                {
                    "semana" => hoje.AddDays(-(((int)hoje.DayOfWeek + 6) % 7)), // segunda-feira
                    "ano" => new DateTime(hoje.Year, 1, 1),
                    _ => new DateTime(hoje.Year, hoje.Month, 1)
                };
            }

            ViewBag.Tipo = tipo;
            ViewBag.Status = status;
            ViewBag.Busca = busca;
            ViewBag.Data = data;
            ViewBag.Periodo = periodo;
            ViewBag.PeriodoInicio = periodoInicio;

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
            else if (periodoInicio.HasValue)
                docs = docs.Where(d => d.DataCriacao >= periodoInicio.Value);

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

            // Recibo e Contrato não têm ciclo de aprovação: o status deles
            // é sempre "Feito" e não pode ser alterado.
            if (documento.TipoDocumento is "Recibo" or "Contrato")
                return BadRequest(new { erro = "Recibo e Contrato não possuem status." });

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