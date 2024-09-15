// using System.Net;
// using System.Net.Http.Json;
// using FSManager.Dto.Collections;
// using Microsoft.AspNetCore.Mvc.Testing;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.Extensions.DependencyInjection;
// using Testcontainers.PostgreSql;
// using Xunit.Abstractions;

// namespace FSManager.Tests.Integration;

// public class CardsIntegrationTests
//     : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
// {
//     private readonly ITestOutputHelper _output;
//     private readonly WebApplicationFactory<Program> _factory;
//     private readonly PostgreSqlContainer _dbContainer;

//     public async Task InitializeAsync() {
//         await _dbContainer.StartAsync();
//         using var scope = _factory.Services.CreateScope();
//         var dbContext = scope.ServiceProvider.GetRequiredService<CardRepository>();
//         await dbContext.Database.MigrateAsync();

//         dbContext.Relations.ExecuteDelete();
//         dbContext.Cards.ExecuteDelete();
//         dbContext.Collections.ExecuteDelete();

//         // TODO bad
//         dbContext.Database.ExecuteSqlRaw(@"
// CREATE OR REPLACE PROCEDURE createCard(
//     key VARCHAR,
//     name VARCHAR,
//     type VARCHAR,
//     health int,
//     attack int,
//     evasion int,
//     text VARCHAR,
//     script VARCHAR,
//     soul_value int,
//     rewards_text VARCHAR,
//     collectionKey VARCHAR,
//     image_url VARCHAR
// )
// LANGUAGE plpgsql
// AS $$
// BEGIN
//     IF NOT EXISTS(SELECT * FROM ""CardCollections"" WHERE ""Key"" = collectionKey)
//     THEN
//         INSERT INTO ""CardCollections""(""Key"") VALUES (collectionKey);
// 	END IF;

//     -- create card
// 	INSERT INTO ""Cards""(
// 		""Key"", 
// 		""Name"", 
// 		""Type"",
// 		""Health"",
// 		""Attack"",
// 		""Evasion"",
// 		""Text"", 
// 		""Script"", 
// 		""SoulValue"",
//         ""RewardsText"",
// 		""CollectionKey"",
//         ""ImageUrl""
//     ) VALUES (
// 		key, 
// 		name, 
// 		type, 
// 		health, 
// 		attack, 
// 		evasion, 
// 		text, 
// 		script, 
// 		soul_value, 
//         rewards_text,
// 		collectionKey,
//         image_url
// 	);
// END; $$;"
//         );
//     }

//     public async Task DisposeAsync() {

//         await _dbContainer.StopAsync();
//     }

//     public CardsIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
//     {
//         _output = output;
//         _dbContainer = new PostgreSqlBuilder()
//             .WithCleanUp(true)
//             .WithReuse(true)
//             .Build();

//         _factory = factory.WithWebHostBuilder(builder => {
//             builder.ConfigureServices(services => {
//                 services.Remove(services.First(s => s.ServiceType == typeof(DbContextOptions<CardRepository>)));
//                 services.AddDbContext<CardRepository>(o => 
//                     o.UseNpgsql(_dbContainer.GetConnectionString()));
//             });
//         });
//     }

//     private async Task<PostCard> CreateCard(
//         HttpClient client,
//         string key = "card-key",
//         string name = "Card Name",
//         string type = "Item",
//         int health = -1,
//         int attack = -1,
//         int evasion = -1,
//         string text = "card text here",
//         string rewardsText = "rewards here",
//         int soulValue = 0,

//         string script = "print('card script missing')",
//         string collectionKey = "testCollection",
//         string imageUrl = "http://test.image"

//     ) {
//         var card = new PostCard {
//             Key = key,
//             Name = name,
//             Type = type,
//             Health = health,
//             Attack = attack,
//             Evasion = evasion,
//             Text = text,
//             RewardsText = rewardsText,
//             SoulValue = soulValue,
//             Script = script,
//             CollectionKey = collectionKey,
//             ImageUrl = imageUrl,
//         };
//         var result = await client.PostAsJsonAsync("/api/v1/Cards", card);
//         result.Should().BeSuccessful();

//         return card;
//     }

//     private static async Task CreateRelation(HttpClient client, string key1, string key2, CardRelationType relationType = CardRelationType.GENERAL, bool assertCreated = true) {
//         var result = await client.PostAsJsonAsync("/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.GENERAL
//         });

//         if (!assertCreated) return;
//         result.StatusCode.Should().Be(HttpStatusCode.Created);
//     }

