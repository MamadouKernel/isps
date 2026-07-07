namespace IspsDashboard.Services.Interfaces;

/// <summary>
/// Abstraction de l'horloge pour faciliter les tests et garantir une time zone cohérente (Abidjan).
/// </summary>
public interface IClock
{
    /// <summary>Date et heure actuelles en time zone Abidjan (UTC+0).</summary>
    DateTime Now { get; }

    /// <summary>Date du jour en time zone Abidjan, sans heure.</summary>
    DateTime Today { get; }

    /// <summary>Convertit une DateTime UTC vers la time zone Abidjan.</summary>
    DateTime ToLocal(DateTime utc);
}
