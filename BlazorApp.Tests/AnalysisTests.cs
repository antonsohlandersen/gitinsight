using Radzen.Blazor;
using Newtonsoft.Json;
using GitInsight.Entities.DTOS;
using GitInsight.WebApi.Tests;
using System.Net.Http.Json;

namespace BlazorApp.Tests;


public class AnalysisTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient client;
    private AnalysisCode analysisCode;
    private TestContext ctx;

    public AnalysisTests(CustomWebApplicationFactory factory) 
    {
        client = factory.CreateClient();
        analysisCode = AnalysisCode.Instance;
        ctx = new TestContext();
    }

    [Fact]
    public async Task GetAuthorAnalysis_Retrieves_Correct_Data()
    {
        var commitDateTime1 = new DateTime(2022, 10, 01);
        var commitDateTime2 = new DateTime(2022, 10, 02);
        using var ctx = new TestContext();
        var component = ctx.RenderComponent<Analysis>();
        var radzenComponent =  component.FindComponent<RadzenTextBox>().Instance;
        var radzenButtons = component.FindComponents<RadzenButton>();
        var radzenAuthorButton = radzenButtons[0].Instance;

        await component.InvokeAsync(async () => 
        await radzenComponent.Change.InvokeAsync("TestUser/TestRepo2"));
        await analysisCode.getAuthorAnalysis(client);

        Assert.Equal("aarv", analysisCode.authorAnalysis[0].author);
        Assert.Equivalent(new List<FrequencyDTO>{new FrequencyDTO {date = commitDateTime2, frequency = 1}, new FrequencyDTO {date = commitDateTime1, frequency = 1}}, analysisCode.authorAnalysis[0].frequencies);
        Assert.Equal("mlth", analysisCode.authorAnalysis[1].author);
        Assert.Equivalent(new List<FrequencyDTO>{new FrequencyDTO {date = commitDateTime1, frequency = 1}}, analysisCode.authorAnalysis[1].frequencies);
    }
    
    [Fact]
    public void TextBox_Renders_ValueParameter()
    {
        using var ctx = new TestContext();
        var component = ctx.RenderComponent<Analysis>();
        var radzenComponent =  component.FindComponent<RadzenTextBox>();

        var value = "Test";
        radzenComponent.SetParametersAndRender(parameters => parameters.Add(p => p.Value, value));

        Assert.Contains(@$"value=""{value}""", component.Markup);
    }

    [Fact]
    public void Button_Renders_TextParameter()
    {
        using var ctx = new TestContext();
        var component = ctx.RenderComponent<Analysis>();
        var radzenCom = component.FindComponent<RadzenButton>();

        var text = "Test";
        radzenCom.SetParametersAndRender(parameters => parameters.Add(p => p.Text, text));

        Assert.Contains(@$"<span class=""rz-button-text"">{text}</span>", radzenCom.Markup);
    }
}