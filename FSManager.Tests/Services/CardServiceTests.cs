using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using FSManager.Dto.Cards;
using FSManager.Mapping;
using FSManager.Shared.Models;
using FSManager.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;

namespace FSManager.Tests.Services;

public class CardServiceTests {
    private readonly ICardRepository _cardRepo;

    private readonly CardService _cardService;

    public CardServiceTests() {
        var mapper = new Mapper(
            new MapperConfiguration(cfg => {
                cfg.AddProfile(new AutoMapperProfile());
            })
        );

        _cardRepo = A.Fake<ICardRepository>();
        A.CallTo(() => _cardRepo.GetDefaultCardImageCollectionKey()).Returns("Default");

        _cardService = new CardService(_cardRepo, mapper);
    }

    private async Task<CardModel> GetDummyCardModel() {
        var result = new CardModel() {
            Key = "card-key",
            Name = "Card",
            Type = "Type1",
            Health = 2,
            Evasion = 3,
            Attack = 1,
            Text = "(no text)",
            Script = "print('Missing script')",
            SoulValue = 1,
            Collection = new() { Key = "test", Cards = [] },
            Images = [ ]
        };

        result.Images.Add(new() {ID = 1, Source = "image-source", Card = result, Collection = new() {Key = await _cardRepo.GetDefaultCardImageCollectionKey(), Images = []} });
        return result;
    } 

    [Fact]
    public async Task ShouldFetchAll() {
        // Arrange
        A.CallTo(() => _cardRepo.AllCards()).Returns([ A.Fake<CardModel>() ]);

        // Act
        var result = await _cardService.All();

        // Assert
        result.Should().NotBeEmpty();
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
            key,
            "Name",
            "Type1",
            2,
            1,
            3,
            "(no text)",
            "print('missing script')",
            1,
            "test",
            "image-source"
        )).WithAnyArguments();
        A.CallTo(() => _cardRepo.ByKey(A<string>._)).Returns( await GetDummyCardModel() );

        // Act
        var result = await _cardService.Create(A.Fake<PostCard>());

        // Assert
        result.Should().NotBeNull();
        result.Key.Should().Be(key);
        call.MustHaveHappenedOnceExactly();
    }

    [Fact]
    public async Task ShouldFetchKeys() {
        // Arrange
        var card = await GetDummyCardModel();
        var call = A.CallTo(() => _cardRepo.GetCards());
        call.Returns([ card ]);

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
        call.Returns([ card ]);

        // Act
        var result = await _cardService.FromCollection(card.Collection.Key);

        // Assert
        result.Should().HaveCount(1);
        result.ElementAt(0).Name.Should().Be(card.Name);
    }
}