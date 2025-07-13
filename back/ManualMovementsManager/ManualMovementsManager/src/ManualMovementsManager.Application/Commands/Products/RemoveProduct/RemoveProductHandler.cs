using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.Products;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.Products.RemoveProduct
{
    public class RemoveProductHandler :
        IRequestHandler<RemoveProductRequest, RemoveProductResponse>
    {
        private readonly ILogger<RemoveProductHandler> Logger;
        private readonly IWriteRepository<Product> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IResponseBuilder ResponseBuilder;
        
        public RemoveProductHandler(
            ILogger<RemoveProductHandler> logger,
            IWriteRepository<Product> repository,
            IProductReadRepository productReadRepository,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ProductReadRepository = productReadRepository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<RemoveProductResponse> Handle(RemoveProductRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting RemoveProductRequest processing for product ID: {ProductId}", request.Id);

            try
            {
                Logger.LogDebug("Checking if product exists for removal. ProductId: {ProductId}", request.Id);
                
                // Verificar se o product existe
                var productExistsSpec = new ProductMustExistByIdSpecification(ProductReadRepository);
                await SpecificationHandler.ValidateAsync<Product>(productExistsSpec, new Product { Id = request.Id });

                Logger.LogDebug("Product found, retrieving data for removal. ProductId: {ProductId}", request.Id);
                var entity = await ProductReadRepository.GetByIdAsync(request.Id);
                
                Logger.LogDebug("Proceeding to delete product. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    entity.Id, entity.ProductCode);
                
                var result = await Repository.Delete(entity);
                
                if (result)
                {
                    Logger.LogInformation("Product removed successfully. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                        entity.Id, entity.ProductCode);
                    return ResponseBuilder.BuildSuccessResponse<RemoveProductResponse, bool>(true, "Product removed successfully");
                }

                Logger.LogError("Failed to remove product from repository. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    entity.Id, entity.ProductCode);
                return ResponseBuilder.BuildErrorResponse<RemoveProductResponse, bool>("Failed to remove product");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while removing product. ProductId: {ProductId}", request.Id);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 