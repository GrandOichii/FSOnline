using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Tests.Controllers;

public class CardsApiControllerTests {
    private readonly ICardService _cardService;
    private readonly CardsApiController _controller;

    public CardsApiControllerTests() {
        _cardService = A.Fake<ICardService>();

        _controller = new(_cardService);
    }

    [Fact]
    public async Task ShouldFetchAll() {
        // Arrange
        var call = A.CallTo(() => _cardService.All(null)).WithAnyArguments();
        call.Returns([ A.Fake<GetCard>() ]);

        // Act
        var result = await _controller.All();

        // Arrange
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldFetchByKey() {
        // Arrange
        var call = A.CallTo(() => _cardService.ByKey(A<string>._)).WithAnyArguments();
        call.Returns(A.Fake<GetCard>());

        // Act
        var result = await _controller.ByKey("card-key");

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldNotFetchByKey() {
        // Arrange
        var call = A.CallTo(() => _cardService.ByKey(A<string>._)).WithAnyArguments();
        call.Throws<CardNotFoundException>();

        // Act
        var result = await _controller.ByKey("card-key");

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ShouldDelete() {
        // Arrange
        var call = A.CallTo(() => _cardService.Delete(A<string>._)).WithAnyArguments();
        call.DoesNothing();

        // Act
        var result = await _controller.Delete("card-key");

        // Assert
        result.Should().BeOfType<OkResult>();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotDelete_NotFound() {
        // Arrange
        var call = A.CallTo(() => _cardService.Delete(A<string>._)).WithAnyArguments();
        call.Throws<CardNotFoundException>();

        // Act
        var result = await _controller.Delete("card-key");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotDelete_FailedToDelete() {
        // Arrange
        var call = A.CallTo(() => _cardService.Delete(A<string>._)).WithAnyArguments();
        call.Throws<FailedToDeleteCardException>();

        // Act
        var result = await _controller.Delete("card-key");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        call.MustHaveHappenedOnceExactly();
    }
}