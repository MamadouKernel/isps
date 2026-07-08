using System.Net;
using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IspsDashboard.Tests.Integration;

/// <summary>
/// Aller-retours Create/Index/Delete bout-en-bout sur un échantillon de modules non couverts
/// par les autres classes de tests d'intégration, pour élargir la couverture au-delà du seul
/// flux d'authentification.
/// </summary>
[Collection("Integration")]
public class CrudFlowTests
{
    private readonly CustomWebApplicationFactory _factory;

    public CrudFlowTests(CustomWebApplicationFactory factory) => _factory = factory;

    private Task<HttpClient> LoginAsAdminAsync() =>
        AuthTestHelper.LoginAsync(
            _factory, CustomWebApplicationFactory.AdminEmail, CustomWebApplicationFactory.AdminPassword, allowAutoRedirect: false);

    [Fact]
    public async Task RestrictedZone_CreateThenIndex_ShowsTheNewZone()
    {
        var client = await LoginAsAdminAsync();
        var token = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Zones/Create");

        var response = await client.PostAsync("/Zones/Create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Code"] = "ZAR-CRUD-TEST",
            ["Name"] = "Zone de test CRUD",
            ["AccessLevel"] = "0",
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);

        var index = await client.GetStringAsync("/Zones");
        Assert.Contains("ZAR-CRUD-TEST", index);
    }

    [Fact]
    public async Task Camera_CreateThenDelete_RemovesItFromIndex()
    {
        var client = await LoginAsAdminAsync();
        var token = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Cameras/Create");

        await client.PostAsync("/Cameras/Create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Code"] = "CCTV-CRUD-TEST",
            ["Label"] = "Caméra de test CRUD",
        }));

        using (var scope = _factory.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var created = await db.Cameras.SingleAsync(c => c.Code == "CCTV-CRUD-TEST");

            var deleteToken = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Cameras/Index");
            var deleteResponse = await client.PostAsync("/Cameras/Delete", new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["__RequestVerificationToken"] = deleteToken,
                ["id"] = created.Id.ToString(),
            }));
            Assert.Equal(HttpStatusCode.Redirect, deleteResponse.StatusCode);
        }

        var index = await client.GetStringAsync("/Cameras");
        Assert.DoesNotContain("CCTV-CRUD-TEST", index);
    }

    [Fact]
    public async Task Checkpoint_AddThenDelete_RoundTripsCorrectly()
    {
        // Régression fonctionnelle : ce CRUD (Patrols/AddCheckpoint, DeleteCheckpoint) a été
        // ajouté pendant cette session — il n'existait pas auparavant (constat de l'audit).
        var client = await LoginAsAdminAsync();
        var token = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Patrols");

        var addResponse = await client.PostAsync("/Patrols/AddCheckpoint", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["code"] = "CP-CRUD-TEST",
            ["label"] = "Checkpoint de test",
            ["targetIntervalMinutes"] = "240",
        }));
        Assert.Equal(HttpStatusCode.Redirect, addResponse.StatusCode);

        var indexAfterAdd = await client.GetStringAsync("/Patrols");
        Assert.Contains("CP-CRUD-TEST", indexAfterAdd);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var created = await db.Checkpoints.SingleAsync(c => c.Code == "CP-CRUD-TEST");

        var deleteToken = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Patrols");
        await client.PostAsync("/Patrols/DeleteCheckpoint", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = deleteToken,
            ["id"] = created.Id.ToString(),
        }));

        var indexAfterDelete = await client.GetStringAsync("/Patrols");
        Assert.DoesNotContain("CP-CRUD-TEST", indexAfterDelete);
    }

    [Fact]
    public async Task ExternalContact_Create_PersistsAndAppearsInIndex()
    {
        var client = await LoginAsAdminAsync();
        var token = await AuthTestHelper.GetAntiForgeryTokenAsync(client, "/Contacts/Create");

        var response = await client.PostAsync("/Contacts/Create", new FormUrlEncodedContent(new Dictionary<string, string>
        {
            ["__RequestVerificationToken"] = token,
            ["Name"] = "Contact de test CRUD",
        }));

        Assert.Equal(HttpStatusCode.Redirect, response.StatusCode);
        var index = await client.GetStringAsync("/Contacts");
        Assert.Contains("Contact de test CRUD", index);
    }
}
