using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.Customers.GetCustomer;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetCustomerTests : BaseTest
    {
        private readonly Mock<ILogger<GetCustomerHandler>> LoggerMock;
        private readonly Mock<IReadRepository<Customer>> ReadRepositoryMock;
        private readonly Mock<IValidator<GetCustomerRequest>> ValidatorMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetCustomerHandler Handler;

        public GetCustomerTests()
        {
            LoggerMock = new Mock<ILogger<GetCustomerHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<Customer>>();
            ValidatorMock = new Mock<IValidator<GetCustomerRequest>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetCustomerHandler(
                ReadRepositoryMock.Object,
                ValidatorMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetCustomerRequest CreateValidRequest()
        {
            return new GetCustomerRequest
            {
                Page = 1,
                Offset = 10,
                FullName = "Ivana"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Customers_Successfully()
        {
            var request = CreateValidRequest();
            var customers = new List<Customer>
            {
                new Customer { FullName = "Ivana Batista" }
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((customers, customers.Count));

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().NotBeNull();
            response.Data.Should().HaveCount(1);
            response.Page.Should().Be(1);
            response.Total.Should().Be(1);
            response.PageSize.Should().Be(10);
        }

        [Fact]
        public async Task Handle_Should_Return_NoContent_When_No_Customers_Found()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((new List<Customer>(), 0));

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(204);
            response.Data.Should().BeEmpty();
        }

        [Fact]
        public async Task Handle_Should_Return_InternalError_When_Exception_Occurs()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<Customer, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ThrowsAsync(new Exception("Erro interno"));

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
        }
    }
}