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

namespace ManualMovementsManager.Application.Commands.ProductCosifs.AddProductCosif
{
    public class AddProductCosifHandler : IRequestHandler<AddProductCosifRequest, AddProductCosifResponse>
    {
        private readonly ILogger<AddProductCosifHandler> Logger;
        private readonly IWriteRepository<ProductCosif> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IProductCosifReadRepository ProductCosifReadRepository;
        private readonly IValidator<AddProductCosifRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public AddProductCosifHandler(
            ILogger<AddProductCosifHandler> logger,
            IWriteRepository<ProductCosif> repository,
            IProductReadRepository productReadRepository,
            IProductCosifReadRepository productCosifReadRepository,
            IValidator<AddProductCosifRequest> validator,
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

        public async Task<AddProductCosifResponse> Handle(AddProductCosifRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting AddProductCosifRequest processing for product code: {ProductCode}, cosif code: {CosifCode}", 
                request.ProductCode, request.CosifCode);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);

            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for AddProductCosifRequest. ProductCode: {ProductCode}, CosifCode: {CosifCode}, Errors: {Errors}", 
                    request.ProductCode, request.CosifCode, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<AddProductCosifResponse, ProductCosif>(errorMessage, errors);
            }

            try
            {
                var entity = Mapper.Map<ProductCosif>(request);
                entity.Id = Guid.NewGuid();
                entity.Status = DataStatus.Active;
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = "ManualMovementsManager Api";

                Logger.LogDebug("ProductCosif entity mapped successfully. ID: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    entity.Id, entity.ProductCode, entity.CosifCode);

                // Aplicar specifications de negócio
                Logger.LogDebug("Validating business rules for product cosif. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    entity.ProductCode, entity.CosifCode);

                // Validar se o produto existe
                var productSpecification = new ProductMustExistSpecification(ProductReadRepository);
                var productExists = await productSpecification.IsSatisfiedByAsync(entity.ProductCode);
                if (!productExists)
                {
                    throw new ProductNotFoundException(entity.ProductCode);
                }

                // Validar se o COSIF é único
                var productCosifSpecification = new ProductCosifCodeMustBeUniqueSpecification(ProductCosifReadRepository);
                var isCosifUnique = await productCosifSpecification.IsSatisfiedByAsync(entity);
                if (!isCosifUnique)
                {
                    throw new ProductCosifCodeAlreadyExistsException(entity.ProductCode, entity.CosifCode);
                }

                Logger.LogDebug("Business rules validated successfully. Proceeding to save product cosif. ID: {ProductCosifId}", 
                    entity.Id);

                var result = await Repository.Add(entity);

                if (result)
                {
                    Logger.LogInformation("ProductCosif created successfully. ID: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                        entity.Id, entity.ProductCode, entity.CosifCode);
                    return ResponseBuilder.BuildSuccessResponse<AddProductCosifResponse, ProductCosif>(entity, "Product COSIF created successfully", 201);
                }
                
                Logger.LogError("Failed to create product cosif in repository. ID: {ProductCosifId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    entity.Id, entity.ProductCode, entity.CosifCode);
                return ResponseBuilder.BuildErrorResponse<AddProductCosifResponse, ProductCosif>("Failed to create product COSIF");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while creating product cosif. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    request.ProductCode, request.CosifCode);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 