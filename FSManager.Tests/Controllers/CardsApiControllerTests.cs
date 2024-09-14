using FSManager.Dto.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;

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
        var call = A.CallTo(() => _cardService.All(A<int>._)).WithAnyArguments();
        call.Returns(A.Fake<CardsPage>());

        // Act
        var result = await _controller.All(0);

        // Arrange
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldFetchByKey() {
        // Arrange
        var call = A.CallTo(() => _cardService.ByKey(A<string>._)).WithAnyArguments();
        call.Returns(A.Fake<GetCardWithRelations>());

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

    [Fact]
    public async Task ShouldFetchFromCollection() {
        // Arrange
        var call = A.CallTo(() => _cardService.FromCollection(A<string>._, A<int>._)).WithAnyArguments();
        call.Returns(A.Fake<CardsPage>());

        // Act
        var result = await _controller.FromCollection("collection-key");

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldCreateRelation() {
        // Arrange
        var call = A.CallTo(() => _cardService.CreateRelation(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        // call.Returns([ A.Fake<GetCard>() ]);

        // Act
        var result = await _controller.CreateRelation(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        result.Should().BeOfType<CreatedResult>();
        call.MustHaveHappenedOnceExactly();
    }
    
    [Fact]
    public async Task ShouldNotCreateRelationTwice() {
        // Arrange
        var call = A.CallTo(() => _cardService.CreateRelation(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing().Once().Then.Throws<RelationAlreadyExistsException>();

        // Act
        await _controller.CreateRelation(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});
        var result = await _controller.CreateRelation(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedTwiceExactly();
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldNotCreateCardNotFound() {
        // Arrange
        var call = A.CallTo(() => _cardService.CreateRelation(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.Throws<CardNotFoundException>();

        // Act
        var result = await _controller.CreateRelation(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ShouldNotCreateRelationWithSelf() {
        // Arrange
        var call = A.CallTo(() => _cardService.CreateRelation(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.Throws<RelationWithSelfException>();

        // Act
        var result = await _controller.CreateRelation(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<BadRequestObjectResult>();

    }

    [Fact]
    public async Task ShouldDeleteRelation() {
        // Arrange
        var call = A.CallTo(() =>_cardService.DeleteRelation(A<string>._, A<string>._)).WithAnyArguments();
        call.DoesNothing();

        // Act
        var result = await _controller.DeleteRelation(new PostCardRelation() { CardKey = "card-key", RelatedCardKey = "related-card-key"} );

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ShouldNotDeleteNonexistantRelation() {
        // Arrange
        var call = A.CallTo(() =>_cardService.DeleteRelation(A<string>._, A<string>._)).WithAnyArguments();
        call.Throws<RelationNotFoundException>();

        // Act
        var result = await _controller.DeleteRelation(new PostCardRelation() { CardKey = "card-key", RelatedCardKey = "realted-card-key"} );

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ShouldNotDeleteRelationBetweenNonexistantCards() {
        // Arrange
        var call = A.CallTo(() =>_cardService.DeleteRelation(A<string>._, A<string>._)).WithAnyArguments();
        call.Throws<CardNotFoundException>();

        // Act
        var result = await _controller.DeleteRelation(new PostCardRelation() { CardKey = "card-key", RelatedCardKey = "realted-card-key"} );

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<NotFoundObjectResult>();
    }
    
    [Fact]
    public async Task ShouldNotDeleteRelationWithSelf() {
        // Arrange
        var call = A.CallTo(() =>_cardService.DeleteRelation(A<string>._, A<string>._)).WithAnyArguments();
        call.Throws<RelationWithSelfException>();

        // Act
        var result = await _controller.DeleteRelation(new PostCardRelation() { CardKey = "card-key", RelatedCardKey = "realted-card-key"} );

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldEditRelationType() {
        // Arrange
        var call = A.CallTo(() => _cardService.EditRelationType(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing();

        // Act
        var result = await _controller.EditRelationType(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task ShouldNotEditNonExistantRelation() {
        // Arrange
        var call = A.CallTo(() => _cardService.EditRelationType(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.Throws<RelationNotFoundException>();

        // Act
        var result = await _controller.EditRelationType(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ShouldNotEditRelationBetweenNonExistantCards() {
        // Arrange
        var call = A.CallTo(() => _cardService.EditRelationType(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.Throws<CardNotFoundException>();

        // Act
        var result = await _controller.EditRelationType(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task ShouldNotEditRelationBetweenSelf() {
        // Arrange
        var call = A.CallTo(() => _cardService.EditRelationType(A<string>._, A<string>._, A<CardRelationType>._)).WithAnyArguments();
        call.Throws<RelationWithSelfException>();

        // Act
        var result = await _controller.EditRelationType(new PostCardRelationWithType() { CardKey = "card-key", RelatedCardKey = "related-card-key", RelationType = CardRelationType.GENERAL});

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ShouldFetchCollections() {
        // Arrange
        var call = A.CallTo(() => _cardService.GetCollections());
        call.Returns(A.Fake<IEnumerable<GetCollection>>());

        // Act
        var result = await _controller.GetCollections();

        // Assert
        call.MustHaveHappenedOnceExactly();
        result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ShouldFetchFilteredCards() {
        // Arrange
        var call = A.CallTo(() => _cardService.Filter(A<CardFilter>._, A<int>._)).WithAnyArguments();
        call.Returns(A.Fake<CardsPage>());

        // Act
        var result = await _controller.Filter(new CardFilter(), 0);

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        call.MustHaveHappenedOnceExactly();
    }
}