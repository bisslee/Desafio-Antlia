using ManualMovementsManager.Domain.Entities.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ManualMovementsManager.Api.Helper;
using ManualMovementsManager.Application.Commands.ManualMovements.RemoveManualMovement;
using ManualMovementsManager.Application.Commands.ManualMovements.ChangeManualMovement;
using ManualMovementsManager.Application.Commands.ManualMovements.AddManualMovement;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovementByKey;
using Microsoft.Extensions.Logging;

namespace ManualMovementsManager.Api.Controllers
{
    /// <summary>
    /// Gerencia operações de cadastro, consulta, atualização e remoção de movimentos manuais.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ManualMovementController : BaseControllerHandle
    {
        private readonly IMediator Mediator;
        private readonly ILogger<ManualMovementController> Logger;

        public ManualMovementController(            
            IMediator mediator,
            ILogger<ManualMovementController> logger
            ) : base()
        {
            Mediator = mediator;
            Logger = logger;
        }

        /// <summary>
        /// Lista movimentos manuais com paginação e filtros opcionais
        /// </summary>
        /// <param name="request">Parâmetros de consulta incluindo filtros e paginação</param>
        /// <returns>Lista paginada de movimentos manuais</returns>
        /// <response code="200">Lista de movimentos manuais retornada com sucesso</response>
        /// <response code="206">Lista parcial de movimentos manuais (paginação)</response>
        /// <response code="204">Nenhum movimento manual encontrado</response>
        /// <response code="400">Parâmetros de consulta inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar movimentos manuais",
            Description = "Retorna uma lista paginada de movimentos manuais com filtros opcionais. Suporta paginação e busca por descrição, período e valor.",
            OperationId = "GetManualMovements",
            Tags = new[] { "ManualMovements" }
        )]
        [SwaggerResponse(200, "Lista de movimentos manuais retornada com sucesso", typeof(GetManualMovementResponse))]
        [SwaggerResponse(206, "Lista parcial de movimentos manuais (paginação)", typeof(GetManualMovementResponse))]
        [SwaggerResponse(204, "Nenhum movimento manual encontrado")]
        [SwaggerResponse(400, "Parâmetros de consulta inválidos", typeof(GetManualMovementResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetManualMovementResponse))]
        public async Task<ActionResult> Get([FromQuery] GetManualMovementRequest request)
        {
            Logger.LogInformation("GET /api/v1/manualmovement - Starting manual movement list request. Filters: {Filters}", 
                new { request.Description, request.StartDate, request.EndDate, request.MinValue, request.MaxValue, request.Active });
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/manualmovement - Successfully processed manual movement list request. StatusCode: {StatusCode}", 
                    response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/manualmovement - Unexpected error occurred while processing manual movement list request");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um movimento manual específico pelo ID
        /// </summary>
        /// <param name="id">ID único do movimento manual (GUID)</param>
        /// <returns>Dados do movimento manual solicitado</returns>
        /// <response code="200">Movimento manual encontrado e retornado com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Movimento manual não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter movimento manual por ID",
            Description = "Retorna os dados de um movimento manual específico baseado no ID fornecido.",
            OperationId = "GetManualMovementById",
            Tags = new[] { "ManualMovements" }
        )]
        [SwaggerResponse(200, "Movimento manual encontrado e retornado com sucesso", typeof(GetManualMovementByKeyResponse))]
        [SwaggerResponse(400, "ID inválido", typeof(GetManualMovementByKeyResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Movimento manual não encontrado", typeof(GetManualMovementByKeyResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetManualMovementByKeyResponse))]
        public async Task<ActionResult> GetByKey(Guid id)
        {
            Logger.LogInformation("GET /api/v1/manualmovement/{ManualMovementId} - Starting get manual movement by key request", id);
            
            var request = new GetManualMovementByKeyRequest()
            {
                Id = id
            };
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/manualmovement/{ManualMovementId} - Successfully processed get manual movement by key request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/manualmovement/{ManualMovementId} - Unexpected error occurred while processing get manual movement by key request", id);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo movimento manual
        /// </summary>
        /// <param name="request">Dados do movimento manual a ser criado</param>
        /// <returns>Movimento manual criado com ID gerado</returns>
        /// <response code="201">Movimento manual criado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar novo movimento manual",
            Description = "Cria um novo movimento manual com validação de produtos e produtos COSIF.",
            OperationId = "CreateManualMovement",
            Tags = new[] { "ManualMovements" }
        )]
        [SwaggerResponse(201, "Movimento manual criado com sucesso", typeof(AddManualMovementResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(AddManualMovementResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(422, "Entidade não processável", typeof(AddManualMovementResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(AddManualMovementResponse))]
        public async Task<ActionResult> Add([FromBody] AddManualMovementRequest request)
        {
            Logger.LogInformation("POST /api/v1/manualmovement - Starting add manual movement request. Description: {Description}, Value: {Value}", 
                request.Description, request.Value);
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("POST /api/v1/manualmovement - Successfully processed add manual movement request. StatusCode: {StatusCode}, ManualMovementId: {ManualMovementId}", 
                    response.StatusCode, response.Data?.Id);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "POST /api/v1/manualmovement - Unexpected error occurred while processing add manual movement request. Description: {Description}", 
                    request.Description);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um movimento manual existente
        /// </summary>
        /// <param name="id">ID do movimento manual a ser atualizado</param>
        /// <param name="request">Novos dados do movimento manual</param>
        /// <returns>Movimento manual atualizado</returns>
        /// <response code="200">Movimento manual atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Movimento manual não encontrado</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Atualizar movimento manual",
            Description = "Atualiza os dados de um movimento manual existente com validação de produtos e produtos COSIF.",
            OperationId = "UpdateManualMovement",
            Tags = new[] { "ManualMovements" }
        )]
        [SwaggerResponse(200, "Movimento manual atualizado com sucesso", typeof(ChangeManualMovementResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(ChangeManualMovementResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Movimento manual não encontrado", typeof(ChangeManualMovementResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(ChangeManualMovementResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ChangeManualMovementResponse))]
        public async Task<ActionResult> Change(Guid id, [FromBody] ChangeManualMovementRequest request)
        {
            Logger.LogInformation("PUT /api/v1/manualmovement/{ManualMovementId} - Starting change manual movement request. Description: {Description}, Value: {Value}", 
                id, request.Description, request.Value);
            
            try
            {
                request.Id = id;
                var response = await Mediator.Send(request);
                Logger.LogInformation("PUT /api/v1/manualmovement/{ManualMovementId} - Successfully processed change manual movement request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "PUT /api/v1/manualmovement/{ManualMovementId} - Unexpected error occurred while processing change manual movement request. Description: {Description}", 
                    id, request.Description);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Remove um movimento manual
        /// </summary>
        /// <param name="id">ID do movimento manual a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="200">Movimento manual removido com sucesso</response>
        /// <response code="204">Movimento manual não encontrado (já removido)</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Movimento manual não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Remover movimento manual",
            Description = "Remove um movimento manual específico baseado no ID fornecido. A remoção é lógica (soft delete).",
            OperationId = "DeleteManualMovement",
            Tags = new[] { "ManualMovements" }
        )]
        [SwaggerResponse(200, "Movimento manual removido com sucesso", typeof(RemoveManualMovementResponse))]
        [SwaggerResponse(204, "Movimento manual não encontrado (já removido)")]
        [SwaggerResponse(400, "ID inválido", typeof(RemoveManualMovementResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Movimento manual não encontrado", typeof(RemoveManualMovementResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(RemoveManualMovementResponse))]
        public async Task<ActionResult> Remove(Guid id)
        {
            Logger.LogInformation("DELETE /api/v1/manualmovement/{ManualMovementId} - Starting remove manual movement request", id);
            
            try
            {
                var request = new RemoveManualMovementRequest()
                {
                    Id = id
                };

                var response = await Mediator.Send(request);
                Logger.LogInformation("DELETE /api/v1/manualmovement/{ManualMovementId} - Successfully processed remove manual movement request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DELETE /api/v1/manualmovement/{ManualMovementId} - Unexpected error occurred while processing remove manual movement request", id);
                return HandleException(ex);
            }
        }
    }
} 