//     #region Test cases

//     public static IEnumerable<object[]> GoodPostCards => [
//         // TODO add more cases
//         [
//             new PostCard {
//                 Key = "key1",
//                 Name = "Card Name",
//                 Type = "Loot",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 0,
//                 Script = "print('no card script')",
//                 CollectionKey = "col1",
//                 ImageUrl = "http://card.image",
//             },
//         ],
//         [
//             new PostCard {
//                 Key = "key1",
//                 Name = "Card Name",
//                 Type = "Item",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 1,
//                 Script = "print('no card script')",
//                 CollectionKey = "c",
//                 ImageUrl = "http://card.image",
//             },
//         ],
//     ];

//     public static IEnumerable<object[]> BadPostCards => [
//         // TODO add more cases
//         [
//             new PostCard {
//                 Key = "",
//                 Name = "Card Name",
//                 Type = "Loot",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 0,
//                 Script = "print('no card script')",
//                 CollectionKey = "col1",
//                 ImageUrl = "http://card.image",
//             },
//             "card key can't be empty"
//         ],
//         [
//             new PostCard {
//                 Key = "key",
//                 Name = "",
//                 Type = "Loot",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 0,
//                 Script = "print('no card script')",
//                 CollectionKey = "col1",
//                 ImageUrl = "http://card.image",
//             },
//             "card name can't be empty"
//         ],
//         [
//             new PostCard {
//                 Key = "key",
//                 Name = "Card name",
//                 Type = "type",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 0,
//                 Script = "print('no card script')",
//                 CollectionKey = "col1",
//                 ImageUrl = "http://card.image",
//             },
//             "invalid card type"
//         ],
//         [
//             new PostCard {
//                 Key = "key",
//                 Name = "Card name",
//                 Type = "Loot",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = -1,
//                 Script = "print('no card script')",
//                 CollectionKey = "col1",
//                 ImageUrl = "http://card.image",
//             },
//             "card soul value can't be less than 0"
//         ],
//         [
//             new PostCard {
//                 Key = "key",
//                 Name = "Card name",
//                 Type = "Loot",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 0,
//                 Script = "print('no card script')",
//                 CollectionKey = "",
//                 ImageUrl = "http://card.image",
//             },
//             "card collection key can't be empty"
//         ],
//         [
//             new PostCard {
//                 Key = "key",
//                 Name = "Card name",
//                 Type = "Loot",
//                 Health = -1,
//                 Attack = -1,
//                 Evasion = -1,
//                 Text = "card text here",
//                 RewardsText = "rewards text here",
//                 SoulValue = 0,
//                 Script = "print('no card script')",
//                 CollectionKey = "col1",
//                 ImageUrl = "card.image",
//             },
//             "card image has to be a valid URL"
//         ],
//     ];

//     #endregion

//     [Fact]
//     public async Task ShouldFetchAll() {
//         // Arrange
//         var client = _factory.CreateClient();


//         // Act
//         var result = await client.GetAsync("/api/v1/Cards");

//         // Assert
//         result.Should().BeSuccessful();
//         var data = await result.Content.ReadFromJsonAsync<CardsPage>();
//         data.Should().NotBeNull();
//         data!.Cards.Should().BeEmpty();
//     }

//     [Theory, MemberData(nameof(GoodPostCards))]
//     public async Task ShouldCreate(PostCard card) {
//         // Arrange
//         var client = _factory.CreateClient();

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards", card);

//         // Assert
//         result.Should().BeSuccessful();
//         result.Should().HaveStatusCode(HttpStatusCode.Created);
//     }

//     [Theory, MemberData(nameof(BadPostCards))]
//     public async Task ShouldNotCreate(PostCard card, string because) {
//         // Arrange
//         var client = _factory.CreateClient();

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards", card);

//         // Assert
//         result.Should().HaveClientError(because);
//     }

//     [Fact]
//     public async Task ShouldNotCreateDuplicate() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var cardKey = "card1";
//         await CreateCard(client, cardKey);

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards", new PostCard {
//             Key = cardKey,
//             Name = "Card Name",
//             Type = "Loot",
//             Health = -1,
//             Attack = -1,
//             Evasion = -1,
//             Text = "card text here",
//             RewardsText = "rewards text here",
//             SoulValue = 0,
//             Script = "print('no card script')",
//             CollectionKey = "col1",
//             ImageUrl = "http://card.image",
//         });

