using FluentAssertions;
using FluentValidation;
using ManualMovementsManager.Application.Commands.ManualMovements.RemoveManualMovement;
using ManualMovementsManager.Application.Helpers;
using ManualMovementsManager.Domain.Entities;
using ManualMovementsManager.Domain.Exceptions;
using ManualMovementsManager.Domain.Repositories;
using ManualMovementsManager.Domain.Resources;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Application.Commands
{
    public class RemoveManualMovementTests : BaseTest
    {
        private readonly Mock<ILogger<RemoveManualMovementHandler>> LoggerMock;
        private readonly Mock<IWriteRepository<ManualMovement>> WriteRepositoryMock;
        private readonly Mock<IManualMovementReadRepository> ManualMovementReadRepositoryMock;
        private readonly IResponseBuilder ResponseBuilder;
        
        private readonly RemoveManualMovementHandler Handler;

        public RemoveManualMovementTests()
        {
            LoggerMock = new Mock<ILogger<RemoveManualMovementHandler>>();
            WriteRepositoryMock = new Mock<IWriteRepository<ManualMovement>>();
            ManualMovementReadRepositoryMock = new Mock<IManualMovementReadRepository>();
            ResponseBuilder = new ResponseBuilder();

            Handler = new RemoveManualMovementHandler(
                LoggerMock.Object,
                WriteRepositoryMock.Object,
                ManualMovementReadRepositoryMock.Object,
                ResponseBuilder
            );
        }

        private RemoveManualMovementRequest CreateValidRequest()
        {
            return new RemoveManualMovementRequest
            {
                Id = Guid.NewGuid()
            };
        }

        [Fact]
        public async Task Handle_Should_Remove_ManualMovement_Successfully()
        {
            var request = CreateValidRequest();
            var manualMovement = new ManualMovement { Id = request.Id, Description = "Test Movement" };

            ManualMovementReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync(manualMovement);

            WriteRepositoryMock.Setup(r => r.Delete(manualMovement))
            .ReturnsAsync(true);

            var response = await Handler.Handle(request, CancellationToken.None);

            response.Success.Should().BeTrue();
            response.StatusCode.Should().Be(200);
            response.Data.Should().BeTrue();
        }

        [Fact]
        public async Task Handle_Should_Throw_ManualMovementNotFoundException_When_ManualMovement_Not_Found()
        {
            var request = CreateValidRequest();

            ManualMovementReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ReturnsAsync((ManualMovement?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ManualMovementNotFoundException>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Contain("was not found");
            exception.ErrorCode.Should().Be("MANUAL_MOVEMENT_NOT_FOUND");
            exception.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task Handle_Should_Throw_Exception_When_Internal_Error_Occurs()
        {
            var request = CreateValidRequest();

            ManualMovementReadRepositoryMock.Setup(r => r.GetByIdAsync(request.Id))
                .ThrowsAsync(new Exception("Erro interno"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(
                () => Handler.Handle(request, CancellationToken.None));

            exception.Message.Should().Be("Erro interno");
        }
    }
} 