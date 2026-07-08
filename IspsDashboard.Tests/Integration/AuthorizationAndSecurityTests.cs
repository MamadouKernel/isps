using System.Net;
using IspsDashboard.Data.Seed;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IspsDashboard.Tests.Integration;

/// <summary>
/// Tests bout-en-bout pour l'autorisation, le CSRF et les en-têtes de sécurité — chacun
/// couvre un constat concret de l'audit du 08/07/2026, pas une vérification générique.
/// </summary>
[Collection("Integration")]
public class AuthorizationAndSecurityTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AuthorizationAndSecurityTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task AnonymousRequest_ToProtectedPage_RedirectsToLogin()
    {
        var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });

        var response = await client.GetAsync("/Dashboard");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/Login", response.Headers.Location?.OriginalString ?? "");
    }

    [Fact]
    public async Task Uploads_WithoutAuthentication_Returns401()
    {
        // Régression : wwwroot/uploads (pièces jointes incidents, documents PFSP) était servi
        // par UseStaticFiles() sans aucune vérification d'authentification.
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/uploads/anything.pdf");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PublicAssets_RemainAccessible_WithoutAuthentication()
    {
        // Le correctif ci-dessus ne doit gager QUE /uploads, pas le reste des fichiers statiques
        // nécessaires à la page de connexion elle-même (logo, CSS compilé, JS).
        var client = _factory.CreateClient();

        var css = await client.GetAsync("/css/tailwind.css");
        var js = await client.GetAsync("/js/site.js");

        Assert.Equal(HttpStatusCode.OK, css.StatusCode);
        Assert.Equal(HttpStatusCode.OK, js.StatusCode);
    }

    [Fact]
    public async Task SecurityHeaders_ArePresent_OnEveryResponse()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/Account/Login");

        Assert.Equal("DENY", response.Headers.GetValues("X-Frame-Options").FirstOrDefault());
        Assert.Equal("nosniff", response.Headers.GetValues("X-Content-Type-Options").FirstOrDefault());
        Assert.True(response.Headers.Contains("Content-Security-Policy"));
        var csp = response.Headers.GetValues("Content-Security-Policy").First();
        Assert.DoesNotContain("unsafe-inline", csp);
        Assert.Contains("frame-ancestors 'none'", csp);
    }

    [Fact]
    public async Task MutatingPost_WithoutAntiForgeryToken_IsRejected()
    {
        var client = await AuthTestHelper.LoginAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword);

        // POST direct sans passer par le formulaire réel : pas de jeton __RequestVerificationToken.
        var response = await client.PostAsync("/Admin/SaveSettings", new FormUrlEncodedContent(
            new Dictionary<string, string> { ["Settings.TerminalTitle"] = "Piraté" }));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Reader_CannotAccessEditorOnlyAction_Gets403()
    {
        const string email = "lecteur@test.local";
        const string password = "Lecteur@2026!";
        await AuthTestHelper.EnsureUserWithRoleAsync(_factory, email, password, DataSeeder.ReaderRole);
        var client = await AuthTestHelper.LoginAsync(_factory, email, password, allowAutoRedirect: false);

        // Admin/Index requiert la policy RequireEditor — un Lecteur doit être refusé. En
        // authentification par cookie, un échec d'autorisation (Forbid) ne renvoie pas un 403
        // brut : il redirige vers AccessDeniedPath ("/Account/AccessDenied", configuré dans
        // Program.cs) — c'est ce comportement-là qu'il faut vérifier, pas un code 403 direct.
        var response = await client.GetAsync("/Admin");

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        Assert.Contains("/Account/AccessDenied", response.Headers.Location?.OriginalString ?? "");
    }

    [Fact]
    public async Task Editor_CanAccessAdminPanel()
    {
        const string email = "editeur@test.local";
        const string password = "Editeur@2026!";
        await AuthTestHelper.EnsureUserWithRoleAsync(_factory, email, password, DataSeeder.EditorRole);
        var client = await AuthTestHelper.LoginAsync(_factory, email, password);

        var response = await client.GetAsync("/Admin");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