//         // Assert
//         result.Should().HaveClientError();        
//     }

//     [Fact]
//     public async Task ShouldFetchByKey() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var cardKey = "card-key";
//         var created = await CreateCard(client, key: cardKey);

//         // Act
//         var result = await client.GetAsync($"/api/v1/Cards/{cardKey}");

//         // Assert
//         result.Should().BeSuccessful();
//         var data = await result.Content.ReadFromJsonAsync<GetCardWithRelations>();
//         data.Should().NotBeNull();
//         data!.Should().BeEquivalentTo(
//             created, 
//             o => o.WithMapping("CollectionKey", "Collection")
//         );
//     }

//     [Fact]
//     public async Task ShouldNotFetchByKey() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var cardKey = "card-key";

//         // Act
//         var result = await client.GetAsync($"/api/v1/Cards/{cardKey}");

//         // Assert
//         result.Should().HaveStatusCode(HttpStatusCode.NotFound);
//     }

//     [Fact]
//     public async Task ShouldDelete() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var cardKey = "card-key";
//         await CreateCard(client, cardKey);

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/{cardKey}");

//         // Assert
//         result.Should().BeSuccessful();
//     }

//     [Fact]
//     public async Task ShouldNotDelete() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var cardKey = "card-key";

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/{cardKey}");

//         // Assert
//         result.Should().HaveClientError();
//     }

//     // TODO add card Patch tests

//     // TODO remove, is replaced with /api/v1/Cards/f
//     [Fact]
//     public async Task ShouldFetchFromCollection() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var collection = "test1";
//         await CreateCard(client, key: "card1", collectionKey: collection);
//         await CreateCard(client, key: "card2", collectionKey: "collection");
//         await CreateCard(client, key: "card3", collectionKey: collection);

//         // Act
//         var result = await client.GetAsync($"/api/v1/Cards/From/{collection}");

//         // Assert
//         result.Should().BeSuccessful();
//         var data = await result.Content.ReadFromJsonAsync<CardsPage>();
//         data.Should().NotBeNull();
//         data!.Cards.Should().HaveCount(2);
//     }

//     [Fact]
//     public async Task ShouldCreateRelation() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.GENERAL
//         });

//         // Assert
//         result.StatusCode.Should().Be(HttpStatusCode.Created);
//     }

//     [Fact]
//     public async Task ShouldNotCreateRelation_CardNotFound1() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = "key2";

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.GENERAL
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotCreateRelation_CardNotFound2() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = "key1";
//         var key2 = (await CreateCard(client, key: "card2")).Key;

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.GENERAL
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotCreateRelation_AlreadyExists() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2);

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.GENERAL
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotCreateRelation_WithSelf() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         await CreateRelation(client, key1, key1, assertCreated: false);

//         // Act
//         var result = await client.PostAsJsonAsync("/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key1,
//             RelationType = CardRelationType.GENERAL
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }


//     [Fact]
//     public async Task ShouldCreateAndFetchWithRelation() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2);

//         // Act
//         var result1 = await client.GetAsync($"/api/v1/Cards/{key1}");
//         var result2 = await client.GetAsync($"/api/v1/Cards/{key2}");

//         // Assert
//         result1.Should().BeSuccessful();
//         var data1 = await result1.Content.ReadFromJsonAsync<GetCardWithRelations>();
//         data1.Should().NotBeNull();
//         var relations1 = data1!.Relations;
//         relations1.Should().HaveCount(1);
//         data1!.Key.Should().NotBe(relations1[0].RelatedCard.Key);
//         data1!.Key.Should().Be(relations1[0].RelatedTo.Key);

//         result2.Should().BeSuccessful();
//         var data2 = await result2.Content.ReadFromJsonAsync<GetCardWithRelations>();
//         data2.Should().NotBeNull();
//         var relations2 = data2!.Relations;
//         relations2.Should().HaveCount(1);
//         data2!.Key.Should().NotBe(relations2[0].RelatedTo.Key);
//         data2!.Key.Should().Be(relations2[0].RelatedCard.Key);
//     }

//     [Fact]
//     public async Task ShouldNotCreateAndFetchWithoutRelation1() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = "key2";
//         await CreateRelation(client, key1, key2, assertCreated: false);

//         // Act
//         var result = await client.GetAsync($"/api/v1/Cards/{key1}");

