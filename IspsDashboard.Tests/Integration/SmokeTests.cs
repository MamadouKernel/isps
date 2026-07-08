using System.Net;
using Xunit;

namespace IspsDashboard.Tests.Integration;

[Collection("Integration")]
public class SmokeTests
{
    private readonly CustomWebApplicationFactory _factory;

    public SmokeTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task LoginPage_ShouldReturn200()
    {
        var client = _factory.CreateClient();
        var response = await client.GetAsync("/Account/Login");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
