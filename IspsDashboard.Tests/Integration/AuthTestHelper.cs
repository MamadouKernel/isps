using System.Text.RegularExpressions;
using IspsDashboard.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IspsDashboard.Tests.Integration;

internal static class AuthTestHelper
{
    private static readonly Regex TokenRegex = new(
        @"name=""__RequestVerificationToken""[^>]*value=""([^""]+)""",
        RegexOptions.Compiled);

    public static async Task<string> GetAntiForgeryTokenAsync(HttpClient client, string url = "/Account/Login")
    {
        var html = await client.GetStringAsync(url);
        var match = TokenRegex.Match(html);
        if (!match.Success) throw new InvalidOperationException($"Jeton antiforgery introuvable sur {url}.");
        return match.Groups[1].Value;
    }

    /// <summary>Connecte un client HTTP réel (cookies inclus) via le vrai formulaire de login.</summary>
    public static async Task<HttpClient> LoginAsync(
        CustomWebApplicationFactory factory, string email, string password, string? returnUrl = null, bool allowAutoRedirect = true)
    {
        var (client, _) = await LoginRawAsync(factory, email, password, returnUrl, allowAutoRedirect);
        return client;
    }

    /// <summary>Variante exposant la réponse HTTP brute du POST de login (ex. pour inspecter une redirection).</summary>
    public static async Task<(HttpClient Client, HttpResponseMessage Response)> LoginRawAsync(
        CustomWebApplicationFactory factory, string email, string password, string? returnUrl = null, bool allowAutoRedirect = false)
    {
        var client = factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = allowAutoRedirect });
        var loginUrl = returnUrl is null ? "/Account/Login" : $"/Account/Login?ReturnUrl={Uri.EscapeDataString(returnUrl)}";
        var token = await GetAntiForgeryTokenAsync(client, loginUrl);

        var form = new Dictionary<string, string>
        {
            ["Email"] = email,
            ["Password"] = password,
            ["__RequestVerificationToken"] = token,
        };
        if (returnUrl is not null) form["ReturnUrl"] = returnUrl;

        var response = await client.PostAsync(loginUrl, new FormUrlEncodedContent(form));
        return (client, response);
    }

    /// <summary>Crée (si besoin) un utilisateur de test avec un rôle donné, directement via Identity.</summary>
    public static async Task EnsureUserWithRoleAsync(CustomWebApplicationFactory factory, string email, string password, string role)
    {
        using var scope = factory.Services.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var existing = await userManager.FindByEmailAsync(email);
        if (existing is not null) return;

        var user = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, FullName = "Utilisateur de test" };
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
            throw new InvalidOperationException(string.Join(" ; ", result.Errors.Select(e => e.Description)));

        var roleResult = await userManager.AddToRoleAsync(user, role);
        if (!roleResult.Succeeded)
            throw new InvalidOperationException(string.Join(" ; ", roleResult.Errors.Select(e => e.Description)));
    }
}
