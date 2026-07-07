namespace IspsDashboard.Models.Enums;

public enum VehicleType
{
    Camion = 0,
    Conteneur = 1,
    VehiculeLeger = 2,
    Bus = 3,
    EnginManutention = 4,
    Autre = 99
}

public enum AccessDirection
{
    Entree = 0,
    Sortie = 1
}

public enum AccessControlResult
{
    Autorise = 0,
    Refuse = 1,
    EnAttente = 2
}
