namespace IspsDashboard.Services.Interfaces;

/// <summary>
/// Chiffre / déchiffre des secrets (ex: mot de passe SMTP) avant persistance en BDD.
/// </summary>
public interface ISecretProtector
{
    string Protect(string plaintext);
    string Unprotect(string ciphertext);
}
