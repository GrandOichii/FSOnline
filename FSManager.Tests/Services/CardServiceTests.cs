using System.Security;
using FSManager.Settings;
using Microsoft.Extensions.Options;

namespace FSManager.Tests.Services;

public class CardServiceTests {
    private readonly ICardRepository _cardRepo;
    private readonly ICollectionRepository _collectionRepo;

    private readonly CardService _cardService;

    public CardServiceTests() {
        var mapper = new Mapper(
            new MapperConfiguration(cfg => {
                cfg.AddProfile(new AutoMapperProfile());
            })
        );

        _cardRepo = A.Fake<ICardRepository>();
        _collectionRepo = A.Fake<ICollectionRepository>();

        _cardService = new CardService(_cardRepo, _collectionRepo, mapper, Options.Create(new CardSettings() {
            CardsPerPage = 10
        }));
    }

    private async Task<PostCard> GetDummyPostCard(string cardKey = "card1") {
        return new PostCard {
            Key = cardKey,
            Name = "Card",
            Type = "Monster",
            Health = 2,
            Evasion = 3,
            Attack = 1,
            Text = "(no text)",
            Script = "print('Missing script')",
            SoulValue = 1,
            CollectionKey = "test",
            RewardsText = "",
            ImageUrl = "http://imageurl.example",
        };
    }

    private async Task<CardModel> GetDummyCardModel(string cardKey = "card-key") {
        return new CardModel() {
            Key = cardKey,
            Name = "Card",
            Type = "Type1",
            Health = 2,
            Evasion = 3,
            Attack = 1,
            Text = "(no text)",
            Script = "print('Missing script')",
            SoulValue = 1,
            Collection = new() { Key = "test", Cards = [] },
            RewardsText = "",
            ImageUrl = "image-url",
            RelatedTo = [],
            Relations = []
        };
    } 

