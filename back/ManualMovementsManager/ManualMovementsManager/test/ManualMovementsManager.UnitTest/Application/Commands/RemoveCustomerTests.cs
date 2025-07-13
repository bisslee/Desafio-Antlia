using FluentAssertions;
using FluentValidation;
using ManualMovementsManager.Application.Commands.Customers.RemoveCustomer;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Resources;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class RemoveCustomerTests : BaseTest
    {
        private readonly Mock<ILogger<RemoveCustomerHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<Customer>> WriteRepositoryMock;
        private readonly Mock<ICustomerReadRepository> CustomerReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;
        
        private readonly RemoveCustomerHandler Handler;

        public RemoveCustomerTests()
        {
            LoggerMock = new Mock<ILogger<RemoveCustomerHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<Customer>>();
            CustomerReadRepositoryMock = new Mock<ICustomerReadRepository>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new RemoveCustomerHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                CustomerReadRepositoryMock.Object,
                ResponseBuilder
            );
        }

        private RemoveCustomerRequest CreateValidRequest()
        {
            return new RemoveCustomerRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Remove_Customer_Successfully()
        {
            var request = CreateValidRequest();
            var customer = new Customer { Id = request.Id };

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(customer);
            CustomerReadRepositoryMock.Setup(r => r.GetCustomerWithAddressByIdAsync(request.Id))
                .ReturnsAsync(customer);

            WriteRepositoryMock.Setup(r => r.Delete(customer))
            .ReturnsAsync(true);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Throw_CustomerNotFoundException_When_Customer_Not_Found()
        {
            var request = CreateValidRequest();

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
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            var request = CreateValidRequest();

            CustomerReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
}

