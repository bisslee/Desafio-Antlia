namespace ManualMovements.UnitTest.Application.Queries
{
    using FluentAssertions;
    using FluentValidation;
    using FluentValidation.Results;
    using global::ManualMovements.Application.Helpers;
    using global::ManualMovements.Application.Queries.Customers.GetCustomerByKey;
    using global::ManualMovements.Domain.Entities;
    using global::ManualMovements.Domain.Exceptions;
    using global::ManualMovements.Domain.Repositories;
    using Microsoft.Extensions.Logging;
    using Moq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Xunit;

    namespace ManualMovements.UnitTest.Application.Queries
    {
        public class GetCustomerByKeyTests : BaseTest
        {
            private readonly Mock<ILogger<GetCustomerByKeyHandler>> LoggerMock;
            private readonly Mock<ICustomerReadRepository> CustomerReadRepositoryMock;
            private readonly Mock<IValidator<GetCustomerByKeyRequest>> ValidatorMock;
            private readonly IResponseBuilder ResponseBuilder;

            private readonly GetCustomerByKeyHandler Handler;

            public GetCustomerByKeyTests()
            {
                LoggerMock = new Mock<ILogger<GetCustomerByKeyHandler>>();
                CustomerReadRepositoryMock = new Mock<ICustomerReadRepository>();
                ValidatorMock = new Mock<IValidator<GetCustomerByKeyRequest>>();
                ResponseBuilder = new ResponseBuilder();

                Handler = new GetCustomerByKeyHandler(
                    CustomerReadRepositoryMock.Object,
                    ValidatorMock.Object,
                    LoggerMock.Object,
                    ResponseBuilder
                );
            }

            private GetCustomerByKeyRequest CreateValidRequest()
            {
                return new GetCustomerByKeyRequest
                {
                    Id = Guid.NewGuid()
                };
            }

            [Fact]
            public async Task Handle_Should_Return_Customer_Successfully()
            {
                var request = CreateValidRequest();
                var customer = new Customer { Id = request.Id };

                ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult());

                CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                    .ReturnsAsync(customer);
                CustomerReadRepositoryMock.Setup(r => r.GetCustomerWithAddressByIdAsync(request.Id))
                    .ReturnsAsync(customer);

                var response = await Handler.Handle(request, CancellationToken.None);

                response.Success.Should().BeTrue();
                response.StatusCode.Should().Be(200);
                response.Data.Should().NotBeNull();
            }

            [Fact]
            public async Task Handle_Should_Throw_CustomerNotFoundException_When_Customer_Not_Exist()
            {
                var request = CreateValidRequest();

                ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult());

                CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                    .ReturnsAsync((Customer?)null);

                // Act & Assert
                var exception = await Assert.ThrowsAsync<CustomerNotFoundException>(
                    () => Handler.Handle(request, CancellationToken.None));

                exception.Message.Should().Contain("was not found");
                exception.ErrorCode.Should().Be("CUSTOMER_NOT_FOUND");
                exception.StatusCode.Should().Be(404);
            }

            [Fact]
            public async Task Handle_Should_Return_BadRequest_When_Validation_Fails()
            {
                var request = CreateValidRequest();
                var validationFailures = new List<ValidationFailure> { new ValidationFailure("Id", "Id inválido") };

                ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult(validationFailures));

                var response = await Handler.Handle(request, CancellationToken.None);

                response.Success.Should().BeFalse();
                response.StatusCode.Should().Be(400);
                response.Data.Should().BeNull();
            }

            [Fact]
            public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
            {
                var request = CreateValidRequest();

                ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new ValidationResult());

                CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                    .ThrowsAsync(new Exception("Erro interno"));

                // Act & Assert
                var exception = await Assert.ThrowsAsync<Exception>(
                    () => Handler.Handle(request, CancellationToken.None));

                exception.Message.Should().Be("Erro interno");
            }
        }
    }

}
