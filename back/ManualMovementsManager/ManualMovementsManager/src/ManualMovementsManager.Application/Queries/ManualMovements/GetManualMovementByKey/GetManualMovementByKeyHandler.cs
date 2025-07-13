using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovementByKey
{
    public class GetManualMovementByKeyHandler : IRequestHandler<GetManualMovementByKeyRequest, GetManualMovementByKeyResponse>
    {
        private readonly ILogger<GetManualMovementByKeyHandler> Logger;
        private readonly IReadRepository<ManualMovement> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetManualMovementByKeyHandler(
            IReadRepository<ManualMovement> repository,
            ILogger<GetManualMovementByKeyHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetManualMovementByKeyResponse> Handle(
            GetManualMovementByKeyRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetManualMovementByKeyRequest processing for manual movement ID: {ManualMovementId}", request.Id);

            try
            {
                Logger.LogDebug("Retrieving manual movement by ID: {ManualMovementId}", request.Id);
                
                var manualMovement = await Repository.GetByIdAsync(request.Id);
                
                if (manualMovement == null)
                {
                    Logger.LogWarning("Manual movement not found for ID: {ManualMovementId}", request.Id);
                    return ResponseBuilder.BuildErrorResponse<GetManualMovementByKeyResponse, ManualMovement>("Manual movement not found");
                }

                Logger.LogInformation("Successfully retrieved manual movement. ManualMovementId: {ManualMovementId}, Description: {Description}, Date: {Date}", 
                    manualMovement.Id, manualMovement.Description, manualMovement.MovementDate.ToString("yyyy-MM-dd"));

                return ResponseBuilder.BuildSuccessResponse<GetManualMovementByKeyResponse, ManualMovement>(manualMovement);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving manual movement by ID: {ManualMovementId}", request.Id);
                return ResponseBuilder.BuildErrorResponse<GetManualMovementByKeyResponse, ManualMovement>("An error occurred while retrieving the manual movement");
            }
        }
    }
} 