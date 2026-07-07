using IspsDashboard.Models.Entities;

namespace IspsDashboard.Services.Interfaces;

public interface IBadgePdfService
{
    /// <summary>Planche A4 des QR codes des checkpoints, à imprimer et coller sur le terrain.</summary>
    byte[] BuildCheckpointSheet(IReadOnlyList<Checkpoint> checkpoints, string scanBaseUrl);

    /// <summary>Badge imprimable d'un laissez-passer avec QR de vérification.</summary>
    byte[] BuildAccessPassBadge(AccessPass pass, string terminalTitle);
}
