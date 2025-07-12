using AutoMapper;
using FluentValidation;
using MediatR;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Specifications;
using ManualMovementsManager.Domain.Specifications.Customers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManualMovementsManager.Application.Commands.Customers.ChangeCustomer
{
    public class ChangeCustomerHandler : 
        IRequestHandler<ChangeCustomerRequest, ChangeCustomerResponse>
    {
        private readonly ILogger<ChangeCustomerHandler> Logger;
        private readonly IWriteRepository<Customer> Repository;
        private readonly ICustomerReadRepository CustomerReadRepository;
        private readonly IValidator<ChangeCustomerRequest> Validator;
        private readonly IMapper Mapper;
        private readonly IResponseBuilder ResponseBuilder;

        public ChangeCustomerHandler(           
            ILogger<ChangeCustomerHandler> logger,
            IWriteRepository<Customer> repository,
            ICustomerReadRepository customerReadRepository,
            IValidator<ChangeCustomerRequest> validator,
            IMapper mapper,
            IResponseBuilder responseBuilder
            )
        {
            Logger = logger;
            Repository = repository;
            CustomerReadRepository = customerReadRepository;
            Validator = validator;
            Mapper = mapper;
            ResponseBuilder = responseBuilder;
        }

        public async Task<ChangeCustomerResponse> Handle(ChangeCustomerRequest request, CancellationToken cancellationToken)
        {
            Logger.LogInformation("Starting ChangeCustomerRequest processing for customer ID: {CustomerId}, Email: {Email}", 
                request.Id, request.Email);

            var validationResult = await Validator.ValidateAsync(request, cancellationToken);
            if (validationResult != null && !validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage);
                var errorMessage = string.Join("; ", errors);
                Logger.LogWarning("Validation failed for ChangeCustomerRequest. CustomerId: {CustomerId}, Email: {Email}, Errors: {Errors}", 
                    request.Id, request.Email, string.Join(", ", errors));
                return ResponseBuilder.BuildValidationErrorResponse<ChangeCustomerResponse, Customer>(errorMessage, errors);
            }

            try
            {
                Logger.LogDebug("Checking if customer exists. CustomerId: {CustomerId}", request.Id);
                
                // Verificar se o customer existe
                var customerExistsSpec = new CustomerMustExistSpecification(CustomerReadRepository);
                await SpecificationHandler.ValidateAsync(customerExistsSpec, new Customer { Id = request.Id });

                Logger.LogDebug("Customer found, retrieving current data. CustomerId: {CustomerId}", request.Id);
                var actualEntity = await CustomerReadRepository.GetCustomerWithAddressByIdAsync(request.Id);
                Mapper.Map(request, actualEntity);
                actualEntity.UpdatedAt = DateTime.Now;
                actualEntity.UpdatedBy = "Template Api";

                Logger.LogDebug("Customer entity mapped successfully. CustomerId: {CustomerId}, Email: {Email}", 
                    actualEntity.Id, actualEntity.Email);

                // Aplicar specifications de negócio (excluindo o customer atual)
                var specifications = new List<IAsyncSpecification<Customer>>
                {
                    new CustomerEmailMustBeUniqueSpecification(CustomerReadRepository, request.Id),
                    new CustomerDocumentMustBeUniqueSpecification(CustomerReadRepository, request.Id)
                };

                Logger.LogDebug("Validating business rules for customer update. CustomerId: {CustomerId}, Email: {Email}, Document: {Document}", 
                    actualEntity.Id, actualEntity.Email, actualEntity.DocumentNumber);

                await SpecificationHandler.ValidateAllAsync(specifications, actualEntity);

                Logger.LogDebug("Business rules validated successfully. Proceeding to update customer. CustomerId: {CustomerId}", 
                    actualEntity.Id);

                var result = await Repository.Update(actualEntity);

                if (result)
                {
                    Logger.LogInformation("Customer updated successfully. CustomerId: {CustomerId}, Email: {Email}", 
                        actualEntity.Id, actualEntity.Email);
                    return ResponseBuilder.BuildSuccessResponse<ChangeCustomerResponse, Customer>(actualEntity, "Customer updated successfully");
                }

                Logger.LogError("Failed to update customer in repository. CustomerId: {CustomerId}, Email: {Email}", 
                    actualEntity.Id, actualEntity.Email);
                return ResponseBuilder.BuildErrorResponse<ChangeCustomerResponse, Customer>("Failed to update customer");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Unexpected error occurred while updating customer. CustomerId: {CustomerId}, Email: {Email}", 
                    request.Id, request.Email);
                throw; // Deixar o middleware global tratar a exceção
            }
        }
    }
}
