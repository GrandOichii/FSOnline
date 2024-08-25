

using AutoMapper;
using FluentAssertions;
using FSManager.Mapping;

public class CardServiceTests {
    private readonly IMapper _mapper;

    public CardServiceTests() {
        _mapper = new Mapper(
            new MapperConfiguration(cfg => {
                cfg.AddProfile(new AutoMapperProfile());
            })
        );
    }

    [Fact]
    public async Task Test1() {
        "test".Should().Be("test");
    }
}