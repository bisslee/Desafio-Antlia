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

namespace ManualMovementsManager.Application.Queries.Products.GetProduct
{
    public class GetProductHandler : IRequestHandler<GetProductRequest, GetProductResponse>
    {
        private readonly ILogger<GetProductHandler> Logger;
        private readonly IValidator<GetProductRequest> Validator;
        private readonly IReadRepository<Product> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetProductHandler(
            IReadRepository<Product> repository,
            IValidator<GetProductRequest> validator,
            ILogger<GetProductHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Validator = validator;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetProductResponse> Handle(
            GetProductRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetProductRequest processing. Filters - ProductCode: {ProductCode}, Description: {Description}", 
                request.ProductCode ?? "null", request.Description ?? "null");

            request = request.LoadPagination();

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                Logger.LogWarning("Validation failed for GetProductRequest. Errors: {Errors}", string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<GetProductResponse, List<Product>>(null, errors);
            }

            try
            {
                Logger.LogDebug("Building search predicate for products");
                
                Expression<Func<Product, bool>> predicate = product =>
                (string.IsNullOrEmpty(request.ProductCode) || product.ProductCode.Contains(request.ProductCode))
                && (string.IsNullOrEmpty(request.Description) || product.Description.Contains(request.Description))
                && (request.Active == null || product.Status == (request.Active.Value ? DataStatus.Active : DataStatus.Inactive));

                if (request.FieldName == null)
                {
                    request.FieldName = "ProductCode";
                }

                Logger.LogDebug("Executing paginated search. Page: {Page}, Offset: {Offset}, OrderBy: {FieldName}, Order: {Order}", 
                    request.Page, request.Offset, request.FieldName, request.Order);

                var (products, totalItems) = await Repository.FindWithPagination
                    (
                        predicate,
                        request.Page,
                        request.Offset,
                        request.FieldName,
                        request.Order
                    );

                Logger.LogDebug("Search completed. Found {Count} products out of {TotalItems} total", 
                    products?.Count ?? 0, totalItems);

                var response = ResponseBuilder.BuildSuccessResponse<GetProductResponse, List<Product>>(products);
                response.Page = request.Page;
                response.Total = totalItems;
                response.PageSize = request.Offset;

                if (products is null || !products.Any())
                {
                    Logger.LogInformation("No products found matching the search criteria");
                    response.Message = "No records found";
                    response.StatusCode = 204;
                }
                else
                {
                    Logger.LogInformation("Successfully retrieved {Count} products", products.Count);
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving products");
                return ResponseBuilder.BuildErrorResponse<GetProductResponse, List<Product>>("An error occurred while retrieving products");
            }
        }
    }
} 