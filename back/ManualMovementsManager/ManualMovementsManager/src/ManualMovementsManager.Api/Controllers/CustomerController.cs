using ManualMovementsManager.Domain.Entities.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ManualMovementsManager.Api.Helper;
using ManualMovementsManager.Application.Commands.Customers.RemoveCustomer;
using ManualMovementsManager.Application.Commands.Customers.ChangeCustomer;
using ManualMovementsManager.Application.Commands.Customers.AddCustomer;
using ManualMovementsManager.Application.Queries.Customers.GetCustomer;
using ManualMovementsManager.Application.Queries.Customers.GetCustomerByKey;
using Microsoft.Extensions.Logging;

namespace ManualMovementsManager.Api.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar operações relacionadas a clientes
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [SwaggerTag("Gerenciamento de Clientes")]
    public class CustomerController : BaseControllerHandle
    {
        private readonly IMediator Mediator;
        private readonly ILogger<CustomerController> Logger;

        public CustomerController(            
            IMediator mediator,
            ILogger<CustomerController> logger
            ) : base()
        {
            Mediator = mediator;
            Logger = logger;
        }

        /// <summary>
        /// Lista clientes com paginação e filtros opcionais
        /// </summary>
        /// <param name="request">Parâmetros de consulta incluindo filtros e paginação</param>
        /// <returns>Lista paginada de clientes</returns>
        /// <response code="200">Lista de clientes retornada com sucesso</response>
        /// <response code="206">Lista parcial de clientes (paginação)</response>
        /// <response code="204">Nenhum cliente encontrado</response>
        /// <response code="400">Parâmetros de consulta inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar clientes",
            Description = "Retorna uma lista paginada de clientes com filtros opcionais. Suporta paginação e busca por nome, email, documento e telefone.",
            OperationId = "GetCustomers",
            Tags = new[] { "Customers" }
        )]
        [SwaggerResponse(200, "Lista de clientes retornada com sucesso", typeof(GetCustomerResponse))]
        [SwaggerResponse(206, "Lista parcial de clientes (paginação)", typeof(GetCustomerResponse))]
        [SwaggerResponse(204, "Nenhum cliente encontrado")]
        [SwaggerResponse(400, "Parâmetros de consulta inválidos", typeof(GetCustomerResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetCustomerResponse))]
        public async Task<ActionResult> Get([FromQuery] GetCustomerRequest request)
        {
            Logger.LogInformation("GET /api/v1/customer - Starting customer list request. Filters: {Filters}", 
                new { request.FullName, request.Email, request.DocumentNumber, request.Phone });
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/customer - Successfully processed customer list request. StatusCode: {StatusCode}", 
                    response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/customer - Unexpected error occurred while processing customer list request");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um cliente específico pelo ID
        /// </summary>
        /// <param name="id">ID único do cliente (GUID)</param>
        /// <returns>Dados do cliente solicitado</returns>
        /// <response code="200">Cliente encontrado e retornado com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter cliente por ID",
            Description = "Retorna os dados de um cliente específico baseado no ID fornecido.",
            OperationId = "GetCustomerById",
            Tags = new[] { "Customers" }
        )]
        [SwaggerResponse(200, "Cliente encontrado e retornado com sucesso", typeof(GetCustomerByKeyResponse))]
        [SwaggerResponse(400, "ID inválido", typeof(GetCustomerByKeyResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado", typeof(GetCustomerByKeyResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetCustomerByKeyResponse))]
        public async Task<ActionResult> GetByKey(Guid id)
        {
            Logger.LogInformation("GET /api/v1/customer/{CustomerId} - Starting get customer by key request", id);
            
            var request = new GetCustomerByKeyRequest()
            {
                Id = id
            };
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/customer/{CustomerId} - Successfully processed get customer by key request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/customer/{CustomerId} - Unexpected error occurred while processing get customer by key request", id);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo cliente
        /// </summary>
        /// <param name="request">Dados do cliente a ser criado</param>
        /// <returns>Cliente criado com ID gerado</returns>
        /// <response code="201">Cliente criado com sucesso</response>
        /// <response code="400">Dados inválidos ou cliente já existe</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="409">Conflito - email ou documento já existem</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar novo cliente",
            Description = "Cria um novo cliente com endereço. Valida se email e documento são únicos.",
            OperationId = "CreateCustomer",
            Tags = new[] { "Customers" }
        )]
        [SwaggerResponse(201, "Cliente criado com sucesso", typeof(AddCustomerResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(AddCustomerResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(409, "Conflito - email ou documento já existem", typeof(AddCustomerResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(AddCustomerResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(AddCustomerResponse))]
        public async Task<ActionResult> Add([FromBody] AddCustomerRequest request)
        {
            Logger.LogInformation("POST /api/v1/customer - Starting add customer request. Email: {Email}, Document: {Document}", 
                request.Email, request.DocumentNumber);
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("POST /api/v1/customer - Successfully processed add customer request. StatusCode: {StatusCode}, CustomerId: {CustomerId}", 
                    response.StatusCode, response.Data?.Id);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "POST /api/v1/customer - Unexpected error occurred while processing add customer request. Email: {Email}", 
                    request.Email);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um cliente existente
        /// </summary>
        /// <param name="id">ID do cliente a ser atualizado</param>
        /// <param name="request">Novos dados do cliente</param>
        /// <returns>Cliente atualizado</returns>
        /// <response code="200">Cliente atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="409">Conflito - email ou documento já existem</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Atualizar cliente",
            Description = "Atualiza os dados de um cliente existente. Valida se email e documento são únicos (exceto para o próprio cliente).",
            OperationId = "UpdateCustomer",
            Tags = new[] { "Customers" }
        )]
        [SwaggerResponse(200, "Cliente atualizado com sucesso", typeof(ChangeCustomerResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(ChangeCustomerResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado", typeof(ChangeCustomerResponse))]
        [SwaggerResponse(409, "Conflito - email ou documento já existem", typeof(ChangeCustomerResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(ChangeCustomerResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ChangeCustomerResponse))]
        public async Task<ActionResult> Change(Guid id, [FromBody] ChangeCustomerRequest request)
        {
            Logger.LogInformation("PUT /api/v1/customer/{CustomerId} - Starting change customer request. Email: {Email}, Document: {Document}", 
                id, request.Email, request.DocumentNumber);
            
            try
            {
                request.Id = id;
                var response = await Mediator.Send(request);
                Logger.LogInformation("PUT /api/v1/customer/{CustomerId} - Successfully processed change customer request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "PUT /api/v1/customer/{CustomerId} - Unexpected error occurred while processing change customer request. Email: {Email}", 
                    id, request.Email);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Remove um cliente
        /// </summary>
        /// <param name="id">ID do cliente a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="200">Cliente removido com sucesso</response>
        /// <response code="204">Cliente não encontrado (já removido)</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Cliente não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Remover cliente",
            Description = "Remove um cliente específico baseado no ID fornecido. A remoção é lógica (soft delete).",
            OperationId = "DeleteCustomer",
            Tags = new[] { "Customers" }
        )]
        [SwaggerResponse(200, "Cliente removido com sucesso", typeof(RemoveCustomerResponse))]
        [SwaggerResponse(204, "Cliente não encontrado (já removido)")]
        [SwaggerResponse(400, "ID inválido", typeof(RemoveCustomerResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Cliente não encontrado", typeof(RemoveCustomerResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(RemoveCustomerResponse))]
        public async Task<ActionResult> Remove(Guid id)
        {
            Logger.LogInformation("DELETE /api/v1/customer/{CustomerId} - Starting remove customer request", id);
            
            try
            {
                var request = new RemoveCustomerRequest()
                {
                    Id = id
                };

                var response = await Mediator.Send(request);
                Logger.LogInformation("DELETE /api/v1/customer/{CustomerId} - Successfully processed remove customer request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DELETE /api/v1/customer/{CustomerId} - Unexpected error occurred while processing remove customer request", id);
                return HandleException(ex);
            }
        }
    }
}
