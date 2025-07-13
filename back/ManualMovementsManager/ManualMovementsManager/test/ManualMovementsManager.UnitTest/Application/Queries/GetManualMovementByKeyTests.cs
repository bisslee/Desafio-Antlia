using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovementByKey;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Repositories;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Queries
{
    public class GetManualMovementByKeyTests : BaseTest
    {
        private readonly Mock<ILogger<GetManualMovementByKeyHandler>> LoggerMock;
        private readonly Mock<IReadRepository<ManualMovement>> ReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;

        private readonly GetManualMovementByKeyHandler Handler;

        public GetManualMovementByKeyTests()
        {
            LoggerMock = new Mock<ILogger<GetManualMovementByKeyHandler>>();
            ReadRepositoryMock = new Mock<IReadRepository<ManualMovement>>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new GetManualMovementByKeyHandler(
                ReadRepositoryMock.Object,
                LoggerMock.Object,
                ResponseBuilder
            );
        }

        private GetManualMovementByKeyRequest CreateValidRequest()
        {
            return new GetManualMovementByKeyRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Return_ManualMovement_Successfully()
        {
            var request = CreateValidRequest();
            var manualMovement = new ManualMovement 
            { 
                Id = request.Id, 
                Description = "Movimento Teste", 
                MovementDate = DateTime.Now, 
                Value = 100.50m 
            };

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(manualMovement);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().NotBeNull();
            response.Data.Id.Should().Be(request.Id);
        }

        [Fact]
        public async Task Handle_Should_Return_Error_When_ManualMovement_Not_Found()
        {
            var request = CreateValidRequest();

            ReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((ManualMovement?)null);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeFalse();
            response.StatusCode.Should().Be(500);
            response.Data.Should().BeNull();
            response.Message.Should().Be("Manual movement not found");
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