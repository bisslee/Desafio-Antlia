using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.ManualMovements;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.ManualMovements.RemoveManualMovement
{
    public class RemoveManualMovementHandler :
        IRequestHandler<RemoveManualMovementRequest, RemoveManualMovementResponse>
    {
        private readonly ILogger<RemoveManualMovementHandler> Logger;
        private readonly IWriteRepository<ManualMovement> Repository;
        private readonly IManualMovementReadRepository ManualMovementReadRepository;
        private readonly IResponseBuilder ResponseBuilder;
        
        public RemoveManualMovementHandler(
            ILogger<RemoveManualMovementHandler> logger,
            IWriteRepository<ManualMovement> repository,
            IManualMovementReadRepository manualMovementReadRepository,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ManualMovementReadRepository = manualMovementReadRepository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<RemoveManualMovementResponse> Handle(RemoveManualMovementRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting RemoveManualMovementRequest processing for manual movement ID: {ManualMovementId}", request.Id);

            try
            {
                Logger.LogDebug("Checking if manual movement exists for removal. ManualMovementId: {ManualMovementId}", request.Id);
                
                // Verificar se o manual movement existe
                var manualMovementExistsSpec = new ManualMovementMustExistByIdSpecification(ManualMovementReadRepository);
                await SpecificationHandler.ValidateAsync<ManualMovement>(manualMovementExistsSpec, new ManualMovement { Id = request.Id });

                Logger.LogDebug("Manual movement found, retrieving data for removal. ManualMovementId: {ManualMovementId}", request.Id);
                var entity = await ManualMovementReadRepository.GetByIdAsync(request.Id);
                
                Logger.LogDebug("Proceeding to delete manual movement. ManualMovementId: {ManualMovementId}, Description: {Description}", 
                    entity.Id, entity.Description);
                
                var result = await Repository.Delete(entity);
                
                if (result)
                {
                    Logger.LogInformation("Manual movement removed successfully. ManualMovementId: {ManualMovementId}, Description: {Description}", 
                        entity.Id, entity.Description);
                    return ResponseBuilder.BuildSuccessResponse<RemoveManualMovementResponse, bool>(true, "Manual movement removed successfully");
                }

                Logger.LogError("Failed to remove manual movement from repository. ManualMovementId: {ManualMovementId}, Description: {Description}", 
                    entity.Id, entity.Description);
                return ResponseBuilder.BuildErrorResponse<RemoveManualMovementResponse, bool>("Failed to remove manual movement");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while removing manual movement. ManualMovementId: {ManualMovementId}", request.Id);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
} 