using FluentAssertions;
using MediatR;
using ManualMovementsManager.Api.Controllers;
using ManualMovementsManager.Application.Commands.ProductCosifs.AddProductCosif;
using ManualMovementsManager.Application.Commands.ProductCosifs.ChangeProductCosif;
using ManualMovementsManager.Application.Commands.ProductCosifs.RemoveProductCosif;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosif;
using ManualMovementsManager.Application.Queries.ProductCosifs.GetProductCosifByKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ManualMovementsManager.Application.DTOs;

namespace ManualMovementsManager.UnitTest.Api.Controllers
{
    public class ProductCosifControllerTests : BaseTest
    {
        private readonly Mock<IMediator> MediatorMock;
        private readonly Mock<ILogger<ProductCosifController>> LoggerMock;
        private readonly ProductCosifController Controller;

        public ProductCosifControllerTests()
        {
            MediatorMock = new Mock<IMediator>();
            LoggerMock = new Mock<ILogger<ProductCosifController>>();
            Controller = new ProductCosifController(
                MediatorMock.Object,
                LoggerMock.Object);
        }

        [Fact]
        public async Task Get_Should_Return_206_When_ProductCosifs_Found()
        {
            var request = new GetProductCosifRequest();
            var response = new GetProductCosifResponse
            {
                Success = true,
                StatusCode = 206,
                Data = new List<ManualMovementsManager.Domain.Entities.ProductCosif> { new() }
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
        public async Task GetByKey_Should_Return_200_When_ProductCosif_Found()
        {
            var id = Guid.NewGuid();
            var response = new GetProductCosifByKeyResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovementsManager.Domain.Entities.ProductCosif { Id = id }
            };

            MediatorMock.Setup(m => m.Send(It.Is<GetProductCosifByKeyRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
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
        public async Task Add_Should_Return_201_When_ProductCosif_Created()
        {
            var request = new AddProductCosifRequest();
            var response = new AddProductCosifResponse
            {
                Success = true,
                StatusCode = 201,
                Data = new ProductCosifDto() // Corrigido para usar o DTO
            };

            MediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await Controller.Add(request);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Change_Should_Return_200_When_ProductCosif_Updated()
        {
            var id = Guid.NewGuid();
            var request = new ChangeProductCosifRequest();
            var response = new ChangeProductCosifResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovementsManager.Domain.Entities.ProductCosif()
            };

            MediatorMock.Setup(m => m.Send(It.Is<ChangeProductCosifRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
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
        public async Task Remove_Should_Return_200_When_ProductCosif_Removed()
        {
            var id = Guid.NewGuid();
            var response = new RemoveProductCosifResponse
            {
                Success = true,
                StatusCode = 200,
                Data = true
            };

            MediatorMock.Setup(m => m.Send(It.Is<RemoveProductCosifRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
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