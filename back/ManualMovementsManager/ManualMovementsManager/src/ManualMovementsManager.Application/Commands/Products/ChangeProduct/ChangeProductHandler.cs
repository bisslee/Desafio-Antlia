using AutoMapper;
using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.Products;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.Products.ChangeProduct
{
    public class ChangeProductHandler : IRequestHandler<ChangeProductRequest, ChangeProductResponse>
    {
        private readonly ILogger<ChangeProductHandler> Logger;
        private readonly IWriteRepository<Product> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IValidator<ChangeProductRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public ChangeProductHandler(
            ILogger<ChangeProductHandler> logger,
            IWriteRepository<Product> repository,
            IProductReadRepository productReadRepository,
            IValidator<ChangeProductRequest> validator,
            IMapper mapper,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ProductReadRepository = productReadRepository;
            Validator = validator;
            Mapper = mapper;
            ResponseBuilder = responseBuilder;
        }

        public async Task<ChangeProductResponse> Handle(ChangeProductRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting ChangeProductRequest processing for product ID: {ProductId}, ProductCode: {ProductCode}", 
                request.Id, request.ProductCode);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for ChangeProductRequest. ProductId: {ProductId}, ProductCode: {ProductCode}, Errors: {Errors}", 
                    request.Id, request.ProductCode, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<ChangeProductResponse, Product>(errorMessage, errors);
            }

            try
            {
                Logger.LogDebug("Checking if product exists. ProductId: {ProductId}", request.Id);
                
                // Verificar se o produto existe
                var productExistsSpec = new ProductMustExistByIdSpecification(ProductReadRepository);
                var productExists = await productExistsSpec.IsSatisfiedByAsync(new Product { Id = request.Id });
                if (!productExists)
                {
                    throw new ProductNotFoundException(request.Id.ToString());
                }

                Logger.LogDebug("Product found, retrieving current data. ProductId: {ProductId}", request.Id);
                var actualEntity = await ProductReadRepository.GetByIdAsync(request.Id);
                Mapper.Map(request, actualEntity);
                actualEntity.UpdatedAt = DateTime.Now;
                actualEntity.UpdatedBy = "ManualMovementsManager Api";

                Logger.LogDebug("Product entity mapped successfully. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    actualEntity.Id, actualEntity.ProductCode);

                // Aplicar specifications de negócio (excluindo o produto atual)
                var specifications = new List<IAsyncSpecification<Product>>
                {
                    new ProductCodeMustBeUniqueForUpdateSpecification(ProductReadRepository, request.Id)
                };

                Logger.LogDebug("Validating business rules for product update. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    actualEntity.Id, actualEntity.ProductCode);

                foreach (var specification in specifications)
                {
                    var isSatisfied = await specification.IsSatisfiedByAsync(actualEntity);
                    if (!isSatisfied)
                    {
                        throw new ProductCodeAlreadyExistsException(actualEntity.ProductCode);
                    }
                }

                Logger.LogDebug("Business rules validated successfully. Proceeding to update product. ProductId: {ProductId}", 
                    actualEntity.Id);

                var result = await Repository.Update(actualEntity);

                if (result)
                {
                    Logger.LogInformation("Product updated successfully. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                        actualEntity.Id, actualEntity.ProductCode);
                    return ResponseBuilder.BuildSuccessResponse<ChangeProductResponse, Product>(actualEntity, "Product updated successfully");
                }

                Logger.LogError("Failed to update product in repository. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    actualEntity.Id, actualEntity.ProductCode);
                return ResponseBuilder.BuildErrorResponse<ChangeProductResponse, Product>("Failed to update product");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while updating product. ProductId: {ProductId}, ProductCode: {ProductCode}", 
                    request.Id, request.ProductCode);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 