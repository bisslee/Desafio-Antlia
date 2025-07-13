using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosifByKey;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetProductCosifByKeyTests : BaseTest
    {
        private readonly Mock<ILogger<GetProductCosifByKeyHandler>> LoggerMock;
        private readonly Mock<IReadRepository<ProductCosif>> ReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetProductCosifByKeyHandler Handler;

        public GetProductCosifByKeyTests()
        {
            LoggerMock = new Mock<ILogger<GetProductCosifByKeyHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<ProductCosif>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetProductCosifByKeyHandler(
                ReadRepositoryMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetProductCosifByKeyRequest CreateValidRequest()
        {
            return new GetProductCosifByKeyRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Return_ProductCosif_Successfully()
        {
            var request = CreateValidRequest();
            var productCosif = new ProductCosif 
            { 
                Id = request.Id, 
                ProductCode = "P001", 
                CosifCode = "C001", 
                ClassificationCode = "CLASS001" 
            };

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(productCosif);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(request.Id);
        }

        [Fact]
        public async Task Handle_Should_Return_Error_When_ProductCosif_Not_Found()
        {
            var request = CreateValidRequest();

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((ProductCosif?)null);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Product cosif not found");
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