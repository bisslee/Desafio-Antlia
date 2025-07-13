using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.Products.GetProductByKey;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetProductByKeyTests : BaseTest
    {
        private readonly Mock<ILogger<GetProductByKeyHandler>> LoggerMock;
        private readonly Mock<IReadRepository<Product>> ReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetProductByKeyHandler Handler;

        public GetProductByKeyTests()
        {
            LoggerMock = new Mock<ILogger<GetProductByKeyHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<Product>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetProductByKeyHandler(
                ReadRepositoryMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetProductByKeyRequest CreateValidRequest()
        {
            return new GetProductByKeyRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Return_Product_Successfully()
        {
            var request = CreateValidRequest();
            var product = new Product { Id = request.Id, ProductCode = "P001", Description = "Produto Teste" };

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(product);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(request.Id);
        }

        [Fact]
        public async Task Handle_Should_Return_Error_When_Product_Not_Found()
        {
            var request = CreateValidRequest();

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((Product?)null);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Product not found");
        }

        [Fact]
        public async Task Handle_Should_Return_InternalError_When_Exception_Occurs()
        {
            var request = CreateValidRequest();

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
        }
    }
} 