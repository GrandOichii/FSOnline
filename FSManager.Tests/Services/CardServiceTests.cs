

using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using FSManager.Mapping;
using FSManager.Models;
using FSManager.Repositories;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Query;

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

    // [Fact]
    // public async Task ShouldCreate() {
    //     // Arrange
    //     A.CallTo(() => _cardRepo.CreateCard(
    //         A<string>._,
    //         A<string>._,
    //         A<string>._,
    //         A<string>._,
    //         A<string>._,
    //         A<string>._,
    //         A<string>._
    //     ))
    // }
}