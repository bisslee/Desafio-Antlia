using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Constants;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Entities.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif
{
    public class GetProductCosifHandler : IRequestHandler<GetProductCosifRequest, GetProductCosifResponse>
    {
        private readonly ILogger<GetProductCosifHandler> Logger;
        private readonly IValidator<GetProductCosifRequest> Validator;
        private readonly IReadRepository<ProductCosif> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetProductCosifHandler(
            IReadRepository<ProductCosif> repository,
            IValidator<GetProductCosifRequest> validator,
            ILogger<GetProductCosifHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Validator = validator;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetProductCosifResponse> Handle(
            GetProductCosifRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetProductCosifRequest processing. Filters - ProductCode: {ProductCode}, CosifCode: {CosifCode}", 
                request.ProductCode ?? "null", request.CosifCode ?? "null");

            request = request.LoadPagination();

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                Logger.LogWarning("Validation failed for GetProductCosifRequest. Errors: {Errors}", string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<GetProductCosifResponse, List<ProductCosif>>(null, errors);
            }

            try
            {
                Logger.LogDebug("Building search predicate for product cosifs");
                
                Expression<Func<ProductCosif, bool>> predicate = productCosif =>
                (string.IsNullOrEmpty(request.ProductCode) || productCosif.ProductCode.Contains(request.ProductCode))
                && (string.IsNullOrEmpty(request.CosifCode) || productCosif.CosifCode.Contains(request.CosifCode))
                && (string.IsNullOrEmpty(request.ClassificationCode) || productCosif.ClassificationCode.Contains(request.ClassificationCode))
                && (request.Active == null || productCosif.Status == (request.Active.Value ? DataStatus.Active : DataStatus.Inactive));

                if (request.FieldName == null)
                {
                    request.FieldName = "ProductCode";
                }

                Logger.LogDebug("Executing paginated search. Page: {Page}, Offset: {Offset}, OrderBy: {FieldName}, Order: {Order}", 
                    request.Page, request.Offset, request.FieldName, request.Order);

                var (productCosifs, totalItems) = await Repository.FindWithPagination
                    (
                        predicate,
                        request.Page,
                        request.Offset,
                        request.FieldName,
                        request.Order
                    );

                Logger.LogDebug("Search completed. Found {Count} product cosifs out of {TotalItems} total", 
                    productCosifs?.Count ?? 0, totalItems);

                var response = ResponseBuilder.BuildSuccessResponse<GetProductCosifResponse, List<ProductCosif>>(productCosifs);
                response.Page = request.Page;
                response.Total = totalItems;
                response.PageSize = request.Offset;

                if (productCosifs is null || !productCosifs.Any())
                {
                    Logger.LogInformation("No product cosifs found matching the search criteria");
                    response.Message = "No records found";
                    response.StatusCode = 204;
                }
                else
                {
                    Logger.LogInformation("Successfully retrieved {Count} product cosifs", productCosifs.Count);
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving product cosifs");
                return ResponseBuilder.BuildErrorResponse<GetProductCosifResponse, List<ProductCosif>>("An error occurred while retrieving product cosifs");
            }
        }
    }
} 