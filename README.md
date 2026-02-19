# Sistema de Emissão de Notas Fiscais

Aplicação Full Stack para gerenciamento e emissão de notas fiscais, desenvolvida com **Angular** (Frontend) e **.NET 8** (Backend) em arquitetura de microsserviços.

## 🚀 Tecnologias Utilizadas

### Frontend
-   **Angular 17+**
-   **Angular Material** (UI Components)
-   **RxJS** (Reatividade)
-   **TypeScript**

### Backend
-   **.NET 8 (C#)**
-   **Entity Framework Core**
-   **SQLite** (Banco de dados)
-   **Polly** (Resiliência e Fault Handling)
-   **Swagger** (Documentação da API)

## 🏗 Arquitetura

O sistema é composto por dois microsserviços principais:
1.  **StockService** (`Porta 7002`): Gerencia o cadastro e saldo de produtos.
2.  **BillingService** (`Porta 7001`): Gerencia a emissão de notas fiscais e comunica-se com o StockService.

## ⚙️ Como Executar o Projeto

### Pré-requisitos
-   .NET 8 SDK
-   Node.js (LTS) & NPM
-   Angular CLI

### 1. Backend (Microsserviços)

Abra dois terminais separados para rodar cada serviço:

**Terminal 1 - StockService:**
```bash
cd backend/StockService
dotnet run
```
*Acesse o Swagger em: https://localhost:7002/swagger*

**Terminal 2 - BillingService:**
```bash
cd backend/BillingService
dotnet run
```
*Acesse o Swagger em: https://localhost:7001/swagger*

### 2. Frontend

**Terminal 3:**
```bash
cd frontend/invoice-system
npm install
npm start
```
*Acesse a aplicação em: http://localhost:4200*

## ✨ Funcionalidades

-   **Cadastro de Produtos**: Adição de produtos com controle de estoque.
-   **Emissão de Notas**: Criação de notas fiscais com múltiplos itens.
-   **Impressão de Notas**: Simulação de impressão que fecha a nota e baixa o estoque automaticamente.
-   **Validações**:
    -   Impedir venda de produtos sem estoque.
    -   Bloqueio de edição/exclusão de notas já emitidas.
-   **Resiliência**: Tratamento de erros no frontend e backend (Circuit Breaker) caso um serviço esteja indisponível.

## 🧪 Diferenciais Implementados

-   **Concorrência Otimista**: Controle de versão (`RowVersion`) para evitar conflitos de edição simultânea no estoque.
-   **Tratamento Global de Erros**: Feedback visual amigável (SnackBars) no frontend e respostas padronizadas no backend.
# Korp_Teste_GabrielFonseca
