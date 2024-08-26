using FakeItEasy;
using FluentAssertions;
using FSManager.Controllers;
using FSManager.Dto.Cards;
using Microsoft.AspNetCore.Mvc;

namespace FSManager.Tests.Controllers;

public class CardsControllerTests {
    private readonly ICardService _cardService;
    private readonly CardsController _controller;

    public CardsControllerTests() {
        _cardService = A.Fake<ICardService>();

        _controller = new(_cardService);
    }

    [Fact]
    public async Task ShouldGetAll() {
        // Arrange
        var call = A.CallTo(() => _cardService.All(null)).WithAnyArguments();
        call.Returns([ A.Fake<GetCard>() ]);

        // Act
        var result = await _controller.All() as ViewResult;
        var model = (List<GetCard>)result!.ViewData.Model!;

        // Assert
        model.Should().HaveCount(1);
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldFetchByKey() {
        // Arrange
        var call = A.CallTo(() => _cardService.ByKey(A<string>._)).WithAnyArguments();
        call.Returns(A.Fake<GetCard>());

        // Act
        var result = await _controller.ByKey("card-key") as ViewResult;
        var model = (GetCard?) result!.ViewData.Model;

        // Assert
        model.Should().NotBeNull();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldDelete() {
        // Arrange
        var call = A.CallTo(() => _cardService.Delete(A<string>._)).WithAnyArguments();
        call.DoesNothing();

        // Act
        var result = await _controller.Delete("card-key") as ViewResult;

        // Assert
        // TODO
        call.MustHaveHappenedOnceExactly();
    }

    // TODO? test CreateForm

    [Fact]
    public async Task ShouldCreate() {
        // Arrange
        var call = A.CallTo(() => _cardService.Create(A<PostCard>._)).WithAnyArguments();
        call.Returns(A.Fake<GetCard>());

        // Act
        var result = await _controller.Create(A.Fake<PostCard>());

        // Assert
        call.MustHaveHappenedOnceExactly();
    }
}