using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetProductCosifTests : BaseTest
    {
        private readonly Mock<ILogger<GetProductCosifHandler>> LoggerMock;
        private readonly Mock<IReadRepository<ProductCosif>> ReadRepositoryMock;
        private readonly Mock<IValidator<GetProductCosifRequest>> ValidatorMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetProductCosifHandler Handler;

        public GetProductCosifTests()
        {
            LoggerMock = new Mock<ILogger<GetProductCosifHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<ProductCosif>>();
            ValidatorMock = new Mock<IValidator<GetProductCosifRequest>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetProductCosifHandler(
                ReadRepositoryMock.Object,
                ValidatorMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetProductCosifRequest CreateValidRequest()
        {
            return new GetProductCosifRequest
            {
                Page = 1,
                Offset = 10,
                ProductCode = "P001",
                CosifCode = "C001"
            };
        }

        [Fact]
        public async Task Handle_Should_Return_ProductCosifs_Successfully()
        {
            var request = CreateValidRequest();
            var productCosifs = new List<ProductCosif>
            {
                new ProductCosif { ProductCode = "P001", CosifCode = "C001", ClassificationCode = "CLASS001" }
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<ProductCosif, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((productCosifs, productCosifs.Count));

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
        public async Task Handle_Should_Return_NoContent_When_No_ProductCosifs_Found()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<ProductCosif, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((new List<ProductCosif>(), 0));

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
                It.IsAny<System.Linq.Expressions.Expression<Func<ProductCosif, bool>>>(),
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