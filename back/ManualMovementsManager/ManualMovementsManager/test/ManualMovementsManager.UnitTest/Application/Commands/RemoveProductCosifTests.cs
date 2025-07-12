using FluentAssertions;
using FluentValidation;
using ManualMovementsManager.Application.Commands.ProductCosifs.RemoveProductCosif;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Resources;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class RemoveProductCosifTests : BaseTest
    {
        private readonly Mock<ILogger<RemoveProductCosifHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<ProductCosif>> WriteRepositoryMock;
        private readonly Mock<IProductCosifReadRepository> ProductCosifReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;
        
        private readonly RemoveProductCosifHandler Handler;

        public RemoveProductCosifTests()
        {
            LoggerMock = new Mock<ILogger<RemoveProductCosifHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<ProductCosif>>();
            ProductCosifReadRepositoryMock = new Mock<IProductCosifReadRepository>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new RemoveProductCosifHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                ProductCosifReadRepositoryMock.Object,
                ResponseBuilder
            );
        }

        private RemoveProductCosifRequest CreateValidRequest()
        {
            return new RemoveProductCosifRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Remove_ProductCosif_Successfully()
        {
            var request = CreateValidRequest();
            var productCosif = new ProductCosif { Id = request.Id, CosifCode = "COSIF001" };

            ProductCosifReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(productCosif);

            WriteRepositoryMock.Setup(r => r.Delete(productCosif))
            .ReturnsAsync(true);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Throw_ProductCosifNotFoundException_When_ProductCosif_Not_Found()
        {
            var request = CreateValidRequest();

            ProductCosifReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((ProductCosif?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ProductCosifNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("PRODUCT_COSIF_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            var request = CreateValidRequest();

            ProductCosifReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
} 