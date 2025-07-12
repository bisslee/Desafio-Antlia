using FluentValidation;
using MediatR;
using ManualMovements.Application.Helpers;
using ManualMovements.Domain.Entities;
using ManualMovements.Domain.Repositories;
using ManualMovements.Domain.Specifications;
using ManualMovements.Domain.Specifications.Customers;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovements.Application.Queries.Customers.GetCustomerByKey
{
    public class GetCustomerByKeyHandler : IRequestHandler<GetCustomerByKeyRequest, GetCustomerByKeyResponse>
    {
        private readonly ILogger<GetCustomerByKeyHandler> Logger;
        private readonly IValidator<GetCustomerByKeyRequest> Validator;
        private readonly ICustomerReadRepository Repository;
        private readonly IResponseBuilder ResponseBuilder;
        
        public GetCustomerByKeyHandler(
            ICustomerReadRepository repository,
            IValidator<GetCustomerByKeyRequest> validator,
            ILogger<GetCustomerByKeyHandler> logger,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Validator = validator;
            Repository = repository;
            ResponseBuilder = responseBuilder;
        }

        public async Task<GetCustomerByKeyResponse> Handle(GetCustomerByKeyRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting GetCustomerByKeyRequest processing for customer ID: {CustomerId}", request.Id);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                Logger.LogWarning("Validation failed for GetCustomerByKeyRequest. CustomerId: {CustomerId}, Errors: {Errors}", 
                    request.Id, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<GetCustomerByKeyResponse, Customer>(null, errors);
            }

            try
            {
                Logger.LogDebug("Checking if customer exists. CustomerId: {CustomerId}", request.Id);
                
                // Verificar se o customer existe
                var customerExistsSpec = new CustomerMustExistSpecification(Repository);
                await SpecificationHandler.ValidateAsync(customerExistsSpec, new Customer { Id = request.Id });

                Logger.LogDebug("Customer found, retrieving data. CustomerId: {CustomerId}", request.Id);
                var entity = await Repository.GetCustomerWithAddressByIdAsync(request.Id);
                
                Logger.LogInformation("Successfully retrieved customer. CustomerId: {CustomerId}, Email: {Email}", 
                    entity.Id, entity.Email);
                
                return ResponseBuilder.BuildSuccessResponse<GetCustomerByKeyResponse, Customer>(entity);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while retrieving customer by key. CustomerId: {CustomerId}", request.Id);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
}
