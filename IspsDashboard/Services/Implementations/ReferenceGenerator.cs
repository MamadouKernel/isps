using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

/// <summary>
/// Sauvegarde une entité dont la référence métier (ex. INC-2026-0001) est calculée à partir
/// du dernier numéro en base. Deux créations concurrentes peuvent calculer le même numéro ;
/// l'index unique sur la colonne Reference rejette alors l'INSERT en doublon, et on relance
/// le calcul + la sauvegarde plutôt que de laisser planter la requête.
/// </summary>
internal static class ReferenceGenerator
{
    private const int MaxAttempts = 5;

    public static async Task SaveWithUniqueReferenceAsync(
        DbContext db,
        Action<string> assignReference,
        Func<string> computeNextReference)
    {
        for (var attempt = 1; ; attempt++)
        {
            assignReference(computeNextReference());
            try
            {
                await db.SaveChangesAsync();
                return;
            }
            catch (DbUpdateException ex) when (attempt < MaxAttempts && IsUniqueReferenceViolation(ex))
            {
            }
        }
    }

    private static bool IsUniqueReferenceViolation(DbUpdateException ex)
        => ex.InnerException is SqlException { Number: 2601 or 2627 };
}
