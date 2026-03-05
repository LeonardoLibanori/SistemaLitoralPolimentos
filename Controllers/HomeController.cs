using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SistemaLitoralPolimentos.Attributes;
using SistemaLitoralPolimentos.Models;
using System.Diagnostics;

namespace SistemaLitoralPolimentos.Controllers
{
    [RequireLogin]
    public class HomeController : Controller
    {
        // Senha de acesso segura
        private readonly string _acessoSenha = "Leo1212";

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            // Se já está autenticado, vai para Index
            if (HttpContext.Session.GetString("Autenticado") == "true")
            {
                return RedirectToAction("Index");
            }
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(string senha)
        {
            if (string.IsNullOrWhiteSpace(senha) || senha != _acessoSenha)
            {
                ViewBag.Erro = "Senha incorreta!";
                return View();
            }

            // Define a sessão como autenticada
            HttpContext.Session.SetString("Autenticado", "true");
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [AllowAnonymous]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult Historico()
        {
            return View();
        }
    }
}
