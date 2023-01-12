using Newtonsoft.Json;
using GitInsight.Entities.DTOS;

namespace GitInsight.WebApi.Tests;

public class AuthorEndpointTest : IClassFixture<CustomWebApplicationFactory> {

    private readonly HttpClient client;

    public AuthorEndpointTest(CustomWebApplicationFactory factory){
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Get()
    {
        var authors = await client.GetFromJsonAsync<AuthorDTO[]>("analysis/TestUser/TestRepo2/author");

        var commitDateTime1 = new DateTime(2022, 10, 01);
        var commitDateTime2 = new DateTime(2022, 10, 02);

        Assert.Equal("aarv", authors![0].author);
        Assert.Equivalent(new List<FrequencyDTO>{new FrequencyDTO {date = commitDateTime2, frequency = 1}, new FrequencyDTO {date = commitDateTime1, frequency = 1}}, authors[0].frequencies);
        Assert.Equal("mlth", authors![1].author);
        Assert.Equivalent(new List<FrequencyDTO>{new FrequencyDTO {date = commitDateTime1, frequency = 1}}, authors[1].frequencies);
    }
}