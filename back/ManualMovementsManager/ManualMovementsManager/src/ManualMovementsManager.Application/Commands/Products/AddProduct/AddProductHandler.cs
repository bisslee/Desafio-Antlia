using AutoMapper;
using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.DTOs;
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

namespace ManualMovementsManager.Application.Commands.Products.AddProduct
{
    public class AddProductHandler : IRequestHandler<AddProductRequest, AddProductResponse>
    {
        private readonly ILogger<AddProductHandler> Logger;
        private readonly IWriteRepository<Product> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IValidator<AddProductRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public AddProductHandler(
            ILogger<AddProductHandler> logger,
            IWriteRepository<Product> repository,
            IProductReadRepository productReadRepository,
            IValidator<AddProductRequest> validator,
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

        public async Task<AddProductResponse> Handle(AddProductRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting AddProductRequest processing for product code: {ProductCode}", request.ProductCode);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);

            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for AddProductRequest. ProductCode: {ProductCode}, Errors: {Errors}", 
                    request.ProductCode, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<AddProductResponse, ProductDto>(errorMessage, errors);
            }

            try
            {
                var entity = Mapper.Map<Product>(request);
                entity.Id = Guid.NewGuid();
                entity.Status = DataStatus.Active;
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = "ManualMovementsManager Api";

                Logger.LogDebug("Product entity mapped successfully. ID: {ProductId}, ProductCode: {ProductCode}", 
                    entity.Id, entity.ProductCode);

                // Aplicar specifications de negócio
                var specifications = new List<IAsyncSpecification<Product>>
                {
                    new ProductCodeMustBeUniqueSpecification(ProductReadRepository)
                };

                Logger.LogDebug("Validating business rules for product. ProductCode: {ProductCode}", 
                    entity.ProductCode);

                foreach (var specification in specifications)
                {
                    var isSatisfied = await specification.IsSatisfiedByAsync(entity);
                    if (!isSatisfied)
                    {
                        throw new ProductCodeAlreadyExistsException(entity.ProductCode);
                    }
                }

                Logger.LogDebug("Business rules validated successfully. Proceeding to save product. ID: {ProductId}", 
                    entity.Id);

                var result = await Repository.Add(entity);

                if (result)
                {
                    Logger.LogInformation("Product created successfully. ID: {ProductId}, ProductCode: {ProductCode}", 
                        entity.Id, entity.ProductCode);
                    var dto = Mapper.Map<ProductDto>(entity);
                    return ResponseBuilder.BuildSuccessResponse<AddProductResponse, ProductDto>(dto, "Product created successfully", 201);
                }
                
                Logger.LogError("Failed to create product in repository. ID: {ProductId}, ProductCode: {ProductCode}", 
                    entity.Id, entity.ProductCode);
                return ResponseBuilder.BuildErrorResponse<AddProductResponse, ProductDto>("Failed to create product");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while creating product. ProductCode: {ProductCode}", request.ProductCode);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 