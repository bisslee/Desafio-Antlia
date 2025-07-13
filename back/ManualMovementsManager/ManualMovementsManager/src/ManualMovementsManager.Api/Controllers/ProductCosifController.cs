using ManualMovementsManager.Domain.Entities.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ManualMovementsManager.Api.Helper;
using ManualMovementsManager.Application.Commands.ProductCosifs.RemoveProductCosif;
using ManualMovementsManager.Application.Commands.ProductCosifs.ChangeProductCosif;
using ManualMovementsManager.Application.Commands.ProductCosifs.AddProductCosif;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosifByKey;
using Microsoft.Extensions.Logging;

namespace ManualMovementsManager.Api.Controllers
{
    /// <summary>
    /// Controller responsável por gerenciar operações relacionadas a produtos COSIF
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    [SwaggerTag("Gerenciamento de Produtos COSIF")]
    public class ProductCosifController : BaseControllerHandle
    {
        private readonly IMediator Mediator;
        private readonly ILogger<ProductCosifController> Logger;

        public ProductCosifController(            
            IMediator mediator,
            ILogger<ProductCosifController> logger
            ) : base()
        {
            Mediator = mediator;
            Logger = logger;
        }

        /// <summary>
        /// Lista produtos COSIF com paginação e filtros opcionais
        /// </summary>
        /// <param name="request">Parâmetros de consulta incluindo filtros e paginação</param>
        /// <returns>Lista paginada de produtos COSIF</returns>
        /// <response code="200">Lista de produtos COSIF retornada com sucesso</response>
        /// <response code="206">Lista parcial de produtos COSIF (paginação)</response>
        /// <response code="204">Nenhum produto COSIF encontrado</response>
        /// <response code="400">Parâmetros de consulta inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar produtos COSIF",
            Description = "Retorna uma lista paginada de produtos COSIF com filtros opcionais. Suporta paginação e busca por código do produto, código COSIF e código de classificação.",
            OperationId = "GetProductCosifs",
            Tags = new[] { "ProductCosifs" }
        )]
        [SwaggerResponse(200, "Lista de produtos COSIF retornada com sucesso", typeof(GetProductCosifResponse))]
        [SwaggerResponse(206, "Lista parcial de produtos COSIF (paginação)", typeof(GetProductCosifResponse))]
        [SwaggerResponse(204, "Nenhum produto COSIF encontrado")]
        [SwaggerResponse(400, "Parâmetros de consulta inválidos", typeof(GetProductCosifResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetProductCosifResponse))]
        public async Task<ActionResult> Get([FromQuery] GetProductCosifRequest request)
        {
            Logger.LogInformation("GET /api/v1/productcosif - Starting product cosif list request. Filters: {Filters}", 
                new { request.ProductCode, request.CosifCode, request.ClassificationCode, request.Active });
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/productcosif - Successfully processed product cosif list request. StatusCode: {StatusCode}", 
                    response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/productcosif - Unexpected error occurred while processing product cosif list request");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um produto COSIF específico pelo ID
        /// </summary>
        /// <param name="id">ID único do produto COSIF (GUID)</param>
        /// <returns>Dados do produto COSIF solicitado</returns>
        /// <response code="200">Produto COSIF encontrado e retornado com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Produto COSIF não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter produto COSIF por ID",
            Description = "Retorna os dados de um produto COSIF específico baseado no ID fornecido.",
            OperationId = "GetProductCosifById",
            Tags = new[] { "ProductCosifs" }
        )]
        [SwaggerResponse(200, "Produto COSIF encontrado e retornado com sucesso", typeof(GetProductCosifByKeyResponse))]
        [SwaggerResponse(400, "ID inválido", typeof(GetProductCosifByKeyResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Produto COSIF não encontrado", typeof(GetProductCosifByKeyResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetProductCosifByKeyResponse))]
        public async Task<ActionResult> GetByKey(Guid id)
        {
            Logger.LogInformation("GET /api/v1/productcosif/{ProductCosifId} - Starting get product cosif by key request", id);
            
            var request = new GetProductCosifByKeyRequest()
            {
                Id = id
            };
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/productcosif/{ProductCosifId} - Successfully processed get product cosif by key request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/productcosif/{ProductCosifId} - Unexpected error occurred while processing get product cosif by key request", id);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo produto COSIF
        /// </summary>
        /// <param name="request">Dados do produto COSIF a ser criado</param>
        /// <returns>Produto COSIF criado com ID gerado</returns>
        /// <response code="201">Produto COSIF criado com sucesso</response>
        /// <response code="400">Dados inválidos ou produto COSIF já existe</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="409">Conflito - combinação produto/cosif já existe</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar novo produto COSIF",
            Description = "Cria um novo produto COSIF. Valida se a combinação de código do produto e código COSIF é única.",
            OperationId = "CreateProductCosif",
            Tags = new[] { "ProductCosifs" }
        )]
        [SwaggerResponse(201, "Produto COSIF criado com sucesso", typeof(AddProductCosifResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(AddProductCosifResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(409, "Conflito - combinação produto/cosif já existe", typeof(AddProductCosifResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(AddProductCosifResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(AddProductCosifResponse))]
        public async Task<ActionResult> Add([FromBody] AddProductCosifRequest request)
        {
            Logger.LogInformation("POST /api/v1/productcosif - Starting add product cosif request. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                request.ProductCode, request.CosifCode);
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("POST /api/v1/productcosif - Successfully processed add product cosif request. StatusCode: {StatusCode}, ProductCosifId: {ProductCosifId}", 
                    response.StatusCode, response.Data?.Id);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "POST /api/v1/productcosif - Unexpected error occurred while processing add product cosif request. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    request.ProductCode, request.CosifCode);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um produto COSIF existente
        /// </summary>
        /// <param name="id">ID do produto COSIF a ser atualizado</param>
        /// <param name="request">Novos dados do produto COSIF</param>
        /// <returns>Produto COSIF atualizado</returns>
        /// <response code="200">Produto COSIF atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Produto COSIF não encontrado</response>
        /// <response code="409">Conflito - combinação produto/cosif já existe</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Atualizar produto COSIF",
            Description = "Atualiza os dados de um produto COSIF existente. Valida se a combinação de código do produto e código COSIF é única (exceto para o próprio produto COSIF).",
            OperationId = "UpdateProductCosif",
            Tags = new[] { "ProductCosifs" }
        )]
        [SwaggerResponse(200, "Produto COSIF atualizado com sucesso", typeof(ChangeProductCosifResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(ChangeProductCosifResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Produto COSIF não encontrado", typeof(ChangeProductCosifResponse))]
        [SwaggerResponse(409, "Conflito - combinação produto/cosif já existe", typeof(ChangeProductCosifResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(ChangeProductCosifResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ChangeProductCosifResponse))]
        public async Task<ActionResult> Change(Guid id, [FromBody] ChangeProductCosifRequest request)
        {
            Logger.LogInformation("PUT /api/v1/productcosif/{ProductCosifId} - Starting change product cosif request. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                id, request.ProductCode, request.CosifCode);
            
            try
            {
                request.Id = id;
                var response = await Mediator.Send(request);
                Logger.LogInformation("PUT /api/v1/productcosif/{ProductCosifId} - Successfully processed change product cosif request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "PUT /api/v1/productcosif/{ProductCosifId} - Unexpected error occurred while processing change product cosif request. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    id, request.ProductCode, request.CosifCode);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Remove um produto COSIF
        /// </summary>
        /// <param name="id">ID do produto COSIF a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="200">Produto COSIF removido com sucesso</response>
        /// <response code="204">Produto COSIF não encontrado (já removido)</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Produto COSIF não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Remover produto COSIF",
            Description = "Remove um produto COSIF específico baseado no ID fornecido. A remoção é lógica (soft delete).",
            OperationId = "DeleteProductCosif",
            Tags = new[] { "ProductCosifs" }
        )]
        [SwaggerResponse(200, "Produto COSIF removido com sucesso", typeof(RemoveProductCosifResponse))]
        [SwaggerResponse(204, "Produto COSIF não encontrado (já removido)")]
        [SwaggerResponse(400, "ID inválido", typeof(RemoveProductCosifResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Produto COSIF não encontrado", typeof(RemoveProductCosifResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(RemoveProductCosifResponse))]
        public async Task<ActionResult> Remove(Guid id)
        {
            Logger.LogInformation("DELETE /api/v1/productcosif/{ProductCosifId} - Starting remove product cosif request", id);
            
            try
            {
                var request = new RemoveProductCosifRequest()
                {
                    Id = id
                };

                var response = await Mediator.Send(request);
                Logger.LogInformation("DELETE /api/v1/productcosif/{ProductCosifId} - Successfully processed remove product cosif request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DELETE /api/v1/productcosif/{ProductCosifId} - Unexpected error occurred while processing remove product cosif request", id);
                return HandleException(ex);
            }
        }
    }
} 