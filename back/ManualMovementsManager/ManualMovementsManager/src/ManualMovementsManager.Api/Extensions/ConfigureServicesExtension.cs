using Biss.MultiSinkLogger.ExceptionHandlers;
using Biss.MultiSinkLogger.Http;
using ManualMovementsManager.Api.Helper;
using ManualMovementsManager.CrossCutting.DependencyInjection;
using ManualMovementsManager.Infrastructure;
using ManualMovementsManager.Infrastructure.Serialization;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Globalization;
using System.Reflection;

namespace ManualMovementsManager.Api.Extensions
{
    public static class ConfigureServicesExtension
    {
        public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHttpClient();

            var connStr = configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connStr));



            services.AddTransient<HttpLoggingHandler>();
            services.AddTransient<IExceptionHandler, DefaultExceptionHandler>();

            services.AddAutoMapper();
            services.AddMediator();
            services.AddRepository();
            services.AddValidators();
            services.ConfigureLogging(configuration);
            services.AddApplicationServices();
            services.AddHealthChecksInjection();
            

            services.AddLocalization(options => options.ResourcesPath = "Resources");

            services.Configure<RequestLocalizationOptions>(options =>
            {
                var supportedCultures = new[] { "en-US", "pt-BR", "es" };
                options.DefaultRequestCulture = new RequestCulture("pt-BR");
                options.SupportedCultures = supportedCultures.Select(c => new CultureInfo(c)).ToList();
                options.SupportedUICultures = options.SupportedCultures;
            });

            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    var jsonOptions = JsonSerializationConfig.CreateApiOptions();
                    options.JsonSerializerOptions.PropertyNamingPolicy = jsonOptions.PropertyNamingPolicy;
                    options.JsonSerializerOptions.WriteIndented = jsonOptions.WriteIndented;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = jsonOptions.DefaultIgnoreCondition;
                    foreach (var converter in jsonOptions.Converters)
                    {
                        options.JsonSerializerOptions.Converters.Add(converter);
                    }
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
                });

            services.AddEndpointsApiExplorer();
            services.ConfigureSwagger();

            // Configuração robusta do CORS
            services.ConfigureCors(configuration);

            return services;
        }

        private static void ConfigureSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ManualMovementsManager API",
                    Version = "v1.0.0",
                    Description = @"
## ManualMovementsManager API

Esta é uma API de template para microserviços desenvolvida em .NET 9, seguindo as melhores práticas de Clean Architecture.

### Características Principais:
- **Clean Architecture**: Separação clara de responsabilidades
- **CQRS Pattern**: Separação de comandos e consultas
- **MediatR**: Implementação do padrão mediator
- **FluentValidation**: Validação robusta de dados
- **AutoMapper**: Mapeamento de objetos
- **Structured Logging**: Logs estruturados com Serilog
- **Global Exception Handling**: Tratamento centralizado de exceções
- **Health Checks**: Monitoramento de saúde da aplicação
- **CORS**: Configuração robusta de Cross-Origin Resource Sharing

### Exemplos de Uso:
Consulte a documentação de cada endpoint para exemplos detalhados de requisição e resposta.
                    ",
                    Contact = new OpenApiContact
                    {
                        Name = "Development Team",
                        Email = "ivana@biss.com.br",
                        Url = new Uri("https://biss.com.br")
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License",
                        Url = new Uri("https://opensource.org/licenses/MIT")
                    },
                    TermsOfService = new Uri("https://biss.com.br/terms")
                });

                // Configurar autenticação JWT (preparado para futuras implementações)
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Incluir comentários XML se existirem
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }

                // Configurar filtros
                c.OperationFilter<AddAcceptLanguageHeaderOperationFilter>();
                c.EnableAnnotations();

                // Configurar esquemas personalizados
                c.CustomSchemaIds(type => type.Name);

                // Configurar respostas padrão
                c.MapType<DateTime>(() => new OpenApiSchema { Type = "string", Format = "date-time" });
                c.MapType<DateTime?>(() => new OpenApiSchema { Type = "string", Format = "date-time", Nullable = true });

                // Configurar tags para organizar endpoints
                c.TagActionsBy(api =>
                {
                    if (api.GroupName != null)
                    {
                        return new[] { api.GroupName.ToString() };
                    }

                    var controllerActionDescriptor = api.ActionDescriptor as Microsoft.AspNetCore.Mvc.Controllers.ControllerActionDescriptor;
                    if (controllerActionDescriptor != null)
                    {
                        return new[] { controllerActionDescriptor.ControllerName };
                    }

                    throw new InvalidOperationException("Unable to determine tag for endpoint.");
                });

                c.DocInclusionPredicate((name, api) => true);
            });
        }

        private static void ConfigureCors(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                // Política para desenvolvimento
                options.AddPolicy("DevelopmentPolicy", builder =>
                {
                    builder
                        .WithOrigins(
                            "http://localhost:3000",
                            "http://localhost:3001",
                            "http://localhost:4200",
                            "http://localhost:8080",
                            "https://localhost:3000",
                            "https://localhost:3001",
                            "https://localhost:4200",
                            "https://localhost:8080"
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders(
                            "X-Total-Count",
                            "X-Pagination",
                            "X-Correlation-Id",
                            "X-Trace-Id"
                        )
                        .SetIsOriginAllowedToAllowWildcardSubdomains();
                });

                // Política para produção (mais restritiva)
                options.AddPolicy("ProductionPolicy", builder =>
                {
                    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>();
                    
                    if (allowedOrigins.Any())
                    {
                        builder.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders(
                                "X-Total-Count",
                                "X-Pagination",
                                "X-Correlation-Id",
                                "X-Trace-Id"
                            );
                    }
                    else
                    {
                        // Fallback para desenvolvimento se não configurado
                        builder
                            .WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .WithExposedHeaders(
                                "X-Total-Count",
                                "X-Pagination",
                                "X-Correlation-Id",
                                "X-Trace-Id"
                            );
                    }
                });

                // Política para APIs públicas (menos restritiva)
                options.AddPolicy("PublicApiPolicy", builder =>
                {
                    builder
                        .AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithExposedHeaders(
                            "X-Total-Count",
                            "X-Pagination",
                            "X-Correlation-Id",
                            "X-Trace-Id"
                        );
                });
            });
        }
    }
}
