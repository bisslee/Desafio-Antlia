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
using ManualMovementsManager.Domain.Specifications.ProductCosifs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.ProductCosifs.ChangeProductCosif
{
    public class ChangeProductCosifHandler : IRequestHandler<ChangeProductCosifRequest, ChangeProductCosifResponse>
    {
        private readonly ILogger<ChangeProductCosifHandler> Logger;
        private readonly IWriteRepository<ProductCosif> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IProductCosifReadRepository ProductCosifReadRepository;
        private readonly IValidator<ChangeProductCosifRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public ChangeProductCosifHandler(
            ILogger<ChangeProductCosifHandler> logger,
            IWriteRepository<ProductCosif> repository,
            IProductReadRepository productReadRepository,
            IProductCosifReadRepository productCosifReadRepository,
            IValidator<ChangeProductCosifRequest> validator,
            IMapper mapper,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ProductReadRepository = productReadRepository;
            ProductCosifReadRepository = productCosifReadRepository;
            Validator = validator;
            Mapper = mapper;
            ResponseBuilder = responseBuilder;
        }

        public async Task<ChangeProductCosifResponse> Handle(ChangeProductCosifRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting ChangeProductCosifRequest processing for product cosif ID: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                request.Id, request.ProductCode, request.CosifCode);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for ChangeProductCosifRequest. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}, Errors: {Errors}", 
                    request.Id, request.ProductCode, request.CosifCode, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<ChangeProductCosifResponse, ProductCosif>(errorMessage, errors);
            }

            try
            {
                Logger.LogDebug("Checking if product cosif exists. ProductCosifId: {ProductCosifId}", request.Id);
                
                // Verificar se o ProductCosif existe
                var productCosifExistsSpec = new ProductCosifMustExistByIdSpecification(ProductCosifReadRepository);
                var productCosifExists = await productCosifExistsSpec.IsSatisfiedByAsync(new ProductCosif { Id = request.Id });
                if (!productCosifExists)
                {
                    throw new ProductCosifNotFoundException(request.Id.ToString(), request.Id.ToString());
                }

                Logger.LogDebug("Product cosif found, retrieving current data. ProductCosifId: {ProductCosifId}", request.Id);
                var actualEntity = await ProductCosifReadRepository.GetByIdAsync(request.Id);
                Mapper.Map(request, actualEntity);
                actualEntity.UpdatedAt = DateTime.Now;
                actualEntity.UpdatedBy = "ManualMovementsManager Api";

                Logger.LogDebug("Product cosif entity mapped successfully. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);

                // Verificar se o produto existe
                var productSpecification = new ProductMustExistSpecification(ProductReadRepository);
                var productExists = await productSpecification.IsSatisfiedByAsync(actualEntity.ProductCode);
                if (!productExists)
                {
                    throw new ProductNotFoundException(actualEntity.ProductCode);
                }

                // Aplicar specifications de negócio (excluindo o ProductCosif atual)
                var specifications = new List<IAsyncSpecification<ProductCosif>>
                {
                    new ProductCosifCodeMustBeUniqueForUpdateSpecification(ProductCosifReadRepository, request.Id)
                };

                Logger.LogDebug("Validating business rules for product cosif update. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);

                foreach (var specification in specifications)
                {
                    var isSatisfied = await specification.IsSatisfiedByAsync(actualEntity);
                    if (!isSatisfied)
                    {
                        throw new ProductCosifCodeAlreadyExistsException(actualEntity.ProductCode, actualEntity.CosifCode);
                    }
                }

                Logger.LogDebug("Business rules validated successfully. Proceeding to update product cosif. ProductCosifId: {ProductCosifId}", 
                    actualEntity.Id);

                var result = await Repository.Update(actualEntity);

                if (result)
                {
                    Logger.LogInformation("Product cosif updated successfully. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                        actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);
                    return ResponseBuilder.BuildSuccessResponse<ChangeProductCosifResponse, ProductCosif>(actualEntity, "Product COSIF updated successfully");
                }

                Logger.LogError("Failed to update product cosif in repository. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);
                return ResponseBuilder.BuildErrorResponse<ChangeProductCosifResponse, ProductCosif>("Failed to update product COSIF");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while updating product cosif. ProductCosifId: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    request.Id, request.ProductCode, request.CosifCode);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 