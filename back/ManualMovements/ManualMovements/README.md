# ManualMovements

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/9.0)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)
[![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen.svg)]

Um template robusto e moderno para microserviços em .NET 9, seguindo as melhores práticas de Clean Architecture, CQRS e Domain-Driven Design.

## 🚀 Características Principais

### Arquitetura

- **Clean Architecture**: Separação clara de responsabilidades entre camadas
- **CQRS Pattern**: Separação de comandos (Commands) e consultas (Queries)
- **Domain-Driven Design**: Foco no domínio de negócio
- **SOLID Principles**: Código limpo e manutenível

### Tecnologias e Padrões

- **.NET 9**: Framework mais recente da Microsoft
- **Entity Framework Core**: ORM para acesso a dados
- **MediatR**: Implementação do padrão mediator
- **FluentValidation**: Validação robusta de dados
- **AutoMapper**: Mapeamento de objetos
- **Swagger/OpenAPI**: Documentação automática da API

### Observabilidade e Monitoramento

- **Structured Logging**: Logs estruturados com Serilog
- **Health Checks**: Monitoramento de saúde da aplicação
- **Global Exception Handling**: Tratamento centralizado de exceções
- **Correlation IDs**: Rastreamento de requisições

### Segurança e Configuração

- **CORS**: Configuração robusta de Cross-Origin Resource Sharing
- **Configuration Management**: Gerenciamento de configurações por ambiente
- **Localization**: Suporte a múltiplos idiomas

## 📁 Estrutura do Projeto

```text
ManualMovements/
├── src/
│   ├── ManualMovements.Api/           # Camada de apresentação
│   │   ├── Controllers/                     # Controllers da API
│   │   ├── Extensions/                      # Extensões de configuração
│   │   ├── Helper/                          # Classes auxiliares
│   │   ├── Middleware/                      # Middlewares customizados
│   │   └── Program.cs                       # Ponto de entrada
│   │
│   ├── ManualMovements.Application/   # Camada de aplicação
│   │   ├── Commands/                        # Comandos CQRS
│   │   ├── Queries/                         # Consultas CQRS
│   │   ├── Mapping/                         # Configurações do AutoMapper
│   │   └── Validators/                      # Validações FluentValidation
│   │
│   ├── ManualMovements.Domain/        # Camada de domínio
│   │   ├── Entities/                        # Entidades de domínio
│   │   ├── Repositories/                    # Interfaces dos repositórios
│   │   ├── Exceptions/                      # Exceções de domínio
│   │   └── Resources/                       # Recursos de localização
│   │
│   ├── ManualMovements.Infrastructure/# Camada de infraestrutura
│   │   ├── AppDbContext.cs                  # Contexto do Entity Framework
│   │   ├── ContextMappings/                 # Mapeamentos do EF
│   │   ├── Migrations/                      # Migrações do banco
│   │   └── Repositories/                    # Implementações dos repositórios
│   │
│   └── ManualMovements.CrossCutting/  # Camada transversal
│       ├── DependencyInjection/             # Configurações de DI
│       └── Health/                          # Health checks
│
└── test/
    └── ManualMovements.UnitTest/      # Testes unitários
        ├── Api/                             # Testes dos controllers
        ├── Application/                     # Testes da aplicação
        └── Infrastructure/                  # Testes da infraestrutura
```

## 🛠️ Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

## 🚀 Como Usar

### 1. Clone o Repositório

```bash
git clone https://github.com/seu-usuario/ManualMovements.git
cd ManualMovements
```

### 2. Configure o Banco de Dados

1. Atualize a connection string no arquivo `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=MicroServiceTemplate;Trusted_Connection=true;MultipleActiveResultSets=true"
  }
}
```

2. Execute as migrações:

```bash
dotnet ef database update --project src/ManualMovements.Infrastructure --startup-project src/ManualMovements.Api
```

### 3. Execute o Projeto

```bash
# Restaurar dependências
dotnet restore

# Executar testes
dotnet test

# Executar a aplicação
dotnet run --project src/ManualMovements.Api
```

### 4. Acesse a Documentação

- **Swagger UI**: https://localhost:7094/swagger
- **Health Checks**: https://localhost:7094/health

## 📚 Endpoints da API

### Clientes (Customers)

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| GET | `/api/v1/customers` | Lista clientes com paginação |
| GET | `/api/v1/customers/{id}` | Obtém cliente por ID |
| POST | `/api/v1/customers` | Cria novo cliente |
| PUT | `/api/v1/customers/{id}` | Atualiza cliente existente |
| DELETE | `/api/v1/customers/{id}` | Remove cliente |

### Exemplos de Uso

#### Criar Cliente

```bash
curl -X POST "https://localhost:7094/api/v1/customers" \
  -H "Content-Type: application/json" \
  -d '{
    "fullName": "João Silva",
    "email": "joao.silva@email.com",
    "documentNumber": "12345678901",
    "gender": "Male",
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
    "favoriteClub": "Corinthians"
  }'
```

#### Listar Clientes

```bash
curl -X GET "https://localhost:7094/api/v1/customers?page=1&pageSize=10&fullName=João"
```

## 🔧 Configuração

### Variáveis de Ambiente

```bash
# Ambiente
ASPNETCORE_ENVIRONMENT=Development

# Connection String
ConnectionStrings__DefaultConnection=Server=localhost;Database=MicroServiceTemplate;Trusted_Connection=true

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

O projeto utiliza logging estruturado com Serilog:

- **Console**: Logs no console durante desenvolvimento
- **File**: Logs em arquivo com rotação diária
- **SQL Server**: Logs no banco de dados (opcional)

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
docker build -t microservice-template .

# Executar container
docker run -p 7094:7094 -p 5145:5145 microservice-template
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

# Health detalhado
curl https://localhost:7094/health/detailed
```

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

## 🆘 Suporte

- **Documentação**: [Wiki do Projeto](https://github.com/seu-usuario/ManualMovements/wiki)
- **Issues**: [GitHub Issues](https://github.com/seu-usuario/ManualMovements/issues)
- **Discussions**: [GitHub Discussions](https://github.com/seu-usuario/ManualMovements/discussions)

## 🙏 Agradecimentos

- [Microsoft .NET Team](https://github.com/dotnet)
- [MediatR](https://github.com/jbogard/MediatR)
- [FluentValidation](https://github.com/FluentValidation/FluentValidation)
- [AutoMapper](https://github.com/AutoMapper/AutoMapper)
- [Serilog](https://github.com/serilog/serilog)

---

**Desenvolvido com ❤️ pela equipe de desenvolvimento**
