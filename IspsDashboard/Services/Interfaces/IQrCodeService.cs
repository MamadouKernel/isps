namespace IspsDashboard.Services.Interfaces;

public interface IQrCodeService
{
    /// <summary>Génère un QR code PNG pour le contenu donné.</summary>
    byte[] GeneratePng(string content, int pixelsPerModule = 10);
}
