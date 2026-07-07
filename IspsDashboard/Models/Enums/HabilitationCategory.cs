namespace IspsDashboard.Models.Enums;

public enum HabilitationCategory
{
    FormationIsps = 0,        // Formation sûreté ISPS (initiale ou continue)
    HabilitationSurete = 1,   // Habilitation sûreté délivrée par l'État
    SecouriseTravail = 2,     // SST / premiers secours
    ManipulationArme = 3,
    PermisConduire = 4,       // Permis spécifique (poids lourd, conteneur)
    HabilitationElectrique = 5,
    EspaceConfine = 6,
    LutteIncendie = 7,
    Autre = 99
}
