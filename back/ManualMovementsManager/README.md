# Desafio para Dev .net: Manual Movements Manager

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)]

Sistema criado para o desafio para vaga de dev .net, robusto e moderno para gerenciamento de movimentos manuais desenvolvido em .NET 9, seguindo as melhores práticas de Clean Architecture, CQRS e Domain-Driven Design.

## 🎯 Sobre o Projeto

Este sistema foi desenvolvido como parte de um desafio técnico para vaga de desenvolvedor .NET, utilizando como base o template oficial da **Biss Solutions** para microserviços em .NET 9.

### 🏗️ Template Base Utilizado

O projeto foi criado utilizando o template oficial:

```bash
dotnet new install Biss.Solutions.MicroService.Template.Net9::2.0.0
```

**Template**: [Biss.Solutions.MicroService.Template.Net9 v2.0.0](https://www.nuget.org/packages/Biss.Solutions.MicroService.Template.Net9/2.0.0)

**Criado por**: [Biss Solutions](https://biss.com.br/)

O template fornece uma estrutura robusta e moderna para microserviços, incluindo:
- Clean Architecture com separação clara de responsabilidades
- CQRS Pattern com MediatR para comandos e consultas
- Domain-Driven Design com foco no domínio de negócio
- Specification Pattern para validação de regras de negócio
- Repository Pattern com interfaces genéricas
- SOLID Principles aplicados em todo o código
- Observabilidade completa com logging estruturado e health checks
- Testes unitários organizados por camada

### 🎨 Adaptações Realizadas

Sobre a base do template, foram implementadas as seguintes adaptações específicas para o domínio de movimentos manuais:

- **Entidades de Domínio**: ManualMovement, Product, ProductCosif, Customer, Address
- **Controllers Específicos**: Endpoints para gerenciamento de movimentos manuais
- **Validações de Negócio**: Regras específicas para o contexto financeiro
- **Relacionamentos**: Mapeamentos entre produtos, COSIF e movimentos
- **Documentação**: Exemplos e documentação específica do domínio

## 🚀 Características Principais

### Arquitetura

- **Clean Architecture**: Separação clara de responsabilidades entre camadas
- **CQRS Pattern**: Separação de comandos (Commands) e consultas (Queries)
- **Domain-Driven Design**: Foco no domínio de movimentos manuais
- **SOLID Principles**: Código limpo e manutenível

### Tecnologias e Padrões

- **.NET 9**: Framework mais recente da Microsoft
- **Entity Framework Core**: ORM para acesso a dados
- **MediatR**: Implementação do padrão mediator
- **FluentValidation**: Validação robusta de dados
- **AutoMapper**: Mapeamento de objetos
- **Swagger/OpenAPI**: Documentação automática da API
- **Biss.MultiSinkLogger**: Sistema de logging estruturado

### Observabilidade e Monitoramento

- **Structured Logging**: Logs estruturados com Serilog e Biss.MultiSinkLogger
- **Health Checks**: Monitoramento detalhado de saúde da aplicação
- **Global Exception Handling**: Tratamento centralizado de exceções
- **Correlation IDs**: Rastreamento de requisições

### Segurança e Configuração

- **CORS**: Configuração robusta de Cross-Origin Resource Sharing
- **Configuration Management**: Gerenciamento de configurações por ambiente
- **Localization**: Suporte a múltiplos idiomas (pt-BR, en-US, es)

## 📁 Estrutura do Projeto

```text
ManualMovementsManager/
├── src/
│   ├── ManualMovementsManager.Api/           # Camada de apresentação
│   │   ├── Controllers/                     # Controllers da API
│   │   ├── Extensions/                      # Extensões de configuração
│   │   ├── Helper/                          # Classes auxiliares
│   │   ├── Middleware/                      # Middlewares customizados
│   │   └── Program.cs                       # Ponto de entrada
│   │
│   ├── ManualMovementsManager.Application/   # Camada de aplicação
│   │   ├── Commands/                        # Comandos CQRS
│   │   ├── Queries/                         # Consultas CQRS
│   │   ├── Mapping/                         # Configurações do AutoMapper
│   │   └── Validators/                      # Validações FluentValidation
│   │
│   ├── ManualMovementsManager.Domain/        # Camada de domínio
│   │   ├── Entities/                        # Entidades de domínio
│   │   ├── Repositories/                    # Interfaces dos repositórios
│   │   ├── Exceptions/                      # Exceções de domínio
│   │   └── Resources/                       # Recursos de localização
│   │
│   ├── ManualMovementsManager.Infrastructure/# Camada de infraestrutura
│   │   ├── AppDbContext.cs                  # Contexto do Entity Framework
│   │   ├── ContextMappings/                 # Mapeamentos do EF
│   │   ├── Migrations/                      # Migrações do banco
│   │   └── Repositories/                    # Implementações dos repositórios
│   │
│   └── ManualMovementsManager.CrossCutting/  # Camada transversal
│       ├── DependencyInjection/             # Configurações de DI
│       └── Health/                          # Health checks
│
└── test/
    └── ManualMovementsManager.UnitTest/      # Testes unitários
        ├── Api/                             # Testes dos controllers
        ├── Application/                     # Testes da aplicação
        └── Infrastructure/                  # Testes da infraestrutura
```

## 🏢 Domínio de Negócio

O sistema gerencia **movimentos manuais** em um contexto financeiro, incluindo:

### Entidades Principais

- **ManualMovement**: Movimentos manuais com mês, ano, número do lançamento, produto, COSIF, descrição, data, usuário e valor
- **Product**: Produtos com código e descrição
- **ProductCosif**: Produtos COSIF com código do produto, código COSIF e código de classificação
- **Customer**: Clientes com dados pessoais, endereço e preferências
- **Address**: Endereços dos clientes

### Relacionamentos

- Um **Product** pode ter múltiplos **ProductCosif**
- Um **Product** pode ter múltiplos **ManualMovement**
- Um **ProductCosif** pode ter múltiplos **ManualMovement**
- Um **Customer** tem um **Address**

## 🛠️ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

## 🚀 Como Usar

### 1. Clone o Repositório

```bash
git clone https://github.com/bisslee/Desafio-Antlia.git
cd ManualMovementsManager
```

### 2. Configure o Banco de Dados

1. Atualize a connection string no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=ManualMovementsManager;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. Execute as migrações

```bash
dotnet ef database update --project src/ManualMovementsManager.Infrastructure --startup-project src/ManualMovementsManager.Api
```

### 3. Execute o Projeto

```bash
# Restaurar dependências
dotnet restore

# Executar testes
dotnet test

# Executar a aplicação
dotnet run --project src/ManualMovementsManager.Api
```

### 4. Acesse a Documentação

- **Swagger UI**: https://localhost:7094/swagger
- **Health Checks**: https://localhost:7094/health

## 📚 Endpoints da API

### Movimentos Manuais (ManualMovements)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/manualmovement` | Lista movimentos manuais com paginação |
| GET | `/api/v1/manualmovement/{id}` | Obtém movimento manual por ID |
| POST | `/api/v1/manualmovement` | Cria novo movimento manual |
| PUT | `/api/v1/manualmovement/{id}` | Atualiza movimento manual existente |
| DELETE | `/api/v1/manualmovement/{id}` | Remove movimento manual |

### Produtos (Products)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/product` | Lista produtos com paginação |
| GET | `/api/v1/product/{id}` | Obtém produto por ID |
| POST | `/api/v1/product` | Cria novo produto |
| PUT | `/api/v1/product/{id}` | Atualiza produto existente |
| DELETE | `/api/v1/product/{id}` | Remove produto |

### Produtos COSIF (ProductCosifs)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/productcosif` | Lista produtos COSIF com paginação |
| GET | `/api/v1/productcosif/{id}` | Obtém produto COSIF por ID |
| POST | `/api/v1/productcosif` | Cria novo produto COSIF |
| PUT | `/api/v1/productcosif/{id}` | Atualiza produto COSIF existente |
| DELETE | `/api/v1/productcosif/{id}` | Remove produto COSIF |

### Clientes (Customers)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/customer` | Lista clientes com paginação |
| GET | `/api/v1/customer/{id}` | Obtém cliente por ID |
| POST | `/api/v1/customer` | Cria novo cliente |
| PUT | `/api/v1/customer/{id}` | Atualiza cliente existente |
| DELETE | `/api/v1/customer/{id}` | Remove cliente |

## 📝 Exemplos de Uso

### Criar Movimento Manual

```bash
curl -X POST "https://localhost:7094/api/v1/manualmovement" \
  -H "Content-Type: application/json" \
  -d '{
    "month": 12,
    "year": 2024,
    "launchNumber": 1,
    "productCode": "PROD001",
    "cosifCode": "COSIF001",
    "description": "Movimento manual de teste",
    "movementDate": "2024-12-01T10:00:00Z",
    "userCode": "USER001",
    "value": 1000.50
  }'
```

### Criar Produto

```bash
curl -X POST "https://localhost:7094/api/v1/product" \
  -H "Content-Type: application/json" \
  -d '{
    "productCode": "PROD001",
    "description": "Produto de teste"
  }'
```

### Criar Produto COSIF

```bash
curl -X POST "https://localhost:7094/api/v1/productcosif" \
  -H "Content-Type: application/json" \
  -d '{
    "productCode": "PROD001",
    "cosifCode": "COSIF001",
    "classificationCode": "CLASS001"
  }'
```

### Criar Cliente

```bash
curl -X POST "https://localhost:7094/api/v1/customer" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "João Silva",
    "email": "joao.silva@email.com",
    "documentNumber": "12345678901",
    "gender": "Male",
    "birthDate": "1990-01-01T00:00:00Z",
    "phone": "(11) 99999-9999",
    "address": {
      "street": "Rua das Flores",
      "number": "123",
      "complement": "Apto 45",
      "neighborhood": "Centro",
      "city": "São Paulo",
      "state": "SP",
      "country": "Brasil",
      "zipCode": "01234-567"
    },
    "favoriteSport": "Futebol",
    "favoriteClub": "Corinthians",
    "acceptTermsUse": true,
    "acceptPrivacyPolicy": true
  }'
```

### Listar Movimentos Manuais

```bash
curl -X GET "https://localhost:7094/api/v1/manualmovement?page=1&pageSize=10&description=teste"
```

## 🔧 Configuração

### Variáveis de Ambiente

```bash
# Ambiente
ASPNETCORE_ENVIRONMENT=Development

# Connection String
ConnectionStrings__DefaultConnection=Server=localhost;Database=ManualMovementsManager;Trusted_Connection=true

# CORS (Produção)
Cors__AllowedOrigins__0=https://yourdomain.com
Cors__AllowedOrigins__1=https://www.yourdomain.com
```

### Health Checks

O projeto inclui health checks detalhados:

- **API Health**: Verifica a saúde geral da aplicação
- **Database Health**: Verifica conectividade e migrações do banco
- **External Dependencies**: Verifica dependências externas

Acesse: `https://localhost:7094/health`

### Logging

O projeto utiliza logging estruturado com Biss.MultiSinkLogger:

- **Console**: Logs no console durante desenvolvimento
- **File**: Logs em arquivo com rotação diária
- **SQL Server**: Logs no banco de dados (opcional)

Configuração no `appsettings.json`:

```json
{
  "BissMultiSinkLogger": {
    "MinimumLevel": "Information",
    "Sinks": [
      {
        "Type": "Console",
        "Enabled": true
      },
      {
        "Type": "File",
        "Enabled": true,
        "Path": "logs/manualmovementsmanager-.log",
        "RollingInterval": "Day",
        "RetainedFileCountLimit": 30
      }
    ],
    "Enrichment": {
      "IncludeEnvironment": true,
      "IncludeApplicationName": true,
      "IncludeVersion": true,
      "IncludeCorrelationId": true
    }
  }
}
```

## 🧪 Testes

### Executar Todos os Testes

```bash
dotnet test
```

### Executar Testes Específicos

```bash
# Testes da API
dotnet test --filter "Category=Api"

# Testes da aplicação
dotnet test --filter "Category=Application"

# Testes da infraestrutura
dotnet test --filter "Category=Infrastructure"
```

### Cobertura de Código

```bash
dotnet test --collect:"XPlat Code Coverage"
```

## 📦 Deploy

### Docker

```bash
# Build da imagem
docker build -t manualmovementsmanager .

# Executar container
docker run -p 7094:7094 -p 5145:5145 manualmovementsmanager
```

### Azure

```bash
# Publicar no Azure App Service
dotnet publish -c Release
az webapp deployment source config-zip --resource-group myResourceGroup --name myAppName --src publish.zip
```

## 🔒 Segurança

### CORS

O projeto inclui configurações CORS robustas:

- **Development**: Permite origens locais para desenvolvimento
- **Production**: Restringe a origens configuradas
- **Public API**: Política menos restritiva para APIs públicas

### Validação

- **FluentValidation**: Validação robusta de dados de entrada
- **Model Validation**: Validação automática de modelos
- **Custom Validators**: Validadores customizados para regras de negócio

## 🌐 Internacionalização

O projeto suporta múltiplos idiomas:

- **Português (Brasil)**: Padrão
- **Inglês (EUA)**: Suporte completo
- **Espanhol**: Suporte básico

Configure o header `Accept-Language` para usar diferentes idiomas.

## 📈 Monitoramento

### Health Checks

```bash
# Health geral
curl https://localhost:7094/health

### Logs

Os logs incluem:

- **Correlation ID**: Rastreamento de requisições
- **Structured Data**: Dados estruturados para análise
- **Performance Metrics**: Métricas de performance
- **Error Tracking**: Rastreamento de erros

## 🤝 Contribuindo

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

## 🙏 Agradecimentos

- [Microsoft .NET Team](https://github.com/dotnet)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [AutoMapper](https://github.com/AutoMapper/AutoMapper)
- [Serilog](https://github.com/serilog/serilog)
- [Biss.MultiSinkLogger](https://github.com/biss/multisinklogger)

---

**Desenvolvido pela Ivana **
