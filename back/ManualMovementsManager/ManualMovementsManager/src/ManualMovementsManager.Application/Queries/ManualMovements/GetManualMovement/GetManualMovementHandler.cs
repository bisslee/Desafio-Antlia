using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Constants;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Entities.Enums;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement
{
    public class GetManualMovementHandler : IRequestHandler<GetManualMovementRequest, GetManualMovementResponse>
    {
        private readonly ILogger<GetManualMovementHandler> Logger;
        private readonly IValidator<GetManualMovementRequest> Validator;
        private readonly IReadRepository<ManualMovement> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetManualMovementHandler(
            IReadRepository<ManualMovement> repository,
            IValidator<GetManualMovementRequest> validator,
            ILogger<GetManualMovementHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Validator = validator;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetManualMovementResponse> Handle(
            GetManualMovementRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetManualMovementRequest processing. Filters - Description: {Description}, StartDate: {StartDate}, EndDate: {EndDate}", 
                request.Description ?? "null", request.StartDate?.ToString("yyyy-MM-dd") ?? "null", request.EndDate?.ToString("yyyy-MM-dd") ?? "null");

            request = request.LoadPagination();

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                Logger.LogWarning("Validation failed for GetManualMovementRequest. Errors: {Errors}", string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<GetManualMovementResponse, List<ManualMovement>>(null, errors);
            }

            try
            {
                Logger.LogDebug("Building search predicate for manual movements");
                
                Expression<Func<ManualMovement, bool>> predicate = manualMovement =>
                (string.IsNullOrEmpty(request.Description) || manualMovement.Description.Contains(request.Description))
                && (request.StartDate == null || manualMovement.MovementDate >= request.StartDate)
                && (request.EndDate == null || manualMovement.MovementDate <= request.EndDate)
                && (request.MinValue == null || manualMovement.Value >= request.MinValue)
                && (request.MaxValue == null || manualMovement.Value <= request.MaxValue)
                && (request.Active == null || manualMovement.Status == (request.Active.Value ? DataStatus.Active : DataStatus.Inactive));

                if (request.FieldName == null)
                {
                    request.FieldName = "MovementDate";
                }

                Logger.LogDebug("Executing paginated search. Page: {Page}, Offset: {Offset}, OrderBy: {FieldName}, Order: {Order}", 
                    request.Page, request.Offset, request.FieldName, request.Order);

                var (manualMovements, totalItems) = await Repository.FindWithPagination
                    (
                        predicate,
                        request.Page,
                        request.Offset,
                        request.FieldName,
                        request.Order
                    );

                Logger.LogDebug("Search completed. Found {Count} manual movements out of {TotalItems} total", 
                    manualMovements?.Count ?? 0, totalItems);

                var response = ResponseBuilder.BuildSuccessResponse<GetManualMovementResponse, List<ManualMovement>>(manualMovements);
                response.Page = request.Page;
                response.Total = totalItems;
                response.PageSize = request.Offset;

                if (manualMovements is null || !manualMovements.Any())
                {
                    Logger.LogInformation("No manual movements found matching the search criteria");
                    response.Message = "No records found";
                    response.StatusCode = 204;
                }
                else
                {
                    Logger.LogInformation("Successfully retrieved {Count} manual movements", manualMovements.Count);
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving manual movements");
                return ResponseBuilder.BuildErrorResponse<GetManualMovementResponse, List<ManualMovement>>("An error occurred while retrieving manual movements");
            }
        }
    }
} 