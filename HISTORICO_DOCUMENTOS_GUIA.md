# 📋 HISTÓRICO DE DOCUMENTOS - GUIA DE FUNCIONALIDADES

## ✨ O que foi implementado

Uma nova página **"Histórico"** foi adicionada ao sistema com as seguintes funcionalidades:

### 1. **Acesso ao Histórico**
- Novo botão "📋 HISTÓRICO" no menu principal
- Ao clicar, abre uma página dedicada com todas as funcionalidades de histórico

### 2. **Armazenamento de Documentos**
Todos os documentos criados (Orçamento, Recibo e Contrato) são automaticamente salvos com:
- **Cliente/Contratante**: Nome da pessoa
- **Valor**: Valor total
- **Data**: Data de criação automática
- **Serviço/Descrição**: Resumo do serviço (até 100 caracteres)
- **ID único**: Identificador para operações

### 3. **Filtros e Busca**

#### Tipo de Documento
- Dropdown para filtrar por: Todos, Orçamento, Recibo, Contrato

#### Busca por Nome
- Campo de texto para buscar por nome do cliente/contratante
- Busca em tempo real (conforme digita)

#### Busca por Data
- Seletor de data para filtrar documentos por dia específico

### 4. **Abas de Visualização**
Três abas separadas para melhor organização:

- **Orçamentos**: Tabela azul com todos os orçamentos registrados
- **Recibos**: Tabela verde com todos os recibos registrados
- **Contratos**: Tabela amarela com todos os contratos registrados

### 5. **Tabelas com Informações**
Cada tabela exibe:
| Coluna | Descrição |
|--------|-----------|
| Cliente | Nome do cliente/contratante |
| Valor | Valor do documento |
| Data | Data de criação (formato: DD/MM/YYYY) |
| Serviço | Resumo do serviço |
| Ações | Botões de Restaurar e Deletar |

### 6. **Ações por Documento**

#### 📋 Restaurar
- Clique em "Restaurar" para preencher automaticamente um formulário
- O documento será restaurado na página correspondente (Orçamento/Recibo/Contrato)
- Você pode editar e gerar novamente se necessário

#### 🗑️ Deletar
- Remove o documento do histórico permanentemente
- Requer confirmação antes de deletar

### 7. **Operações em Lote**

#### 🗑️ Limpar Histórico Completo
- Deleta TODOS os documentos salvos
- Requer confirmação dupla
- ⚠️ Esta ação é irreversível

#### 📥 Exportar Histórico (JSON)
- Baixa um arquivo `.json` com todo o histórico
- Formato: `historico-documentos-DD-MM-YYYY.json`
- Útil para backup ou transferência de dados

### 8. **Armazenamento**
- Dados salvos no **LocalStorage do navegador**
- Persiste mesmo após fechar e reabrir o navegador
- Cada documento tem um ID único (timestamp)

## 🔄 Como Funciona

### Criação automática de histórico:
1. Usuário preenche formulário (Orçamento/Recibo/Contrato)
2. Clica em "Gerar PDF"
3. PDF é aberto/baixado
4. **Documento é salvo automaticamente no histórico** 🎉

### Visualização:
1. Clique em "📋 HISTÓRICO" no menu
2. Use os filtros (tipo, nome, data) conforme necessário
3. Visualize os documentos em abas separadas
4. Clique em "Restaurar" para editar ou em "Deletar" para remover

### Restauração:
1. Clique em "📋 Restaurar" em um documento
2. Formulário será preenchido automaticamente
3. Você é levado para a página do documento
4. Pode editar e gerar novo PDF se necessário

## 💾 Estrutura de Armazenamento

O histórico é salvo como JSON com a seguinte estrutura:

```json
{
  "orcamentos": [
    {
      "id": 1234567890,
      "data": "2024-01-15T10:30:00.000Z",
      "cliente": "João Silva",
      "valor": "R$ 1.500,00",
      "descricao": "Polimento de mármore..."
    }
  ],
  "recibos": [...],
  "contratos": [...]
}
```

## ⚙️ Detalhes Técnicos

- **Linguagem**: JavaScript puro
- **Armazenamento**: LocalStorage (navegador)
- **Limite**: Depende do navegador (geralmente 5-10MB)
- **Sincronização**: Automática entre abas
- **Banco de Dados**: Client-side (sem servidor)

## 📱 Responsivo

- Design adaptado para desktop e mobile
- Filtros em layout responsivo
- Tabelas com scroll horizontal em mobile
- Botões otimizados para touch

## 🎯 Benefícios

✅ Nunca perca um documento novamente  
✅ Busca rápida por cliente ou data  
✅ Restaure documentos antigos facilmente  
✅ Backup automático em JSON  
✅ Sem necessidade de servidor  
✅ Totalmente privado (dados no seu navegador)  

---

**Versão**: 1.0  
**Data**: Janeiro 2024
