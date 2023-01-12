using Newtonsoft.Json;
using GitInsight.Entities.DTOS;

namespace GitInsight.WebApi.Tests;
public class FrequencyEndpointTest : IClassFixture<CustomWebApplicationFactory> {

    private readonly HttpClient client;

    public FrequencyEndpointTest(CustomWebApplicationFactory factory){
        client = factory.CreateClient();
    }

    [Fact]
    public async Task Get()
    {
        var frequencies = await client.GetFromJsonAsync<FrequencyDTO[]>("analysis/TestUser/TestRepo1/frequency");

        var commitDate1 = frequencies![0].date;
        var commitDate2 = frequencies![1].date;

        Assert.Equal(2, frequencies[0].frequency);
        Assert.Equivalent(new DateTime(2022, 10, 01), commitDate1);
        Assert.Equal(1, frequencies[1].frequency);
        Assert.Equal(new DateTime(2022, 10, 02), commitDate2);
    }
}