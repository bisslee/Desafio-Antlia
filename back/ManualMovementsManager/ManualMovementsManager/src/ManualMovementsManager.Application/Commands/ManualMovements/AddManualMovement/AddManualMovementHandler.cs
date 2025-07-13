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
using ManualMovementsManager.Domain.Specifications.ProductCosifs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.ManualMovements.AddManualMovement
{
    public class AddManualMovementHandler : IRequestHandler<AddManualMovementRequest, AddManualMovementResponse>
    {
        private readonly ILogger<AddManualMovementHandler> Logger;
        private readonly IWriteRepository<ManualMovement> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IProductCosifReadRepository ProductCosifReadRepository;
        private readonly IManualMovementReadRepository ManualMovementReadRepository;
        private readonly IValidator<AddManualMovementRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public AddManualMovementHandler(
            ILogger<AddManualMovementHandler> logger,
            IWriteRepository<ManualMovement> repository,
            IProductReadRepository productReadRepository,
            IProductCosifReadRepository productCosifReadRepository,
            IManualMovementReadRepository manualMovementReadRepository,
            IValidator<AddManualMovementRequest> validator,
            IMapper mapper,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ProductReadRepository = productReadRepository;
            ProductCosifReadRepository = productCosifReadRepository;
            ManualMovementReadRepository = manualMovementReadRepository;
            Validator = validator;
            Mapper = mapper;
            ResponseBuilder = responseBuilder;
        }

        public async Task<AddManualMovementResponse> Handle(AddManualMovementRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting AddManualMovementRequest processing for product code: {ProductCode}, cosif code: {CosifCode}, month: {Month}, year: {Year}", 
                request.ProductCode, request.CosifCode, request.Month, request.Year);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);

            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for AddManualMovementRequest. ProductCode: {ProductCode}, CosifCode: {CosifCode}, Errors: {Errors}", 
                    request.ProductCode, request.CosifCode, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<AddManualMovementResponse, ManualMovementDto>(errorMessage, errors);
            }

            try
            {
                var entity = Mapper.Map<ManualMovement>(request);
                entity.Id = Guid.NewGuid();
                entity.Status = DataStatus.Active;
                entity.CreatedAt = DateTime.Now;
                entity.CreatedBy = "ManualMovementsManager Api";

                // Gerar número de lançamento sequencial
                entity.LaunchNumber = await ManualMovementReadRepository.GetNextLaunchNumberAsync(request.Month, request.Year);

                Logger.LogDebug("ManualMovement entity mapped successfully. ID: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}, LaunchNumber: {LaunchNumber}", 
                    entity.Id, entity.ProductCode, entity.CosifCode, entity.LaunchNumber);

                // Aplicar specifications de negócio
                Logger.LogDebug("Validating business rules for manual movement. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    entity.ProductCode, entity.CosifCode);

                // Validar se o produto existe
                var productSpecification = new ProductMustExistSpecification(ProductReadRepository);
                var productExists = await productSpecification.IsSatisfiedByAsync(entity.ProductCode);
                if (!productExists)
                {
                    throw new ProductNotFoundException(entity.ProductCode);
                }

                // Validar se o ProductCosif existe
                var productCosifSpecification = new ProductCosifMustExistSpecification(ProductCosifReadRepository);
                var productCosifExists = await productCosifSpecification.IsSatisfiedByAsync((entity.ProductCode, entity.CosifCode));
                if (!productCosifExists)
                {
                    throw new ProductCosifNotFoundException(entity.ProductCode, entity.CosifCode);
                }

                Logger.LogDebug("Business rules validated successfully. Proceeding to save manual movement. ID: {ManualMovementId}", 
                    entity.Id);

                var result = await Repository.Add(entity);

                if (result)
                {
                    Logger.LogInformation("ManualMovement created successfully. ID: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}, LaunchNumber: {LaunchNumber}", 
                        entity.Id, entity.ProductCode, entity.CosifCode, entity.LaunchNumber);
                    var dto = Mapper.Map<ManualMovementDto>(entity);
                    return ResponseBuilder.BuildSuccessResponse<AddManualMovementResponse, ManualMovementDto>(dto, "Manual movement created successfully", 201);
                }
                
                Logger.LogError("Failed to create manual movement in repository. ID: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    entity.Id, entity.ProductCode, entity.CosifCode);
                return ResponseBuilder.BuildErrorResponse<AddManualMovementResponse, ManualMovementDto>("Failed to create manual movement");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while creating manual movement. ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    request.ProductCode, request.CosifCode);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 