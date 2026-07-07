namespace IspsDashboard.Models.Enums;

public enum ContactType
{
    AutoritePortuaire = 0,    // PAA (Port Autonome d'Abidjan)
    GendarmerieMaritime = 1,
    Douanes = 2,
    Police = 3,
    Pompiers = 4,
    Samu = 5,
    PrefetMaritime = 6,
    GardesCotes = 7,
    Compagnie = 8,
    Prestataire = 9,
    Autre = 99
}

public enum InteractionDirection
{
    Entrante = 0,
    Sortante = 1
}

public enum InteractionChannel
{
    Telephone = 0,
    Email = 1,
    Radio = 2,
    PresentielReunion = 3,
    Courrier = 4,
    Sms = 5,
    Autre = 99
}
