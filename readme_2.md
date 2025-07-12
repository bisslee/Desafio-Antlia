# Desafio Antlia - Solução Modernizada

Este repositório contém a solução para o desafio de backend proposto pela BNP Paribas & Antlia, reimaginado com uma arquitetura moderna, robusta e escalável, utilizando as melhores práticas de desenvolvimento de software atuais.

## 1. Visão Geral do Projeto

O objetivo é desenvolver uma API RESTful completa para o gerenciamento de Movimentos Manuais, com um frontend reativo. A solução foi projetada para demonstrar proficiência em:

- Arquitetura Limpa (Clean Architecture) com Domain-Driven Design (DDD).
- Separação de responsabilidades com o padrão CQRS (Command Query Responsibility Segregation).
- Gerenciamento de banco de dados via código (Code-First) com EF Core Migrations.
- Desenvolvimento de uma interface de usuário (UI) moderna e reativa com Angular.

---

## 2. Stack de Tecnologia

| Camada    | Tecnologia/Padrão                                                              |
| :-------- | :----------------------------------------------------------------------------- |
| **Backend** | .NET 9, ASP.NET Core, C#                                                       |
| **Arquitetura** | Domain-Driven Design (DDD), CQRS, Arquitetura Limpa (Clean Architecture)       |
| **Template Base** | `Biss.Solutions.MicroService.Template.Net9`                                    |
| **Persistência** | Entity Framework Core 8+                                                       |
| **Banco de Dados** | SQL Server (Schema gerenciado 100% via EF Core Migrations)                     |
| **Frontend** | Angular 17+, TypeScript                                                        |
| **UI Kit (Frontend)** | Angular Material                                                               |
| **Estilo (Frontend)** | SCSS                                                                           |

---

## 3. Plano de Execução - Backend

A solução backend é dividida em quatro projetos principais, seguindo os princípios da Arquitetura Limpa.

### 3.1. Estrutura dos Projetos

1. **`AntliaChallenge.Domain`**: O núcleo da aplicação. Contém as entidades, a lógica de negócio pura, interfaces de repositórios e serviços de domínio. Não depende de nenhuma outra camada.
2. **`AntliaChallenge.Application`**: A camada de orquestração. Implementa o padrão CQRS com `Commands` (para escrita) e `Queries` (para leitura). Depende do `Domain`.
3. **`AntliaChallenge.Infrastructure`**: A camada de detalhes técnicos. Implementa os repositórios (acesso a dados com EF Core), serviços de email, etc. Depende do `Application`.
4. **`AntliaChallenge.Api`**: A camada de apresentação. Expõe os endpoints RESTful, lida com requisições/respostas HTTP e delega o trabalho para a camada `Application`.

### 3.2. Modelagem e Mapeamento do Banco (Code-First)

As entidades são definidas em C# e mapeadas para o schema do banco de dados especificado no PDF usando o Fluent API do EF Core.

#### Tabela: `PRODUTO` (Entidade: `Product`)

