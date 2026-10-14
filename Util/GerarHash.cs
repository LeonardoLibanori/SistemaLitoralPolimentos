namespace SistemaLitoralPolimentos.Util
{
    public static class GerarHash
    {
        /// <summary>
        /// Gera um hash BCrypt para uma senha.
        /// Use apenas para configuração inicial — remova após gerar.
        /// </summary>
        public static string Hash(string senha)
        {
            return BCrypt.Net.BCrypt.HashPassword(senha, workFactor: 12);
        }

        public static bool Verificar(string senha, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
    }
}