    [Fact]
    public async Task ShouldFetchByKey() {
        // Arrange
        A.CallTo(() => _cardRepo.ByKey(A<string>._)).Returns(await GetDummyCardModel());

        // Act
        var result = await _cardService.ByKey("card-key");

        // Assert
        result.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldNotFetchByKey() {
        // Arrange
        A.CallTo(() => _cardRepo.ByKey(A<string>._)).Returns<CardModel?>(null);

        // Act
        var act = () => _cardService.ByKey("card-key");

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
    }

    [Fact]
    public async Task ShouldDelete() {
        // Arrange
        A.CallTo(() => _cardRepo.RemoveCard(A<string>._)).Returns(true);

        // Act
        var act = () => _cardService.Delete("card-key");

        // Assert
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ShouldNotDelete() {
        // Arrange
        A.CallTo(() => _cardRepo.RemoveCard(A<string>._)).Returns(false);

        // Act
        var act = () => _cardService.Delete("card-key");

        // Assert
        await act.Should().ThrowAsync<FailedToDeleteCardException>();
    }

    [Fact]
    public async Task ShouldCreate() {
        // Arrange
        var key = "card-key";
        var call = A.CallTo(() => _cardRepo.CreateCard(
            A<string>._,
            A<string>._,
            A<string>._,
            A<int>._,
            A<int>._,
            A<int>._,
            A<string>._,
            A<string>._,
            A<int>._,
            A<string>._,
            A<string>._,
            A<string>._
        )).WithAnyArguments();
        A.CallTo(() => _cardRepo.ByKey(A<string>._)).Returns( await GetDummyCardModel() );

        // Act
        var result = await _cardService.Create(await GetDummyPostCard());

        // Assert
        result.Should().NotBeNull();
        result.Key.Should().Be(key);
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotCreate() {
        // Assert
        var call = A.CallTo(() => _cardRepo.CreateCard(
            A<string>._,
            A<string>._,
            A<string>._,
            A<int>._,
            A<int>._,
            A<int>._,
            A<string>._,
            A<string>._,
            A<int>._,
            A<string>._,
            A<string>._,
            A<string>._
        )).WithAnyArguments();    
        var pc = await GetDummyPostCard("");

        // Act
        var act = () => _cardService.Create(pc);

        // Assert
        call.MustNotHaveHappened();
        await act.Should().ThrowAsync<CardValidationException>();
    }

    [Fact]
    public async Task ShouldEdit() {
        // Assert
        var updateCall = A.CallTo(() => _cardRepo.UpdateCard(A<CardModel>._, A<CardModel>._)).WithAnyArguments();
        var fetchCall = A.CallTo(() => _cardRepo.ByKey(A<string>._)).WithAnyArguments();
        fetchCall.Returns(A.Fake<CardModel>());
        var pc = await GetDummyPostCard();

        // Act
        var act = () => _cardService.Edit("card-key", pc);

        // Assert
        await act.Should().NotThrowAsync();
        updateCall.MustHaveHappenedOnceExactly();
        fetchCall.MustHaveHappenedTwiceExactly();
    }

    [Fact]
    public async Task ShouldNotEdit_CardNotFound() {
        // Assert
        var updateCall = A.CallTo(() => _cardRepo.UpdateCard(A<CardModel>._, A<CardModel>._)).WithAnyArguments();
        var fetchCall = A.CallTo(() => _cardRepo.ByKey(A<string>._)).WithAnyArguments();
        fetchCall.Returns<CardModel?>(null);
        var pc = await GetDummyPostCard();

        // Act
        var act = () => _cardService.Edit("card-key", pc);

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        updateCall.MustNotHaveHappened();
        fetchCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotEdit_ValidationFailed() {
        // Assert
        var updateCall = A.CallTo(() => _cardRepo.UpdateCard(A<CardModel>._, A<CardModel>._)).WithAnyArguments();
        var fetchCall = A.CallTo(() => _cardRepo.ByKey(A<string>._)).WithAnyArguments();
        fetchCall.Returns(A.Fake<CardModel>());
        var pc = await GetDummyPostCard("");

        // Act
        var act = () => _cardService.Edit("card-key", pc);

        // Assert
        await act.Should().ThrowAsync<CardValidationException>();
        updateCall.MustNotHaveHappened();
        fetchCall.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldFetchKeys() {
        // Arrange
        var card = await GetDummyCardModel();
        var call = A.CallTo(() => _cardRepo.GetCards());
        call.Returns(
            new List<CardModel>() { card }
                .AsQueryable()
        );

        // Act
        var result = await _cardService.GetKeys();

        // Assert
        result.Should().HaveCount(1);
        result.ElementAt(0).Should().Be(card.Key);
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldFetchFromCollection() {
        // Arrange
        var card = await GetDummyCardModel();
        var call = A.CallTo(() => _cardRepo.GetCards());
        call.Returns(
            new List<CardModel>() { card }
                .AsQueryable()
        );

        // Act
        var result = await _cardService.FromCollection(card.Collection.Key, 0);

        // Assert
        result.Page.Should().Be(0);
        result.Cards.Should().HaveCount(1);
        result.Cards.ElementAt(0).Name.Should().Be(card.Name);
    }

    [Fact]
    public async Task ShouldCreateRelation() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.SaveRelation(A<CardRelation>._)).WithAnyArguments();
        var key1 = "card-key";
        var key2 = "related-card-key";
        A.CallTo(() => _cardRepo.ByKey(key1)).Returns(await GetDummyCardModel(cardKey: key1));
        A.CallTo(() => _cardRepo.ByKey(key2)).Returns(await GetDummyCardModel(cardKey: key2));
        call.DoesNothing();

        // Act
        var act = () => _cardService.CreateRelation(key1, key2, CardRelationType.GENERAL);

        // Assert
        await act.Should().NotThrowAsync();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotCreateRelationWithSelf() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.SaveRelation(A<CardRelation>._)).WithAnyArguments();
        var key = "card-key";
        A.CallTo(() => _cardRepo.ByKey(key)).Returns(await GetDummyCardModel(cardKey: key));
        call.DoesNothing();

        // Act
        var act = () => _cardService.CreateRelation(key, key, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowExactlyAsync<RelationWithSelfException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotCreateRelationWithNonexistantCard1() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.SaveRelation(A<CardRelation>._)).WithAnyArguments();
        var key1 = "card-key";
        var key2 = "related-card-key";
        A.CallTo(() => _cardRepo.ByKey(key1)).Returns(await GetDummyCardModel(cardKey: key1));
        A.CallTo(() => _cardRepo.ByKey(key2)).Returns<CardModel?>(null);
        call.DoesNothing();

        // Act
        var act = () => _cardService.CreateRelation(key1, key2, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowExactlyAsync<CardNotFoundException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotCreateRelationWithNonexistantCard2() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.SaveRelation(A<CardRelation>._)).WithAnyArguments();
        var key1 = "card-key";
        var key2 = "related-card-key";
        A.CallTo(() => _cardRepo.ByKey(key1)).Returns<CardModel?>(null);
        A.CallTo(() => _cardRepo.ByKey(key2)).Returns(await GetDummyCardModel(cardKey: key2));
        call.DoesNothing();

        // Act
        var act = () => _cardService.CreateRelation(key1, key2, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowExactlyAsync<CardNotFoundException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotCreateRelationWhichAlreadyExists1() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.SaveRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();
        var c1 = await GetDummyCardModel(cardKey: "card-key");
        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        c1.Relations.Add(new CardRelation() {
            RelatedTo = c1,
            RelatedCard = c2,
            RelationType = CardRelationType.GENERAL
        });
        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.CreateRelation(c1.Key, c2.Key, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowExactlyAsync<RelationAlreadyExistsException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotCreateRelationWhichAlreadyExists2() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.SaveRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();
        var c1 = await GetDummyCardModel(cardKey: "card-key");
        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        c1.RelatedTo.Add(new CardRelation() {
            RelatedTo = c2,
            RelatedCard = c1,
            RelationType = CardRelationType.GENERAL
        });
        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.CreateRelation(c1.Key, c2.Key, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowExactlyAsync<RelationAlreadyExistsException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldDeleteRelation() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.DeleteRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();

        var c1 = await GetDummyCardModel(cardKey: "card-key");
        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        c1.Relations.Add(new CardRelation() {
            RelatedTo = c1,
            RelatedCard = c2,
            RelationType = CardRelationType.GENERAL
        });
        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.DeleteRelation(c1.Key, c2.Key);

        // Assert
        await act.Should().NotThrowAsync();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotDeleteNonexistantRelation() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.DeleteRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();

        var c1 = await GetDummyCardModel(cardKey: "card-key");
        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.DeleteRelation(c1.Key, c2.Key);

        // Assert
        await act.Should().ThrowAsync<RelationNotFoundException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotDeleteRelationBetweenNonexistantCards1() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.DeleteRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();

        var c1 = await GetDummyCardModel(cardKey: "card-key");

        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey("related-card-key")).Returns<CardModel?>(null);

        // Act
        var act = () => _cardService.DeleteRelation(c1.Key, "related-card-key");

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        call.MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldNotDeleteRelationBetweenNonexistantCards2() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.DeleteRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();

        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        A.CallTo(() => _cardRepo.ByKey("card-key")).Returns<CardModel?>(null);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.DeleteRelation("card-key", c2.Key);

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotDeleteRelationWithSelf() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.DeleteRelation(A<CardRelation>._)).WithAnyArguments();
        call.DoesNothing();

        var c = await GetDummyCardModel(cardKey: "card-key");

        A.CallTo(() => _cardRepo.ByKey(c.Key)).Returns(c);

        // Act
        var act = () => _cardService.DeleteRelation(c.Key, c.Key);

        // Assert
        await act.Should().ThrowAsync<RelationWithSelfException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldEditRelation() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.UpdateRelationType(A<CardRelation>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing();

        var c1 = await GetDummyCardModel(cardKey: "card-key");
        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        c1.Relations.Add(new CardRelation() {
            RelatedTo = c1,
            RelatedCard = c2,
            RelationType = CardRelationType.GENERAL
        });
        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.EditRelationType(c1.Key, c2.Key, CardRelationType.GENERAL);

        // Assert
        await act.Should().NotThrowAsync();
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldNotEditNonexistantRelation() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.UpdateRelationType(A<CardRelation>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing();

        var c1 = await GetDummyCardModel(cardKey: "card-key");
        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.EditRelationType(c1.Key, c2.Key, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowAsync<RelationNotFoundException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotEditRelationBetweenNonexistantCards1() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.UpdateRelationType(A<CardRelation>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing();

        var c1 = await GetDummyCardModel(cardKey: "card-key");

        A.CallTo(() => _cardRepo.ByKey(c1.Key)).Returns(c1);
        A.CallTo(() => _cardRepo.ByKey("related-card-key")).Returns<CardModel?>(null);

        // Act
        var act = () => _cardService.EditRelationType(c1.Key, "related-card-key", CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        call.MustNotHaveHappened();
    }
    
    [Fact]
    public async Task ShouldNotEditRelationBetweenNonexistantCards2() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.UpdateRelationType(A<CardRelation>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing();

        var c2 = await GetDummyCardModel(cardKey: "related-card-key");

        A.CallTo(() => _cardRepo.ByKey("card-key")).Returns<CardModel?>(null);
        A.CallTo(() => _cardRepo.ByKey(c2.Key)).Returns(c2);

        // Act
        var act = () => _cardService.EditRelationType("card-key", c2.Key, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowAsync<CardNotFoundException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldNotEditRelationWithSelf() {
        // Arrange
        var call = A.CallTo(() => _cardRepo.UpdateRelationType(A<CardRelation>._, A<CardRelationType>._)).WithAnyArguments();
        call.DoesNothing();

        var c = await GetDummyCardModel(cardKey: "card-key");

        A.CallTo(() => _cardRepo.ByKey(c.Key)).Returns(c);

        // Act
        var act = () => _cardService.EditRelationType(c.Key, c.Key, CardRelationType.GENERAL);

        // Assert
        await act.Should().ThrowAsync<RelationWithSelfException>();
        call.MustNotHaveHappened();
    }

    [Fact]
    public async Task ShouldGetAllCollections() {
        // Arrange
        List<CardCollection> returns = [A.Fake<CardCollection>(), A.Fake<CardCollection>()];
        var call = A.CallTo(() => _collectionRepo.All());
        call.Returns(returns);        

        // Act
        var result = await _cardService.GetCollections();

        // Assert
        result.Should().HaveCount(returns.Count);
        call.MustHaveHappenedOnceExactly();
    }
}