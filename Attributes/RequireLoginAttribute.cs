using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SistemaLitoralPolimentos.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class RequireLoginAttribute : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Verifica se a ação tem o atributo AllowAnonymous
            var allowAnonymous = context.ActionDescriptor.EndpointMetadata
                .OfType<AllowAnonymousAttribute>()
                .Any();

            if (allowAnonymous)
            {
                return; // Permite acesso sem autenticação
            }

            // Verifica se a sessão está setada como autenticada
            string? autenticado = context.HttpContext.Session.GetString("Autenticado");

            if (string.IsNullOrEmpty(autenticado) || autenticado != "true")
            {
                // Redireciona para o login se não estiver autenticado
                context.Result = new RedirectToActionResult("Login", "Home", null);
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Nada a fazer após a ação
        }
    }
}
