# 📋 Sistema Litoral Polimentos

Sistema de gerenciamento de documentos (orçamentos, recibos e contratos) para a empresa Litoral Polimentos, desenvolvido com **ASP.NET Core Razor Pages** e **.NET 10**.

## 🎯 Características

- ✅ **Autenticação Segura** - Tela de login obrigatória na primeira página
- ✅ **Senha Protegida** - Armazenada apenas no backend (não visível no código-fonte)
- ✅ **Sessão de 30 minutos** - Logout automático por inatividade
- ✅ **Histórico de Documentos** - Visualização de orçamentos, recibos e contratos
- ✅ **Filtros Avançados** - Busca por tipo e nome do cliente
- ✅ **Geração de PDF** - Download de documentos em PDF
- ✅ **Progressive Web App (PWA)** - Funciona como app no navegador
- ✅ **Paleta Verde** - Interface moderna e responsiva

## 🔐 Segurança

### Autenticação
- Senha armazenada no **backend C#** (Program.cs)
- **Não aparece** no código-fonte HTML/JavaScript
- **Não aparece** ao inspecionar elemento
- Cookie de sessão **HttpOnly** para prevenir acesso por JavaScript malicioso

### Proteção
- Atributo `[RequireLogin]` protege todas as ações
- `[AllowAnonymous]` apenas para Login e Logout
- Redirecionamento automático para login se sessão expirar

## 🛠️ Tecnologias

- **Framework**: ASP.NET Core (Razor Pages)
- **.NET Target**: .NET 10
- **C# Version**: 14.0
- **Frontend**: Bootstrap 5
- **Funcionalidade PWA**: Service Workers
- **PDF**: jsPDF 2.5.1
- **Banco de Dados**: Local Storage (JSON)

## 📦 Instalação

### Pré-requisitos
- .NET 10 SDK ou superior
- Visual Studio 2024 ou VS Code
- Git

### Passos

1. **Clone o repositório**
```bash
git clone https://github.com/LeonardoLibanori/SistemaLitoralPolimentos.git
cd SistemaLitoralPolimentos
```

2. **Restaure as dependências**
```bash
dotnet restore
```

3. **Execute a aplicação**
```bash
dotnet run
```

4. **Acesse no navegador**
```
https://localhost:5001
```

## 🔑 Credenciais de Acesso

**Senha**: `Leo1212`

> ⚠️ **Nota**: A senha está armazenada no backend (`Program.cs` / `HomeController.cs`) e não é visível no código-fonte público.

## 📁 Estrutura do Projeto

```
SistemaLitoralPolimentos/
├── Controllers/
│   └── HomeController.cs          # Controlador principal com ações de login
├── Views/
│   ├── Home/
│   │   ├── Index.cshtml           # Página inicial
│   │   ├── Login.cshtml           # Tela de autenticação
│   │   ├── Historico.cshtml       # Histórico de documentos
│   │   ├── _Orcamento.cshtml      # Parcial de orçamentos
│   │   ├── _Recibo.cshtml         # Parcial de recibos
│   │   └── _Contrato.cshtml       # Parcial de contratos
│   └── Shared/
│       └── _Layout.cshtml         # Layout principal
├── Attributes/
│   └── RequireLoginAttribute.cs    # Filtro de autenticação
├── wwwroot/
│   ├── css/
│   │   └── site.css               # Estilos personalizados
│   ├── js/
│   │   └── site.js                # Scripts do site
│   ├── sw.js                       # Service Worker (PWA)
│   ├── manifest.json              # Manifesto PWA
│   └── icon-192.png               # Ícone da aplicação
├── Program.cs                      # Configuração da aplicação
└── README.md                       # Este arquivo
```

## 🎨 Paleta de Cores

- **Primária**: Verde `#27ae60`
- **Secundária**: Verde Escuro `#229954`
- **Destaque**: Verde Claro `#2ecc71`
- **Fundo**: Branco `#f8f9fa`

## 🚀 Funcionalidades

### Login e Autenticação
- Tela de login obrigatória na primeira página
- Validação de senha no backend
- Sessão segura com HttpOnly Cookie

### Histórico de Documentos
- Listagem de documentos com filtros
- Busca por tipo de documento (Orçamento, Recibo, Contrato)
- Busca por nome do cliente
- Exportação para PDF

### Gerenciamento de Sessão
- Timeout de 30 minutos de inatividade
- Botão "Sair" para logout imediato
- Limpeza automática de sessão

## 📱 PWA (Progressive Web App)

A aplicação funciona como um Progressive Web App:
- Pode ser instalada como app no navegador
- Funciona offline com Service Worker
- Ícone customizado na tela inicial

## 🐛 Relatório de Bugs

Se encontrar algum bug, abra uma [Issue](https://github.com/LeonardoLibanori/SistemaLitoralPolimentos/issues) no repositório.

## 📄 Licença

Este projeto é privado para uso interno da Litoral Polimentos.

## 👤 Autor

**Leonardo Libanori**
- GitHub: [@LeonardoLibanori](https://github.com/LeonardoLibanori)
- Email: leonardo@litoralpolimentos.com

---

**Desenvolvido com ❤️ para Litoral Polimentos**

Última atualização: 2024
