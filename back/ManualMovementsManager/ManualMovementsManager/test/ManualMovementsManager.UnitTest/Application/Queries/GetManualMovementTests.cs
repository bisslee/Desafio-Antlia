using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetManualMovementTests : BaseTest
    {
        private readonly Mock<ILogger<GetManualMovementHandler>> LoggerMock;
        private readonly Mock<IReadRepository<ManualMovement>> ReadRepositoryMock;
        private readonly Mock<IValidator<GetManualMovementRequest>> ValidatorMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetManualMovementHandler Handler;

        public GetManualMovementTests()
        {
            LoggerMock = new Mock<ILogger<GetManualMovementHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<ManualMovement>>();
            ValidatorMock = new Mock<IValidator<GetManualMovementRequest>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetManualMovementHandler(
                ReadRepositoryMock.Object,
                ValidatorMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetManualMovementRequest CreateValidRequest()
        {
            return new GetManualMovementRequest
            {
                Page = 1,
                Offset = 10,
                Description = "Movimento Teste",
                StartDate = DateTime.Now.AddDays(-30),
                EndDate = DateTime.Now
            };
        }

        [Fact]
        public async Task Handle_Should_Return_ManualMovements_Successfully()
        {
            var request = CreateValidRequest();
            var manualMovements = new List<ManualMovement>
            {
                new ManualMovement 
                { 
                    Description = "Movimento Teste", 
                    MovementDate = DateTime.Now, 
                    Value = 100.50m 
                }
            };

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<ManualMovement, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((manualMovements, manualMovements.Count));

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
        public async Task Handle_Should_Return_NoContent_When_No_ManualMovements_Found()
        {
            var request = CreateValidRequest();

            ValidatorMock.Setup(v => v.ValidateAsync(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ValidationResult());

            ReadRepositoryMock.Setup(r => r.FindWithPagination(
                It.IsAny<System.Linq.Expressions.Expression<Func<ManualMovement, bool>>>(),
                request.Page,
                request.Offset,
                It.IsAny<string>(),
                It.IsAny<string>()))
                .ReturnsAsync((new List<ManualMovement>(), 0));

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
                It.IsAny<System.Linq.Expressions.Expression<Func<ManualMovement, bool>>>(),
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