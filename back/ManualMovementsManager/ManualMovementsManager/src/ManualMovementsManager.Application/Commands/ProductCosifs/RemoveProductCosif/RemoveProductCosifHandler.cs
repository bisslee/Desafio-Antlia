using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.ProductCosifs;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.RemoveProductCosif
{
    public class RemoveProductCosifHandler :
        IRequestHandler<RemoveProductCosifRequest, RemoveProductCosifResponse>
    {
        private readonly ILogger<RemoveProductCosifHandler> Logger;
        private readonly IWriteRepository<ProductCosif> Repository;
        private readonly IProductCosifReadRepository ProductCosifReadRepository;
        private readonly IResponseBuilder ResponseBuilder;
        
        public RemoveProductCosifHandler(
            ILogger<RemoveProductCosifHandler> logger,
            IWriteRepository<ProductCosif> repository,
            IProductCosifReadRepository productCosifReadRepository,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ProductCosifReadRepository = productCosifReadRepository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<RemoveProductCosifResponse> Handle(RemoveProductCosifRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting RemoveProductCosifRequest processing for product cosif ID: {ProductCosifId}", request.Id);

            try
            {
                Logger.LogDebug("Checking if product cosif exists for removal. ProductCosifId: {ProductCosifId}", request.Id);
                
                // Verificar se o product cosif existe
                var productCosifExistsSpec = new ProductCosifMustExistByIdSpecification(ProductCosifReadRepository);
                await SpecificationHandler.ValidateAsync<ProductCosif>(productCosifExistsSpec, new ProductCosif { Id = request.Id });

                Logger.LogDebug("Product cosif found, retrieving data for removal. ProductCosifId: {ProductCosifId}", request.Id);
                var entity = await ProductCosifReadRepository.GetByIdAsync(request.Id);
                
                Logger.LogDebug("Proceeding to delete product cosif. ProductCosifId: {ProductCosifId}, CosifCode: {CosifCode}", 
                    entity.Id, entity.CosifCode);
                
                var result = await Repository.Delete(entity);
                
                if (result)
                {
                    Logger.LogInformation("Product cosif removed successfully. ProductCosifId: {ProductCosifId}, CosifCode: {CosifCode}", 
                        entity.Id, entity.CosifCode);
                    return ResponseBuilder.BuildSuccessResponse<RemoveProductCosifResponse, bool>(true, "Product cosif removed successfully");
                }

                Logger.LogError("Failed to remove product cosif from repository. ProductCosifId: {ProductCosifId}, CosifCode: {CosifCode}", 
                    entity.Id, entity.CosifCode);
                return ResponseBuilder.BuildErrorResponse<RemoveProductCosifResponse, bool>("Failed to remove product cosif");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while removing product cosif. ProductCosifId: {ProductCosifId}", request.Id);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 