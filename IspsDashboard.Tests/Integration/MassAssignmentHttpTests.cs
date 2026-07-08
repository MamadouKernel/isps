using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IspsDashboard.Tests.Integration;

/// <summary>
/// Preuve bout-en-bout (vrai POST HTTP, vrai jeton antiforgery, vrai pipeline de binding MVC)
/// que le correctif d'assignation de masse tient — pas seulement au niveau service comme
/// MassAssignmentSecurityTests, mais depuis la requête HTTP elle-même.
/// </summary>
[Collection("Integration")]
public class MassAssignmentHttpTests
{
    private readonly CustomWebApplicationFactory _factory;

    public MassAssignmentHttpTests(CustomWebApplicationFactory factory) => _factory = factory;

    [Fact]
    public async Task PostingExtraFields_ToAuditCreate_CannotForgeClosedStatus()
    {
        var client = await AuthTestHelper.LoginAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword);
        var token = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Audits/Create");

        // Un attaquant ajoute des champs de formulaire absents de l'écran de création réel.
        var form = new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Title"] = "Audit forgé via HTTP",
            ["Type"] = "0",
            ["ScheduledDate"] = DateTime.UtcNow.Date.ToString("yyyy-MM-dd"),
            ["Status"] = "2",                              // AuditStatus.Cloture
            ["CompletedDate"] = DateTime.UtcNow.ToString("yyyy-MM-dd"),
            ["seedChecklist"] = "false",
        };

        var response = await client.PostAsync("/Audits/Create", new FormUrlEncodedContent(form));
        Assert.True(response.StatusCode is System.Net.HttpStatusCode.Redirect or System.Net.HttpStatusCode.OK);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var saved = await db.SecurityAudits
            .OrderByDescending(a => a.Id)
            .FirstAsync(a => a.Title == "Audit forgé via HTTP");

        Assert.Equal(IspsDashboard.Models.Enums.AuditStatus.Planifie, saved.Status);
        Assert.Null(saved.CompletedDate);
    }
}
