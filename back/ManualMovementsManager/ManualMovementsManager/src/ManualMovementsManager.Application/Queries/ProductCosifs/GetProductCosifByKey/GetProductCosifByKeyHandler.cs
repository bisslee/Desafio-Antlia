using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosifByKey
{
    public class GetProductCosifByKeyHandler : IRequestHandler<GetProductCosifByKeyRequest, GetProductCosifByKeyResponse>
    {
        private readonly ILogger<GetProductCosifByKeyHandler> Logger;
        private readonly IReadRepository<ProductCosif> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetProductCosifByKeyHandler(
            IReadRepository<ProductCosif> repository,
            ILogger<GetProductCosifByKeyHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetProductCosifByKeyResponse> Handle(
            GetProductCosifByKeyRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetProductCosifByKeyRequest processing for product cosif ID: {ProductCosifId}", request.Id);

            try
            {
                Logger.LogDebug("Retrieving product cosif by ID: {ProductCosifId}", request.Id);
                
                var productCosif = await Repository.GetByIdAsync(request.Id);
                
                if (productCosif == null)
                {
                    Logger.LogWarning("Product cosif not found for ID: {ProductCosifId}", request.Id);
                    return ResponseBuilder.BuildErrorResponse<GetProductCosifByKeyResponse, ProductCosif>("Product cosif not found");
                }

                Logger.LogInformation("Successfully retrieved product cosif. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    productCosif.Id, productCosif.ProductCode, productCosif.CosifCode);

                return ResponseBuilder.BuildSuccessResponse<GetProductCosifByKeyResponse, ProductCosif>(productCosif);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving product cosif by ID: {ProductCosifId}", request.Id);
                return ResponseBuilder.BuildErrorResponse<GetProductCosifByKeyResponse, ProductCosif>("An error occurred while retrieving the product cosif");
            }
        }
    }
} 