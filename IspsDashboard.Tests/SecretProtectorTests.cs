using IspsDashboard.Services.Implementations;
using Microsoft.AspNetCore.DataProtection;
using Xunit;

namespace IspsDashboard.Tests;

public class SecretProtectorTests
{
    private readonly DataProtectionSecretProtector _protector;

    public SecretProtectorTests()
    {
        var provider = DataProtectionProvider.Create("IspsDashboardTests");
        _protector = new DataProtectionSecretProtector(provider);
    }

    [Fact]
    public void ProtectThenUnprotect_ShouldReturnOriginal()
    {
        const string plain = "MyS3cret!Smtp";
        var cipher = _protector.Protect(plain);

        Assert.NotEqual(plain, cipher);
        Assert.StartsWith("prot:", cipher);
        Assert.Equal(plain, _protector.Unprotect(cipher));
    }

    [Fact]
    public void Protect_OnEmptyString_ReturnsEmpty()
    {
        Assert.Equal(string.Empty, _protector.Protect(string.Empty));
    }

    [Fact]
    public void Protect_OnAlreadyProtectedValue_ReturnsSameValue()
    {
        var cipher = _protector.Protect("hello");
        var twice = _protector.Protect(cipher);
        Assert.Equal(cipher, twice);
    }

    [Fact]
    public void Unprotect_OnLegacyPlaintext_ReturnsAsIs()
    {
        // Migration douce : valeurs en clair stockées avant le chiffrement
        Assert.Equal("legacy", _protector.Unprotect("legacy"));
    }
}
