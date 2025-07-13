using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.Products.GetProductByKey
{
    public class GetProductByKeyHandler : IRequestHandler<GetProductByKeyRequest, GetProductByKeyResponse>
    {
        private readonly ILogger<GetProductByKeyHandler> Logger;
        private readonly IReadRepository<Product> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetProductByKeyHandler(
            IReadRepository<Product> repository,
            ILogger<GetProductByKeyHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetProductByKeyResponse> Handle(
            GetProductByKeyRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetProductByKeyRequest processing for product ID: {ProductId}", request.Id);

            try
            {
                Logger.LogDebug("Retrieving product by ID: {ProductId}", request.Id);
                
                var product = await Repository.GetByIdAsync(request.Id);
                
                if (product == null)
                {
                    Logger.LogWarning("Product not found for ID: {ProductId}", request.Id);
                    return ResponseBuilder.BuildErrorResponse<GetProductByKeyResponse, Product>("Product not found");
                }

                Logger.LogInformation("Successfully retrieved product. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    product.Id, product.ProductCode);

                return ResponseBuilder.BuildSuccessResponse<GetProductByKeyResponse, Product>(product);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving product by ID: {ProductId}", request.Id);
                return ResponseBuilder.BuildErrorResponse<GetProductByKeyResponse, Product>("An error occurred while retrieving the product");
            }
        }
    }
} 