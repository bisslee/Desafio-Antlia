using ManualMovementsManager.Domain.Entities.Response;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using ManualMovementsManager.Api.Helper;
using ManualMovementsManager.Application.Commands.Products.RemoveProduct;
using ManualMovementsManager.Application.Commands.Products.ChangeProduct;
using ManualMovementsManager.Application.Commands.Products.AddProduct;
using ManualMovementsManager.Application.Queries.Products.GetProduct;
using ManualMovementsManager.Application.Queries.Products.GetProductByKey;
using Microsoft.Extensions.Logging;

namespace ManualMovementsManager.Api.Controllers
{
    /// <summary>
    /// Gerencia operações de cadastro, consulta, atualização e remoção de produtos.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ProductController : BaseControllerHandle
    {
        private readonly IMediator Mediator;
        private readonly ILogger<ProductController> Logger;

        public ProductController(            
            IMediator mediator,
            ILogger<ProductController> logger
            ) : base()
        {
            Mediator = mediator;
            Logger = logger;
        }

        /// <summary>
        /// Lista produtos com paginação e filtros opcionais
        /// </summary>
        /// <param name="request">Parâmetros de consulta incluindo filtros e paginação</param>
        /// <returns>Lista paginada de produtos</returns>
        /// <response code="200">Lista de produtos retornada com sucesso</response>
        /// <response code="206">Lista parcial de produtos (paginação)</response>
        /// <response code="204">Nenhum produto encontrado</response>
        /// <response code="400">Parâmetros de consulta inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet]
        [SwaggerOperation(
            Summary = "Listar produtos",
            Description = "Retorna uma lista paginada de produtos com filtros opcionais. Suporta paginação e busca por código e descrição.",
            OperationId = "GetProducts",
            Tags = new[] { "Products" }
        )]
        [SwaggerResponse(200, "Lista de produtos retornada com sucesso", typeof(GetProductResponse))]
        [SwaggerResponse(206, "Lista parcial de produtos (paginação)", typeof(GetProductResponse))]
        [SwaggerResponse(204, "Nenhum produto encontrado")]
        [SwaggerResponse(400, "Parâmetros de consulta inválidos", typeof(GetProductResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetProductResponse))]
        public async Task<ActionResult> Get([FromQuery] GetProductRequest request)
        {
            Logger.LogInformation("GET /api/v1/product - Starting product list request. Filters: {Filters}", 
                new { request.ProductCode, request.Description, request.Active });
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/product - Successfully processed product list request. StatusCode: {StatusCode}", 
                    response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/product - Unexpected error occurred while processing product list request");
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Obtém um produto específico pelo ID
        /// </summary>
        /// <param name="id">ID único do produto (GUID)</param>
        /// <returns>Dados do produto solicitado</returns>
        /// <response code="200">Produto encontrado e retornado com sucesso</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpGet("{id:guid}")]
        [SwaggerOperation(
            Summary = "Obter produto por ID",
            Description = "Retorna os dados de um produto específico baseado no ID fornecido.",
            OperationId = "GetProductById",
            Tags = new[] { "Products" }
        )]
        [SwaggerResponse(200, "Produto encontrado e retornado com sucesso", typeof(GetProductByKeyResponse))]
        [SwaggerResponse(400, "ID inválido", typeof(GetProductByKeyResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Produto não encontrado", typeof(GetProductByKeyResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(GetProductByKeyResponse))]
        public async Task<ActionResult> GetByKey(Guid id)
        {
            Logger.LogInformation("GET /api/v1/product/{ProductId} - Starting get product by key request", id);
            
            var request = new GetProductByKeyRequest()
            {
                Id = id
            };
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("GET /api/v1/product/{ProductId} - Successfully processed get product by key request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "GET /api/v1/product/{ProductId} - Unexpected error occurred while processing get product by key request", id);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Cria um novo produto
        /// </summary>
        /// <param name="request">Dados do produto a ser criado</param>
        /// <returns>Produto criado com ID gerado</returns>
        /// <response code="201">Produto criado com sucesso</response>
        /// <response code="400">Dados inválidos ou produto já existe</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="409">Conflito - código do produto já existe</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPost]
        [SwaggerOperation(
            Summary = "Criar novo produto",
            Description = "Cria um novo produto. Valida se o código do produto é único.",
            OperationId = "CreateProduct",
            Tags = new[] { "Products" }
        )]
        [SwaggerResponse(201, "Produto criado com sucesso", typeof(AddProductResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(AddProductResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(409, "Conflito - código do produto já existe", typeof(AddProductResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(AddProductResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(AddProductResponse))]
        public async Task<ActionResult> Add([FromBody] AddProductRequest request)
        {
            Logger.LogInformation("POST /api/v1/product - Starting add product request. ProductCode: {ProductCode}", 
                request.ProductCode);
            
            try
            {
                var response = await Mediator.Send(request);
                Logger.LogInformation("POST /api/v1/product - Successfully processed add product request. StatusCode: {StatusCode}, ProductId: {ProductId}", 
                    response.StatusCode, response.Data?.Id);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "POST /api/v1/product - Unexpected error occurred while processing add product request. ProductCode: {ProductCode}", 
                    request.ProductCode);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Atualiza um produto existente
        /// </summary>
        /// <param name="id">ID do produto a ser atualizado</param>
        /// <param name="request">Novos dados do produto</param>
        /// <returns>Produto atualizado</returns>
        /// <response code="200">Produto atualizado com sucesso</response>
        /// <response code="400">Dados inválidos</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="409">Conflito - código do produto já existe</response>
        /// <response code="422">Entidade não processável</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpPut("{id:guid}")]
        [SwaggerOperation(
            Summary = "Atualizar produto",
            Description = "Atualiza os dados de um produto existente. Valida se o código do produto é único (exceto para o próprio produto).",
            OperationId = "UpdateProduct",
            Tags = new[] { "Products" }
        )]
        [SwaggerResponse(200, "Produto atualizado com sucesso", typeof(ChangeProductResponse))]
        [SwaggerResponse(400, "Dados inválidos", typeof(ChangeProductResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Produto não encontrado", typeof(ChangeProductResponse))]
        [SwaggerResponse(409, "Conflito - código do produto já existe", typeof(ChangeProductResponse))]
        [SwaggerResponse(422, "Entidade não processável", typeof(ChangeProductResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(ChangeProductResponse))]
        public async Task<ActionResult> Change(Guid id, [FromBody] ChangeProductRequest request)
        {
            Logger.LogInformation("PUT /api/v1/product/{ProductId} - Starting change product request. ProductCode: {ProductCode}", 
                id, request.ProductCode);
            
            try
            {
                request.Id = id;
                var response = await Mediator.Send(request);
                Logger.LogInformation("PUT /api/v1/product/{ProductId} - Successfully processed change product request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "PUT /api/v1/product/{ProductId} - Unexpected error occurred while processing change product request. ProductCode: {ProductCode}", 
                    id, request.ProductCode);
                return HandleException(ex);
            }
        }

        /// <summary>
        /// Remove um produto
        /// </summary>
        /// <param name="id">ID do produto a ser removido</param>
        /// <returns>Confirmação da remoção</returns>
        /// <response code="200">Produto removido com sucesso</response>
        /// <response code="204">Produto não encontrado (já removido)</response>
        /// <response code="400">ID inválido</response>
        /// <response code="401">Não autorizado</response>
        /// <response code="404">Produto não encontrado</response>
        /// <response code="500">Erro interno do servidor</response>
        [HttpDelete("{id:guid}")]
        [SwaggerOperation(
            Summary = "Remover produto",
            Description = "Remove um produto específico baseado no ID fornecido. A remoção é lógica (soft delete).",
            OperationId = "DeleteProduct",
            Tags = new[] { "Products" }
        )]
        [SwaggerResponse(200, "Produto removido com sucesso", typeof(RemoveProductResponse))]
        [SwaggerResponse(204, "Produto não encontrado (já removido)")]
        [SwaggerResponse(400, "ID inválido", typeof(RemoveProductResponse))]
        [SwaggerResponse(401, "Não autorizado")]
        [SwaggerResponse(404, "Produto não encontrado", typeof(RemoveProductResponse))]
        [SwaggerResponse(500, "Erro interno do servidor", typeof(RemoveProductResponse))]
        public async Task<ActionResult> Remove(Guid id)
        {
            Logger.LogInformation("DELETE /api/v1/product/{ProductId} - Starting remove product request", id);
            
            try
            {
                var request = new RemoveProductRequest()
                {
                    Id = id
                };

                var response = await Mediator.Send(request);
                Logger.LogInformation("DELETE /api/v1/product/{ProductId} - Successfully processed remove product request. StatusCode: {StatusCode}", 
                    id, response.StatusCode);
                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "DELETE /api/v1/product/{ProductId} - Unexpected error occurred while processing remove product request", id);
                return HandleException(ex);
            }
        }
    }
} 