//         // Assert
//         result.Should().BeSuccessful();
//         var data1 = await result.Content.ReadFromJsonAsync<GetCardWithRelations>();
//         data1.Should().NotBeNull();
//         var relations1 = data1!.Relations;
//         relations1.Should().BeEmpty();
//     }

//     [Fact]
//     public async Task ShouldNotCreateAndFetchWithoutRelation2() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = "key1";
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2, assertCreated: false);

//         // Act
//         var result = await client.GetAsync($"/api/v1/Cards/{key2}");

//         // Assert
//         result.Should().BeSuccessful();
//         var data1 = await result.Content.ReadFromJsonAsync<GetCardWithRelations>();
//         data1.Should().NotBeNull();
//         var relations1 = data1!.Relations;
//         relations1.Should().BeEmpty();
//     }

//     [Fact]
//     public async Task ShouldEditRelationType1() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2);

//         // Act
//         var result = await client.PatchAsJsonAsync($"/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.STARTING_ITEM
//         });

//         // Assert
//         result.Should().BeSuccessful();
//     }
    
//     [Fact]
//     public async Task ShouldEditRelationType2() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2);

//         // Act
//         var result = await client.PatchAsJsonAsync($"/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key2,
//             RelatedCardKey = key1,
//             RelationType = CardRelationType.STARTING_ITEM
//         });

//         // Assert
//         result.Should().BeSuccessful();
//     }

//     [Fact]
//     public async Task ShouldNotEditRelationType_DoesntExist() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;

//         // Act
//         var result = await client.PatchAsJsonAsync($"/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.STARTING_ITEM
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotEditRelationType_CardNotFound1() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = "key2";
//         await CreateRelation(client, key1, key2, assertCreated: false);

//         // Act
//         var result = await client.PatchAsJsonAsync($"/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.STARTING_ITEM
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotEditRelationType_CardNotFound2() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = "key1";
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2, assertCreated: false);

//         // Act
//         var result = await client.PatchAsJsonAsync($"/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key2,
//             RelationType = CardRelationType.STARTING_ITEM
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotEditRelationType_WithSelf() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         await CreateRelation(client, key1, key1, assertCreated: false);

//         // Act
//         var result = await client.PatchAsJsonAsync($"/api/v1/Cards/Relation", new PostCardRelationWithType {
//             CardKey = key1,
//             RelatedCardKey = key1,
//             RelationType = CardRelationType.STARTING_ITEM
//         });

//         // Assert
//         result.Should().HaveClientError();
//     }


//     [Fact]
//     public async Task ShouldDeleteRelation() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;
//         await CreateRelation(client, key1, key2);

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/Relation/{key1}/{key2}");

//         // Assert
//         result.Should().BeSuccessful();
//     }

//     [Fact]
//     public async Task ShouldNotDeleteRelation_DoesntExist() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = (await CreateCard(client, key: "card2")).Key;

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/Relation/{key1}/{key2}");

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotDeleteRelation_CardNotFound1() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;
//         var key2 = "key2";

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/Relation/{key1}/{key2}");

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotDeleteRelation_CardNotFound2() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = "key1";
//         var key2 = (await CreateCard(client, key: "card2")).Key;

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/Relation/{key1}/{key2}");

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldNotDeleteRelation_WithSelf() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var key1 = (await CreateCard(client, key: "card1")).Key;

//         // Act
//         var result = await client.DeleteAsync($"/api/v1/Cards/Relation/{key1}/{key1}");

//         // Assert
//         result.Should().HaveClientError();
//     }

//     [Fact]
//     public async Task ShouldGetCollections() {
//         // Arrange
//         var client = _factory.CreateClient();
//         var col1 = "col1";
//         var col2 = "col2";
//         await CreateCard(client, key: "card1", collectionKey: col1);
//         await CreateCard(client, key: "card2", collectionKey: col1);
//         await CreateCard(client, key: "card3", collectionKey: col1);
//         await CreateCard(client, key: "card4", collectionKey: col2);

//         // Act
//         var result = await client.GetAsync("/api/v1/Cards/Collections");

//         // Assert
//         result.Should().BeSuccessful();
//         var data = await result.Content.ReadFromJsonAsync<IEnumerable<GetCollection>>();
//         data.Should().NotBeNull();
//         data!.Should().HaveCount(2);
//         data!.Should().BeEquivalentTo(new List<GetCollection>() {
//             new() {
//                 Name = col1,
//                 CardCount = 3,
//             },
//             new() {
//                 Name = col2,
//                 CardCount = 1,
//             },
//         });

//     }
   
// }