using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Implementations;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace IspsDashboard.Tests;

/// <summary>
/// Vérifie que RowVersion est bien câblé de bout en bout : une modification concurrente doit
/// lever DbUpdateConcurrencyException plutôt que d'écraser silencieusement l'autre écriture.
/// Régression pour l'audit du 08/07/2026 (RowVersion déclaré mais jamais vérifié sur 5 entités).
///
/// Le fournisseur EF Core InMemory ne génère pas automatiquement les octets RowVersion comme le
/// ferait SQL Server : ils sont donc assignés manuellement ici pour simuler ce que produirait un
/// vrai SGBD. L'objectif du test n'est pas de vérifier la génération du jeton (comportement du
/// SGBD, hors périmètre), mais que le service compare bien l'OriginalValue et lève l'exception
/// attendue en cas de conflit — exactement ce qui manquait avant le correctif.
/// </summary>
public class OptimisticConcurrencyTests
{
    [Fact]
    public async Task CameraUpdate_ShouldThrow_WhenRowVersionIsStale()
    {
        var dbName = Guid.NewGuid().ToString();
        var db = TestHelpers.CreateInMemoryDb(dbName);
        var camera = new Camera { Code = "CCTV-01", Label = "Portail nord", RowVersion = new byte[] { 1 } };
        db.Cameras.Add(camera);
        await db.SaveChangesAsync();
        var staleRowVersion = camera.RowVersion!.ToArray();

        // Une deuxième "session" (même base en mémoire) modifie la même caméra en premier —
        // RowVersion change, comme le ferait SQL Server automatiquement à chaque écriture.
        var db2 = TestHelpers.CreateInMemoryDb(dbName);
        var fromDb2 = await db2.Cameras.FirstAsync(c => c.Id == camera.Id);
        fromDb2.Label = "Modifié par un autre utilisateur";
        fromDb2.RowVersion = new byte[] { 2 };
        await db2.SaveChangesAsync();

        var service = new CameraService(db, new TestHelpers.NoOpAuditService());
        var input = new Camera { Id = camera.Id, Label = "Ma modification", Code = camera.Code };

        await Assert.ThrowsAsync<DbUpdateConcurrencyException>(
            () => service.UpdateAsync(input, staleRowVersion));
    }

    [Fact]
    public async Task CameraUpdate_ShouldSucceed_WhenRowVersionIsCurrent()
    {
        var db = TestHelpers.CreateInMemoryDb();
        var camera = new Camera { Code = "CCTV-02", Label = "Portail sud", RowVersion = new byte[] { 1 } };
        db.Cameras.Add(camera);
        await db.SaveChangesAsync();

        var service = new CameraService(db, new TestHelpers.NoOpAuditService());
        var input = new Camera { Id = camera.Id, Label = "Portail sud rénové", Code = camera.Code };

        var ok = await service.UpdateAsync(input, camera.RowVersion!.ToArray());

        Assert.True(ok);
        Assert.Equal("Portail sud rénové", (await db.Cameras.FindAsync(camera.Id))!.Label);
    }
}
