using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Constants;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Queries.Customers.GetCustomer
{
    public class GetCustomerHandler : IRequestHandler<GetCustomerRequest, GetCustomerResponse>
    {
        private readonly ILogger<GetCustomerHandler> Logger;
        private readonly IValidator<GetCustomerRequest> Validator;
        private readonly IReadRepository<Customer> Repository;
        private readonly IResponseBuilder ResponseBuilder;

        public GetCustomerHandler(
            IReadRepository<Customer> repository,
            IValidator<GetCustomerRequest> validator,
            ILogger<GetCustomerHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Validator = validator;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetCustomerResponse> Handle(
            GetCustomerRequest request,
            CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetCustomerRequest processing. Filters - FullName: {FullName}, Email: {Email}, Document: {Document}, Phone: {Phone}", 
                request.FullName ?? "null", request.Email ?? "null", request.DocumentNumber ?? "null", request.Phone ?? "null");

            request = request.LoadPagination();

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                Logger.LogWarning("Validation failed for GetCustomerRequest. Errors: {Errors}", string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<GetCustomerResponse, List<Customer>>(null, errors);
            }

            try
            {
                Logger.LogDebug("Building search predicate for customers");
                
                Expression<Func<Customer, bool>> predicate = customer =>
                (string.IsNullOrEmpty(request.FullName) || customer.FullName.Contains(request.FullName))
                && (string.IsNullOrEmpty(request.Email) || customer.Email.Contains(request.Email))
                && (string.IsNullOrEmpty(request.DocumentNumber) || customer.DocumentNumber.Contains(request.DocumentNumber))
                && (string.IsNullOrEmpty(request.Phone) || customer.Phone.Contains(request.Phone))
                && (request.StartBirthDate == null || customer.BirthDate >= request.StartBirthDate)
                    && (request.EndBirthDate == null || customer.BirthDate <= request.EndBirthDate);

                if (request.FieldName == null)
                {
                    request.FieldName = "FullName";
                }

                Logger.LogDebug("Executing paginated search. Page: {Page}, Offset: {Offset}, OrderBy: {FieldName}, Order: {Order}", 
                    request.Page, request.Offset, request.FieldName, request.Order);

                var (customers, totalItems) = await Repository.FindWithPagination
                    (
                        predicate,
                        request.Page,
                        request.Offset,
                        request.FieldName,
                        request.Order
                    );

                Logger.LogDebug("Search completed. Found {Count} customers out of {TotalItems} total", 
                    customers?.Count ?? 0, totalItems);

                var response = ResponseBuilder.BuildSuccessResponse<GetCustomerResponse, List<Customer>>(customers);
                response.Page = request.Page;
                response.Total = totalItems;
                response.PageSize = request.Offset;

                if (customers is null || !customers.Any())
                {
                    Logger.LogInformation("No customers found matching the search criteria");
                    response.Message = "No records found";
                    response.StatusCode = 204;
                }
                else
                {
                    Logger.LogInformation("Successfully retrieved {Count} customers", customers.Count);
                }

                return response;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving customers");
                return ResponseBuilder.BuildErrorResponse<GetCustomerResponse, List<Customer>>("An error occurred while retrieving customers");
            }
        }
    }
}
