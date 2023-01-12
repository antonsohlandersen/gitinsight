namespace BlazorApp.Tests;

public class TestComponentTests
{
    [Fact]
    public void TestComponent()
    {
        using var ctx = new TestContext();
        var cut = ctx.RenderComponent<TestComponent>();
        cut.MarkupMatches("<p>Hello from TestComponent</p>");
    }
}