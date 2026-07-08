using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IspsDashboard.Tests.Integration;

/// <summary>
/// Vérifie qu'aucun module opérationnel n'est accessible sans connexion — une régression ici
/// serait un oubli de [Authorize] sur un contrôleur entier, pas juste sur une action isolée
/// (déjà couvert pour BriefingsController.Acknowledge par ailleurs).
/// </summary>
[Collection("Integration")]
public class AnonymousAccessTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AnonymousAccessTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Theory]
    [InlineData("/Dashboard")]
    [InlineData("/Admin")]
    [InlineData("/Incidents")]
    [InlineData("/Visitors")]
    [InlineData("/Patrols")]
    [InlineData("/Zones")]
    [InlineData("/Vessels")]
    [InlineData("/Cameras")]
    [InlineData("/Contacts")]
    [InlineData("/Briefings")]
    [InlineData("/Marsec")]
    [InlineData("/Personnel")]
    [InlineData("/Habilitations")]
    [InlineData("/Documents")]
    [InlineData("/NonConformities")]
    [InlineData("/Audits")]
    [InlineData("/Rex")]
    [InlineData("/Analytics")]
    [InlineData("/Reports")]
    [InlineData("/AccessPass")]
    [InlineData("/VehicleAccess")]
    [InlineData("/Users")]
    [InlineData("/Trash")]
    public async Task ProtectedController_WithoutAuthentication_RedirectsToLogin(string path)
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString ?? "");
    }
}
