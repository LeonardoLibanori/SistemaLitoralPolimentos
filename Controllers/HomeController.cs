using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SistemaLitoralPolimentos.Data;
using SistemaLitoralPolimentos.Models;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;

namespace SistemaLitoralPolimentos.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;
        private readonly IHttpClientFactory _httpFactory;

        public HomeController(AppDbContext db, IConfiguration config, IHttpClientFactory httpFactory)
        {
            _db = db;
            _config = config;
            _httpFactory = httpFactory;
        }

        public IActionResult Index() => View();

        public async Task<IActionResult> Dashboard()
        {
            // Remove duplicados (mesma lógica do Histórico): mantém apenas o
            // registro mais recente de cada ConteudoJson para as contagens
            var documentos = await _db.Documentos
                .AsNoTracking()
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            documentos = documentos
                .GroupBy(d => string.IsNullOrWhiteSpace(d.ConteudoJson)
                    ? $"unico_{d.Id}"
                    : d.ConteudoJson)
                .Select(g => g.First())
                .ToList();

            var orcamentos = documentos
                .Where(d => d.TipoDocumento == "Orcamento")
                .ToList();

            var orcamentosFeitos = orcamentos
                .Where(d => d.Status == "Feito")
                .ToList();

            ViewBag.ValorFeito = orcamentosFeitos.Sum(d => (double)d.Valor);
            ViewBag.TotalFeitos = orcamentosFeitos.Count;
            ViewBag.Feitos = ViewBag.TotalFeitos;

            ViewBag.TotalDocumentos = documentos.Count;
            ViewBag.Pendentes = orcamentos.Count(d => d.Status == "Pendente");
            ViewBag.Cancelados = orcamentos.Count(d => d.Status == "Cancelado");

            return View();
        }

        // =====================================================
        // LOGIN
        // =====================================================

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity != null && User.Identity.IsAuthenticated)
                return RedirectToAction("Index");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha))
            {
                ViewBag.Erro = "Digite a senha.";
                return View();
            }

            var hashSalvo = _config["Auth:SenhaHash"];

            if (string.IsNullOrWhiteSpace(hashSalvo))
            {
                ViewBag.Erro = "Sistema não configurado. Contate o administrador.";
                return View();
            }

            bool senhaCorreta;
            try
            {
                senhaCorreta = BCrypt.Net.BCrypt.Verify(senha, hashSalvo);
            }
            catch
            {
                senhaCorreta = false;
            }

            if (!senhaCorreta)
            {
                ViewBag.Erro = "Senha incorreta!";
                return View();
            }

            var nomeUsuario = _config["Auth:NomeUsuario"] ?? "Administrador";

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, nomeUsuario)
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(60)
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(claimsIdentity), 
                authProperties);

            HttpContext.Session.SetString("Autenticado", "true");
            HttpContext.Session.SetString("NomeUsuario", nomeUsuario);

            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // =====================================================
        // OUTROS
        // =====================================================

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        public IActionResult Historico() => View();

        // =====================================================
        // ENDPOINTS DE DOCUMENTOS
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> BuscarHistorico()
        {
            var documentos = await _db.Documentos
                .AsNoTracking()
                .Include(d => d.Cliente)
                .OrderByDescending(d => d.DataCriacao)
                .ToListAsync();

            // Remove duplicados (mesma lógica do Histórico):
            // mantém apenas o registro mais recente de cada ConteudoJson
            var distintos = documentos
                .GroupBy(d => string.IsNullOrWhiteSpace(d.ConteudoJson)
                    ? $"unico_{d.Id}"
                    : d.ConteudoJson)
                .Select(g => g.First())
                .Select(d => new
                {
                    id = d.Id,
                    tipo = d.TipoDocumento,
                    cliente = d.Cliente != null ? d.Cliente.Nome : "—",
                    data = d.DataCriacao.ToString("dd/MM/yyyy"),
                    valor = d.Valor,
                    status = d.Status,
                    dataAprovacao = d.DataAprovacao.HasValue
                        ? d.DataAprovacao.Value.ToString("dd/MM/yyyy")
                        : null,
                    conteudoJson = d.ConteudoJson
                })
                .ToList();

            return Ok(distintos);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> SalvarDocumento(
    [FromBody] SalvarDocumentoRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.TipoDocumento))
                return BadRequest(new { erro = "Dados inválidos." });

            // Idempotência: se o mesmo documento (tipo + conteúdo) acabou de ser
            // salvo — duplo clique ou reenvio acidental — não cria registro novo
            var conteudo = request.ConteudoJson ?? "{}";
            var janelaDuplicidade = DateTime.Now.AddSeconds(-5);
            var existente = await _db.Documentos
                .Where(d => d.TipoDocumento == request.TipoDocumento
                         && d.ConteudoJson == conteudo
                         && d.DataCriacao >= janelaDuplicidade)
                .OrderByDescending(d => d.Id)
                .FirstOrDefaultAsync();

            if (existente != null)
                return Ok(new
                {
                    id = existente.Id,
                    mensagem = "Documento duplicado ignorado."
                });

            var cliente = await _db.Clientes
                .FirstOrDefaultAsync(c => c.Nome == request.NomeCliente);

            if (cliente == null)
            {
                cliente = new Cliente
                {
                    Nome = request.NomeCliente ?? "Cliente não informado",
                    Telefone = request.TelefoneCliente,
                    Endereco = request.EnderecoCliente,
                    DataCadastro = DateTime.Now
                };
                _db.Clientes.Add(cliente);
                await _db.SaveChangesAsync();
            }

            var documento = new Documento
            {
                TipoDocumento = request.TipoDocumento,
                ClienteId = cliente.Id,
                DataCriacao = DateTime.Now,
                Valor = request.Valor,
                ConteudoJson = request.ConteudoJson ?? "{}",
                Status = "Pendente"
            };

            _db.Documentos.Add(documento);
            await _db.SaveChangesAsync();

            return Ok(new { id = documento.Id, mensagem = "Documento salvo." });
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult SalvarHistorico([FromBody] JsonElement dados) => Ok();

        [HttpPost]
        [IgnoreAntiforgeryToken]
        public IActionResult LimparHistorico() => Ok();
    }

    public class SalvarDocumentoRequest
    {
        public string? TipoDocumento { get; set; }
        public string? NomeCliente { get; set; }
        public string? TelefoneCliente { get; set; }
        public string? EnderecoCliente { get; set; }
        public decimal Valor { get; set; }
        public string? ConteudoJson { get; set; }
    }
}