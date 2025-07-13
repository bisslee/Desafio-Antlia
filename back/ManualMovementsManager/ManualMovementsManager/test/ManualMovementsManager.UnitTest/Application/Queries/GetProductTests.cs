using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.Products.GetProduct;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetProductTests : BaseTest
    {
        private readonly Mock<ILogger<GetProductHandler>> LoggerMock;
        private readonly Mock<IReadRepository<Product>> ReadRepositoryMock;
        private readonly Mock<IValidator<GetProductRequest>> ValidatorMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetProductHandler Handler;

        public GetProductTests()
        {
            LoggerMock = new Mock<ILogger<GetProductHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<Product>>();
            ValidatorMock = new Mock<IValidator<GetProductRequest>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetProductHandler(
                ReadRepositoryMock.Object,
                ValidatorMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetProductRequest CreateValidRequest()
        {
            return new GetProductRequest
            {
                Page = 1,
                Offset = 10,
                ProductCode = "P001"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Products_Successfully()
        {
            var request = CreateValidRequest();
            var products = new List<Product>
            {
                new Product { ProductCode = "P001", Description = "Produto Teste" }
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((products, products.Count));

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
        public async Task Handle_Should_Return_NoContent_When_No_Products_Found()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((new List<Product>(), 0));

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
                It.IsAny<System.Linq.Expressions.Expression<Func<Product, bool>>>(),
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