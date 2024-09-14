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

    public Task InitializeAsync() {
        return _dbContainer.StartAsync();
    }

    public Task DisposeAsync() {
        return _dbContainer.StopAsync();
    }

    public CardsIntegrationTests(WebApplicationFactory<Program> factory, ITestOutputHelper output)
    {
        _output = output;
        _dbContainer = new PostgreSqlBuilder()
            .Build();

        _factory = factory.WithWebHostBuilder(builder => {
            builder.ConfigureServices(services => {
                services.AddDbContext<CardRepository>(o => 
                    o.UseNpgsql(_dbContainer.GetConnectionString()));
            });
        });
    }

    [Fact]
    public async Task ShouldFetchAll() {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var result = await client.GetAsync("/api/v1/Card");

        // Assert
        result.Should().BeSuccessful();
    }
}