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
using ManualMovementsManager.Domain.Specifications.ManualMovements;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.ManualMovements.ChangeManualMovement
{
    public class ChangeManualMovementHandler : IRequestHandler<ChangeManualMovementRequest, ChangeManualMovementResponse>
    {
        private readonly ILogger<ChangeManualMovementHandler> Logger;
        private readonly IWriteRepository<ManualMovement> Repository;
        private readonly IProductReadRepository ProductReadRepository;
        private readonly IProductCosifReadRepository ProductCosifReadRepository;
        private readonly IManualMovementReadRepository ManualMovementReadRepository;
        private readonly IValidator<ChangeManualMovementRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public ChangeManualMovementHandler(
            ILogger<ChangeManualMovementHandler> logger,
            IWriteRepository<ManualMovement> repository,
            IProductReadRepository productReadRepository,
            IProductCosifReadRepository productCosifReadRepository,
            IManualMovementReadRepository manualMovementReadRepository,
            IValidator<ChangeManualMovementRequest> validator,
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

        public async Task<ChangeManualMovementResponse> Handle(ChangeManualMovementRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting ChangeManualMovementRequest processing for manual movement ID: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                request.Id, request.ProductCode, request.CosifCode);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for ChangeManualMovementRequest. ManualMovementId: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}, Errors: {Errors}", 
                    request.Id, request.ProductCode, request.CosifCode, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<ChangeManualMovementResponse, ManualMovement>(errorMessage, errors);
            }

            try
            {
                Logger.LogDebug("Checking if manual movement exists. ManualMovementId: {ManualMovementId}", request.Id);
                
                // Verificar se o ManualMovement existe
                var manualMovementExistsSpec = new ManualMovementMustExistByIdSpecification(ManualMovementReadRepository);
                var manualMovementExists = await manualMovementExistsSpec.IsSatisfiedByAsync(new ManualMovement { Id = request.Id });
                if (!manualMovementExists)
                {
                    throw new ManualMovementNotFoundException(request.Id);
                }

                Logger.LogDebug("Manual movement found, retrieving current data. ManualMovementId: {ManualMovementId}", request.Id);
                var actualEntity = await ManualMovementReadRepository.GetByIdAsync(request.Id);
                Mapper.Map(request, actualEntity);
                actualEntity.UpdatedAt = DateTime.Now;
                actualEntity.UpdatedBy = "ManualMovementsManager Api";

                Logger.LogDebug("Manual movement entity mapped successfully. ManualMovementId: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);

                // Verificar se o produto existe
                var productSpecification = new ProductMustExistSpecification(ProductReadRepository);
                var productExists = await productSpecification.IsSatisfiedByAsync(actualEntity.ProductCode);
                if (!productExists)
                {
                    throw new ProductNotFoundException(actualEntity.ProductCode);
                }

                // Verificar se o ProductCosif existe
                var productCosifSpecification = new ProductCosifMustExistSpecification(ProductCosifReadRepository);
                var productCosifExists = await productCosifSpecification.IsSatisfiedByAsync((actualEntity.ProductCode, actualEntity.CosifCode));
                if (!productCosifExists)
                {
                    throw new ProductCosifNotFoundException(actualEntity.ProductCode, actualEntity.CosifCode);
                }

                Logger.LogDebug("Business rules validated successfully. Proceeding to update manual movement. ManualMovementId: {ManualMovementId}", 
                    actualEntity.Id);

                var result = await Repository.Update(actualEntity);

                if (result)
                {
                    Logger.LogInformation("Manual movement updated successfully. ManualMovementId: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                        actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);
                    return ResponseBuilder.BuildSuccessResponse<ChangeManualMovementResponse, ManualMovement>(actualEntity, "Manual movement updated successfully");
                }

                Logger.LogError("Failed to update manual movement in repository. ManualMovementId: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    actualEntity.Id, actualEntity.ProductCode, actualEntity.CosifCode);
                return ResponseBuilder.BuildErrorResponse<ChangeManualMovementResponse, ManualMovement>("Failed to update manual movement");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while updating manual movement. ManualMovementId: {ManualMovementId}, ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                    request.Id, request.ProductCode, request.CosifCode);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 