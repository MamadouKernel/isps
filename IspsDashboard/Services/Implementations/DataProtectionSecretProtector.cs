using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.DataProtection;

namespace IspsDashboard.Services.Implementations;

public sealed class DataProtectionSecretProtector : ISecretProtector
{
    private const string Purpose = "IspsDashboard.Smtp.Password.v1";
    private const string Marker = "prot:"; // marqueur pour distinguer ciphertext vs legacy plaintext

    private readonly IDataProtector _protector;

    public DataProtectionSecretProtector(IDataProtectionProvider provider)
        => _protector = provider.CreateProtector(Purpose);

    public string Protect(string plaintext)
    {
        if (string.IsNullOrEmpty(plaintext)) return string.Empty;
        if (plaintext.StartsWith(Marker, StringComparison.Ordinal)) return plaintext;
        return Marker + _protector.Protect(plaintext);
    }

    public string Unprotect(string ciphertext)
    {
        if (string.IsNullOrEmpty(ciphertext)) return string.Empty;
        if (!ciphertext.StartsWith(Marker, StringComparison.Ordinal)) return ciphertext; // legacy plaintext
        try { return _protector.Unprotect(ciphertext.Substring(Marker.Length)); }
        catch { return string.Empty; }
    }
}
