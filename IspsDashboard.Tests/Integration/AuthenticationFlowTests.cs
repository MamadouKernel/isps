using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IspsDashboard.Tests.Integration;

/// <summary>
/// Tests bout-en-bout sur le vrai pipeline HTTP (WebApplicationFactory) pour les
/// comportements de connexion vérifiés manuellement pendant l'audit du 08/07/2026.
/// </summary>
[Collection("Integration")]
public class AuthenticationFlowTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthenticationFlowTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task Login_WithValidCredentials_Succeeds()
    {
        var (_, response) = await AuthTestHelper.LoginRawAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword);

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        // RedirectToAction("Index", "Dashboard") omet les segments qui correspondent à la route
        // par défaut ({controller=Dashboard}/{action=Index}/{id?}) : l'URL générée est "/".
        Assert.Equal("/", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsGenericError_NotAccountSpecific()
    {
        var (_, response) = await AuthTestHelper.LoginRawAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, "MotDePasseIncorrect1!");

        var body = await response.Content.ReadAsStringAsync();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode); // ré-affiche le formulaire, pas de redirection
        Assert.Contains("Email ou mot de passe incorrect", body);
    }

    [Fact]
    public async Task Login_WithNonExistentEmail_ReturnsIdenticalErrorMessage_NoAccountEnumeration()
    {
        // Régression : avant correctif, un compte verrouillé affichait un message différent
        // ("Compte verrouillé"), permettant de déterminer par tâtonnement qu'un email existe.
        var (_, response) = await AuthTestHelper.LoginRawAsync(
            _factory, "ce-compte-n-existe-pas@test.local", "PeuImporte1!");

        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Email ou mot de passe incorrect", body);
        Assert.DoesNotContain("verrouillé", body);
    }

    [Fact]
    public async Task Login_WithExternalReturnUrl_RedirectsLocally_NotToExternalSite()
    {
        // Régression : Redirect(model.ReturnUrl) sans Url.IsLocalUrl() permettait un lien de
        // phishing (/Account/Login?ReturnUrl=https://evil.example.com) après une connexion réelle.
        var (_, response) = await AuthTestHelper.LoginRawAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword,
            returnUrl: "https://evil-phishing-site.example.com");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var location = response.Headers.Location?.OriginalString ?? "";
        Assert.DoesNotContain("evil-phishing-site", location);
        Assert.Equal("/", location);
    }

    [Fact]
    public async Task Login_WithInternalReturnUrl_RedirectsThere()
    {
        var (_, response) = await AuthTestHelper.LoginRawAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword,
            returnUrl: "/Zones");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Equal("/Zones", response.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task AuthenticatedRequest_CanReachDashboard()
    {
        var client = await AuthTestHelper.LoginAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword);

        var response = await client.GetAsync("/Dashboard");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
