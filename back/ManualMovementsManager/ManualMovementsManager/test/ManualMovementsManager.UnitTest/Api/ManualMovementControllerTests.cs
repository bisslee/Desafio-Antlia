using FluentAssertions;
using MediatR;
using ManualMovementsManager.Api.Controllers;
using ManualMovementsManager.Application.Commands.ManualMovements.AddManualMovement;
using ManualMovementsManager.Application.Commands.ManualMovements.ChangeManualMovement;
using ManualMovementsManager.Application.Commands.ManualMovements.RemoveManualMovement;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovement;
using ManualMovementsManager.Application.Queries.ManualMovements.GetManualMovementByKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Api.Controllers
{
    public class ManualMovementControllerTests : BaseTest
    {
        private readonly Mock<IMediator> MediatorMock;
        private readonly Mock<ILogger<ManualMovementController>> LoggerMock;
        private readonly ManualMovementController Controller;

        public ManualMovementControllerTests()
        {
            MediatorMock = new Mock<IMediator>();
            LoggerMock = new Mock<ILogger<ManualMovementController>>();
            Controller = new ManualMovementController(
                MediatorMock.Object,
                LoggerMock.Object);
        }

        [Fact]
        public async Task Get_Should_Return_206_When_ManualMovements_Found()
        {
            var request = new GetManualMovementRequest();
            var response = new GetManualMovementResponse
            {
                Success = true,
                StatusCode = 206,
                Data = new List<ManualMovementsManager.Domain.Entities.ManualMovement> { new() }
            };

            MediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.Get(request);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(206);
        }

        [Fact]
        public async Task GetByKey_Should_Return_200_When_ManualMovement_Found()
        {
            var id = Guid.NewGuid();
            var response = new GetManualMovementByKeyResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovementsManager.Domain.Entities.ManualMovement { Id = id }
            };

            MediatorMock.Setup(m => m.Send(It.Is<GetManualMovementByKeyRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.GetByKey(id);

            result.Should().BeOfType<OkObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Add_Should_Return_201_When_ManualMovement_Created()
        {
            var request = new AddManualMovementRequest();
            var response = new AddManualMovementResponse
            {
                Success = true,
                StatusCode = 201,
                Data = new ManualMovementsManager.Domain.Entities.ManualMovement()
            };

            MediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await Controller.Add(request);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Change_Should_Return_200_When_ManualMovement_Updated()
        {
            var id = Guid.NewGuid();
            var request = new ChangeManualMovementRequest();
            var response = new ChangeManualMovementResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovementsManager.Domain.Entities.ManualMovement()
            };

            MediatorMock.Setup(m => m.Send(It.Is<ChangeManualMovementRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.Change(id, request);

            result.Should().BeOfType<OkObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(200);
        }

        [Fact]
        public async Task Remove_Should_Return_200_When_ManualMovement_Removed()
        {
            var id = Guid.NewGuid();
            var response = new RemoveManualMovementResponse
            {
                Success = true,
                StatusCode = 200,
                Data = true
            };

            MediatorMock.Setup(m => m.Send(It.Is<RemoveManualMovementRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            Controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext()
            };

            var result = await Controller.Remove(id);

            result.Should().BeOfType<OkObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(200);
        }
    }
} 