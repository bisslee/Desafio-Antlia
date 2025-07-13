using FluentAssertions;
using MediatR;
using ManualMovementsManager.Api.Controllers;
using ManualMovementsManager.Application.Commands.Products.AddProduct;
using ManualMovementsManager.Application.Commands.Products.ChangeProduct;
using ManualMovementsManager.Application.Commands.Products.RemoveProduct;
using ManualMovementsManager.Application.Queries.Products.GetProduct;
using ManualMovementsManager.Application.Queries.Products.GetProductByKey;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace ManualMovementsManager.UnitTest.Api.Controllers
{
    public class ProductControllerTests : BaseTest
    {
        private readonly Mock<IMediator> MediatorMock;
        private readonly Mock<ILogger<ProductController>> LoggerMock;
        private readonly ProductController Controller;

        public ProductControllerTests()
        {
            MediatorMock = new Mock<IMediator>();
            LoggerMock = new Mock<ILogger<ProductController>>();
            Controller = new ProductController(
                MediatorMock.Object,
                LoggerMock.Object);
        }

        [Fact]
        public async Task Get_Should_Return_206_When_Products_Found()
        {
            var request = new GetProductRequest();
            var response = new GetProductResponse
            {
                Success = true,
                StatusCode = 206,
                Data = new List<ManualMovementsManager.Domain.Entities.Product> { new() }
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
        public async Task GetByKey_Should_Return_200_When_Product_Found()
        {
            var id = Guid.NewGuid();
            var response = new GetProductByKeyResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovementsManager.Domain.Entities.Product { Id = id }
            };

            MediatorMock.Setup(m => m.Send(It.Is<GetProductByKeyRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
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
        public async Task Add_Should_Return_201_When_Product_Created()
        {
            var request = new AddProductRequest();
            var response = new AddProductResponse
            {
                Success = true,
                StatusCode = 201,
                Data = new ManualMovementsManager.Domain.Entities.Product()
            };

            MediatorMock.Setup(m => m.Send(request, It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);

            var result = await Controller.Add(request);

            result.Should().BeOfType<ObjectResult>();
            (result as ObjectResult)!.StatusCode.Should().Be(201);
        }

        [Fact]
        public async Task Change_Should_Return_200_When_Product_Updated()
        {
            var id = Guid.NewGuid();
            var request = new ChangeProductRequest();
            var response = new ChangeProductResponse
            {
                Success = true,
                StatusCode = 200,
                Data = new ManualMovementsManager.Domain.Entities.Product()
            };

            MediatorMock.Setup(m => m.Send(It.Is<ChangeProductRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
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
        public async Task Remove_Should_Return_200_When_Product_Removed()
        {
            var id = Guid.NewGuid();
            var response = new RemoveProductResponse
            {
                Success = true,
                StatusCode = 200,
                Data = true
            };

            MediatorMock.Setup(m => m.Send(It.Is<RemoveProductRequest>(x => x.Id == id), It.IsAny<CancellationToken>()))
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