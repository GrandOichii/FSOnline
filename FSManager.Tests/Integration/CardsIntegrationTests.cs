using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit.Abstractions;

namespace FSManager.Tests.Integration;

public class CardsIntegrationTests
    : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly ITestOutputHelper _output;
    private readonly WebApplicationFactory<Program> _factory;
    private readonly PostgreSqlContainer _dbContainer;

    public async Task InitializeAsync() {
        await _dbContainer.StartAsync();
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<CardRepository>();
        await dbContext.Database.MigrateAsync();

        // TODO bad
        dbContext.Database.ExecuteSqlRaw(@"
CREATE OR REPLACE PROCEDURE createCard(
    key VARCHAR,
    name VARCHAR,
    type VARCHAR,
    health int,
    attack int,
    evasion int,
    text VARCHAR,
    script VARCHAR,
    soul_value int,
    rewards_text VARCHAR,
    collectionKey VARCHAR,
    image_url VARCHAR
)
LANGUAGE plpgsql
AS $$
BEGIN
    IF NOT EXISTS(SELECT * FROM ""CardCollections"" WHERE ""Key"" = collectionKey)
    THEN
        INSERT INTO ""CardCollections""(""Key"") VALUES (collectionKey);
	END IF;

    -- create card
	INSERT INTO ""Cards""(
		""Key"", 
		""Name"", 
		""Type"",
		""Health"",
		""Attack"",
		""Evasion"",
		""Text"", 
		""Script"", 
		""SoulValue"",
        ""RewardsText"",
		""CollectionKey"",
        ""ImageUrl""
    ) VALUES (
		key, 
		name, 
		type, 
		health, 
		attack, 
		evasion, 
		text, 
		script, 
		soul_value, 
        rewards_text,
		collectionKey,
        image_url
	);
END; $$;");
    }

    public async Task DisposeAsync() {

        await _dbContainer.StopAsync();
    }

    public CardsIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _dbContainer = new PostgreSqlBuilder()
            .WithCleanUp(true)
            .Build();

        _factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services => {
                services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<CardRepository>)));
                services.AddDbContext<CardRepository>(o => 
                    o.UseNpgsql(_dbContainer.GetConnectionString()));
            });
        });
    }

    // move to Shared
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

    private async Task<PostCard> CreateCard(
        HttpClient client,
        string key = "card-key",
        string name = "Card Name",
        string type = "Item",
        int health = -1,
        int attack = -1,
        int evasion = -1,
        string text = "card text here",
        string rewardsText = "rewards here",
        int soulValue = 0,

        string script = "print('card script missing')",
        string collectionKey = "testCollection",
        string imageUrl = "http://test.image"

    ) {
        var card = new PostCard {
            Key = key,
            Name = name,
            Type = type,
            Health = health,
            Attack = attack,
            Evasion = evasion,
            Text = text,
            RewardsText = rewardsText,
            SoulValue = soulValue,
            Script = script,
            CollectionKey = collectionKey,
            ImageUrl = imageUrl,
        };
        var result = await client.PostAsJsonAsync("/api/v1/Cards", card);
        result.Should().BeSuccessful();

        return card;
    }

    [Fact]
    public async Task ShouldFetchAll() {
        // Arrange
        var client = _factory.CreateClient();


        // Act
        var result = await client.GetAsync("/api/v1/Cards");

        // Assert
        result.Should().BeSuccessful();
        var data = await result.Content.ReadFromJsonAsync<CardsPage>();
        data.Should().NotBeNull();
        data!.Cards.Should().BeEmpty();
    }

    [Fact]
    public async Task ShouldCreate() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.PostAsJsonAsync("/api/v1/Cards", await GetDummyPostCard());

        // Assert
        result.Should().BeSuccessful();
        result.Should().HaveStatusCode(HttpStatusCode.Created);
    }

    [Fact] // TODO change to Theory, add more cases
    public async Task ShouldNotCreate() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.PostAsJsonAsync("/api/v1/Cards", await GetDummyPostCard(""));

        // Assert
        result.Should().HaveClientError();
    }

    [Fact]
    public async Task ShouldFetchByKey() {
        // Arrange
        var client = _factory.CreateClient();
        var cardKey = "card-key";
        var created = await CreateCard(client, key: cardKey);

        // Act
        var result = await client.GetAsync($"/api/v1/Cards/{cardKey}");

        // Assert
        result.Should().BeSuccessful();
        var data = await result.Content.ReadFromJsonAsync<GetCard>();
        data.Should().NotBeNull();
        data!.Should().BeEquivalentTo(
            created, 
            o => o.WithMapping("CollectionKey", "Collection")
        );
    }

    [Fact]
    public async Task ShouldNotFetchByKey() {
        // Arrange
        var client = _factory.CreateClient();
        var cardKey = "card-key";

        // Act
        var result = await client.GetAsync($"/api/v1/Cards/{cardKey}");

        // Assert
        result.Should().HaveStatusCode(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task ShouldDelete() {
        // Arrange
        var client = _factory.CreateClient();
        var cardKey = "card-key";
        await CreateCard(client, cardKey);

        // Act
        var result = await client.DeleteAsync($"/api/v1/Cards/{cardKey}");

        // Assert
        result.Should().BeSuccessful();
    }

    [Fact]
    public async Task ShouldNotDelete() {
        // Arrange
        var client = _factory.CreateClient();
        var cardKey = "card-key";

        // Act
        var result = await client.DeleteAsync($"/api/v1/Cards/{cardKey}");

        // Assert
        result.Should().HaveClientError();
    }

}