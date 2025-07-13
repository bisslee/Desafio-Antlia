using FluentAssertions;
using FluentValidation;
using ManualMovementsManager.Application.Commands.Products.RemoveProduct;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Resources;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class RemoveProductTests : BaseTest
    {
        private readonly Mock<ILogger<RemoveProductHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<Product>> WriteRepositoryMock;
        private readonly Mock<IProductReadRepository> ProductReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;
        
        private readonly RemoveProductHandler Handler;

        public RemoveProductTests()
        {
            LoggerMock = new Mock<ILogger<RemoveProductHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<Product>>();
            ProductReadRepositoryMock = new Mock<IProductReadRepository>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new RemoveProductHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                ProductReadRepositoryMock.Object,
                ResponseBuilder
            );
        }

        private RemoveProductRequest CreateValidRequest()
        {
            return new RemoveProductRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Remove_Product_Successfully()
        {
            var request = CreateValidRequest();
            var product = new Product { Id = request.Id, ProductCode = "PROD001" };

            ProductReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(product);

            WriteRepositoryMock.Setup(r => r.Delete(product))
            .ReturnsAsync(true);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Throw_ProductNotFoundException_When_Product_Not_Found()
        {
            var request = CreateValidRequest();

            // Configurar o mock para retornar null quando GetByIdAsync for chamado
            ProductReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((Product?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("PRODUCT_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            var request = CreateValidRequest();

            ProductReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
} 