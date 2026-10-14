# 📋 Backlog de Melhorias e Dívidas Técnicas

Este documento lista melhorias estruturais, arquiteturais e de segurança a serem implementadas no projeto. A qualquer momento, você pode pedir para a IA ler este arquivo e executar uma das tarefas.

À medida que os tópicos forem concluídos, o rascunho do currículo (no final deste arquivo) será atualizado para refletir as novas ferramentas e padrões aplicados.

## 1. Segurança e Autenticação
- [x] **Substituir Autenticação Baseada em Sessão por Cookie Auth Nativo:** O sistema atual usa `HttpContext.Session`. Isso deve ser migrado para o middleware de autenticação nativo do .NET (`CookieAuthenticationDefaults`) utilizando `ClaimsPrincipal`.
- [ ] **Gestão de Segredos:** Isolar chaves de APIs e strings de conexão, garantindo que sejam gerenciadas por variáveis de ambiente ou *User Secrets* em desenvolvimento, e não *hardcoded*.

## 2. Banco de Dados, Storage e Performance
- [x] **Remover Armazenamento de Base64 do Banco de Dados:** A propriedade `ArquivoBase64` na tabela de Solicitações incha o banco SQLite. O correto é salvar a imagem no disco (ex: `wwwroot/uploads`) e gravar apenas o `string CaminhoArquivo` no banco.
- [x] **Tuning de Consultas LINQ:** Nas listagens (como o `Dashboard` e `Index`), evitar realizar `.ToListAsync()` antes dos filtros e agregações (`Count()`, `Sum()`). O processamento deve ocorrer no banco de dados e não na memória RAM da aplicação.
- [x] **Uso de AsNoTracking:** Implementar `.AsNoTracking()` em todas as consultas exclusivas para leitura, economizando memória no *Change Tracker* do Entity Framework.

## 3. Arquitetura e Clean Code
- [x] **Camada de Serviços (Desacoplamento):** Extrair a lógica de negócio pesada e manipulação de banco de dados de dentro dos `Controllers` para classes na pasta `Services`. O Controller deve focar apenas em receber requests e devolver views/json.
- [ ] **Testes Automatizados:** Configurar um projeto de testes (com xUnit, por exemplo) para validar o cálculo de dias livres, a formatação de dados, e a criação de entidades de agendamento.

---

## 📄 Rascunho do Currículo (Backend Júnior)
*Status atual das descrições para o seu currículo. Elas serão incrementadas conforme você aprovar a execução das melhorias acima.*

> - **Desenvolvimento de Arquitetura Backend:** Criação da base de um sistema de gestão de agendamentos e documentos utilizando **C#, ASP.NET Core MVC** e **.NET 8**.
> - **Modelagem de Dados e ORM:** Estruturação de banco relacional (**SQLite**) baseada em *Code-First*, com evolução do esquema via *Migrations* do **Entity Framework Core (EF Core)**.
> - **Processamento de Dados:** Construção de consultas com **LINQ** para filtragem e agregação de métricas no Dashboard.
> - **Integração de APIs REST:** Configuração de integrações via `HttpClient` com **Google Calendar API** (*OAuth*) e serviços de geolocalização.
> - **Implementação de Segurança Backend:** Aplicação de autenticação por sessão, hash criptográfico (**BCrypt**) e injeção de *Security Headers*.
> - **Design de Código:** Configuração do container de **Injeção de Dependência (DI)** para desacoplamento de serviços.

### 🚀 Oportunidades para o Currículo (Pós-Melhorias):
Quando as melhorias do backlog forem feitas, adicionaremos frases valiosas como:
*   *"Implementação de infraestrutura robusta de controle de acesso utilizando ASP.NET Core Authentication/Identity com Claims e Cookies Seguros."*
*   *"Otimização de performance de banco de dados migrando processamento de arquivos pesados (Base64) para Storage físico, preservando a integridade do SGBD."*
*   *"Garantia de qualidade de software através da implementação de testes unitários automatizados cobrindo regras de negócio com xUnit."*
*   *"Refatoração de código legado (Clean Code), separando responsabilidades através da implementação da Camada de Serviço."*
*   *"Tuning de performance no EF Core, otimizando consultas com agregação (IQueryable) e leitura de baixo custo de memória (AsNoTracking)."*
