using IspsDashboard.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IspsDashboard.Tests;

/// <summary>
/// Héberge l'application réelle (Program.cs, pipeline HTTP complet, Identity, SignalR, Quartz)
/// pour des tests d'intégration bout-en-bout, avec SQL Server remplacé par SQLite en mémoire.
/// Une seule connexion SQLite est gardée ouverte pour la durée de vie de la factory : une base
/// SQLite ":memory:" est détruite dès que sa dernière connexion se ferme.
/// </summary>
public sealed class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    public const string AdminEmail = "admin@test.local";
    public const string AdminPassword = "Test@2026!";

    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public CustomWebApplicationFactory() => _connection.Open();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Seed:AdminEmail"] = AdminEmail,
                ["Seed:AdminPassword"] = AdminPassword,
                ["Seed:ReferenceData"] = "false",
                ["Hosting:UseHttps"] = "false",
            });
        });

        builder.ConfigureServices(services =>
        {
            // AddDbContext<T>() enregistre plus que le seul descripteur DbContextOptions<T> — sur
            // les versions récentes d'EF Core, la configuration UseSqlServer(...) de Program.cs est
            // câblée via des descripteurs IDbContextOptionsConfiguration<T> qui persistent si on ne
            // retire que DbContextOptions<T>, provoquant "deux fournisseurs enregistrés" au démarrage.
            var toRemove = services.Where(d =>
                d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>) ||
                d.ServiceType == typeof(DbContextOptions) ||
                (d.ServiceType.IsGenericType && d.ServiceType.GetGenericTypeDefinition().Name.Contains("DbContextOptionsConfiguration")))
                .ToList();
            foreach (var d in toRemove) services.Remove(d);

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(_connection));
        });
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) _connection.Dispose();
        base.Dispose(disposing);
    }
}