| Coluna (BD) | Propriedade (C#) | Tipo (SQL) | Tipo (C#) | Restrições |
| :--- | :--- | :--- | :--- | :--- |
| `COD_PRODUTO` | `ProductCode` | `char(4)` | `string` | PK, Required |
| `DES_PRODUTO` | `Description` | `varchar(30)` | `string` | Nullable |
| `STA_STATUS` | `Status` | `char(1)` | `string` | Nullable |

#### Tabela: `PRODUTO_COSIF` (Entidade: `ProductCosif`)

| Coluna (BD) | Propriedade (C#) | Tipo (SQL) | Tipo (C#) | Restrições |
| :--- | :--- | :--- | :--- | :--- |
| `COD_PRODUTO` | `ProductCode` | `char(4)` | `string` | PK, FK (PRODUTO), Required |
| `COD_COSIF` | `CosifCode` | `char(11)` | `string` | PK, Required |
| `COD_CLASSIFICACAO` | `ClassificationCode` | `char(6)` | `string` | Nullable |
| `STA_STATUS` | `Status` | `char(1)` | `string` | Nullable |

#### Tabela: `MOVIMENTO_MANUAL` (Entidade: `ManualMovement`)

| Coluna (BD) | Propriedade (C#) | Tipo (SQL) | Tipo (C#) | Restrições |
| :--- | :--- | :--- | :--- | :--- |
| `DAT_MES` | `Month` | `numeric(2, 0)` | `int` | PK, Required |
| `DAT_ANO` | `Year` | `numeric(4, 0)` | `int` | PK, Required |
| `NUM_LANCAMENTO` | `LaunchNumber` | `numeric(18, 0)` | `int` | PK, Required |
| `COD_PRODUTO` | `ProductCode` | `char(4)` | `string` | PK, FK (PRODUTO_COSIF), Required |
| `COD_COSIF` | `CosifCode` | `char(11)` | `string` | PK, FK (PRODUTO_COSIF), Required |
| `DES_DESCRICAO` | `Description` | `varchar(50)` | `string` | Required |
| `DAT_MOVIMENTO` | `MovementDate` | `smalldatetime` | `DateTime` | Required |
| `COD_USUARIO` | `UserCode` | `varchar(15)` | `string` | Required |
| `VAL_VALOR` | `Value` | `numeric(18, 2)` | `decimal` | Required |

### 3.3. Stored Procedure

A Stored Procedure `PROC_GET_MOVEMENTS` será criada e gerenciada através de uma **Migration do EF Core**, garantindo que todo o schema do banco esteja versionado junto com o código-fonte. Ela será otimizada para leitura e consumida pela `Query` correspondente no CQRS.

### 3.4. Endpoints da API

| Verbo | Rota | Descrição |
| :--- | :--- | :--- |
| `GET` | `/api/movements` | Retorna a lista de movimentos manuais, consumindo a Stored Procedure. |
| `POST` | `/api/movements` | Cria um novo movimento manual. |
| `GET` | `/api/products` | Retorna a lista de produtos para preencher o combo do formulário. |
| `GET` | `/api/products/{productCode}/cosifs` | Retorna os COSIFs de um produto específico. |

---

## 4. Plano de Execução - Frontend (Angular)

A interface será uma Single Page Application (SPA) desenvolvida em Angular, com foco em reatividade e experiência do usuário.

### 4.1 Descrição dos Elementos na Tela

1. **Título Principal**: "Movimentos Manuais", deixando claro o propósito da página.
2. **Seção do Formulário** (MovementFormComponent):
    - Organizado dentro de um card (`<mat-card>`) para uma melhor separação visual.
    - **Campos de Entrada:** Utiliza componentes do Angular Material (`<mat-form-field>`) para uma aparência consistente e profissional.
      - **Mês e Ano:** Campos de texto numéricos, lado a lado para otimizar o espaço.
      - **Produto e COSIF**: Componentes de seleção (`<mat-select>`), que são os "ComboBox" modernos. O campo COSIF ficaria desabilitado até que um Produto seja selecionado.
      - **Valor:** Campo numérico, idealmente com uma máscara de moeda para facilitar a digitação.
      - **Descrição:** Uma área de texto (`<textarea>`) para permitir descrições mais longas.
    - **Botões de Ação:**
      - **Limpar:** Botão secundário (estilo stroked) para limpar os campos do formulário.
      - **Novo:** Botão de destaque (flat ou stroked com cor primária) para habilitar o formulário para um novo lançamento.
      - **Incluir:** Botão principal (raised com cor primary) para submeter o formulário. Ele ficaria desabilitado até que o formulário esteja válido.
3. **Seção da Grade de Dados (`MovementGridComponent`):**
    - **Tabela Moderna:** Utiliza o componente `<mat-table>` do Angular Material, que oferece recursos como ordenação, paginação e uma aparência limpa.
    - **Colunas:** Exibe exatamente os dados retornados pela Stored Procedure, conforme especificado no desafio: Mês, Ano, Cód. Produto, Desc. Produto, NR Lançamento, Descrição e Valor.
    - **Feedback Visual:** A tabela pode incluir um spinner de carregamento (`<mat-progress-spinner>`) enquanto os dados estão sendo buscados na API, melhorando a experiência do usuário.

### 4.2. Estrutura dos Componentes

- **`MovementMainComponent`**: Componente "inteligente" (smart component) que orquestra a página, gerencia o estado e a comunicação com os serviços.
- **`MovementFormComponent`**: Componente "burro" (dumb component) que apenas exibe o formulário, emite eventos e recebe dados via `@Input`. Utiliza `ReactiveFormsModule` para validações.
- **`MovementGridComponent`**: Componente "burro" que exibe os dados em uma tabela do Angular Material (`<mat-table>`). Recebe a lista de movimentos via `@Input`.

### 4.3. Serviços

- **`MovementService`**: Encapsula toda a lógica de comunicação com a API para obter e criar movimentos.
- **`ProductService`**: Responsável por buscar os dados de Produtos e COSIFs para popular os `selects` do formulário.

### 4.4. Fluxo de Interação do Usuário

1. A página é carregada e o `MovementMainComponent` solicita ao `MovementService` a lista inicial de movimentos, que é exibida na `MovementGridComponent`.
2. O usuário clica no botão "Novo" no `MovementFormComponent`, que habilita os campos do formulário.
3. Os combos de "Produto" e "Cosif" são preenchidos através de chamadas ao `ProductService`.
4. O usuário preenche o formulário e clica em "Incluir".
5. O `MovementFormComponent` emite um evento com os dados do formulário.
6. O `MovementMainComponent` captura o evento, chama o `MovementService` para enviar os dados à API.
7. Após o sucesso da criação, o `MovementService` notifica o `MovementMainComponent`, que solicita a atualização da lista de movimentos.
8. A `MovementGridComponent` é atualizada automaticamente com o novo registro.
9. O formulário é limpo e desabilitado.

---

## 5. Como Executar o Projeto

### Pré-requisitos

- .NET SDK 9 ou superior
- Node.js e Angular CLI
- SQL Server (instância local ou Docker)

### Backend

1. Navegue até a pasta `src/AntliaChallenge.Api`.
2. Atualize a `DefaultConnection` no arquivo `appsettings.Development.json`.
3. Execute `dotnet ef database update` para criar o banco e aplicar as migrations.
4. Execute `dotnet run` para iniciar a API.

### Frontend

1. Navegue até a pasta do projeto Angular.
2. Execute `npm install` para instalar as dependências.
3. Execute `ng serve` para iniciar a aplicação.
4. Acesse `http://localhost:4200` no seu navegador.
