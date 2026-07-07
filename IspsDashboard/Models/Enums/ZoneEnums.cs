namespace IspsDashboard.Models.Enums;

/// <summary>Niveau de restriction d'accès d'une zone (croissant).</summary>
public enum ZoneAccessLevel
{
    Public = 0,         // accès libre (hors zone sûreté)
    Controle = 1,       // contrôlé (badge requis)
    Restreint = 2,      // restreint (autorisation nominative)
    Sensible = 3        // hautement sensible (escorte + habilitation)
}

/// <summary>État opérationnel d'une zone restreinte.</summary>
public enum ZoneStatus
{
    Active = 0,
    Verrouillee = 1,    // verrouillée (incident, niveau MARSEC élevé)
    Maintenance = 2
}
