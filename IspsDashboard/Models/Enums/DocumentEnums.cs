namespace IspsDashboard.Models.Enums;

public enum DocumentCategory
{
    Pfsp = 0,             // Plan de Sûreté de l'Installation Portuaire
    Procedure = 1,
    Consigne = 2,
    Rapport = 3,
    Certificat = 4,
    Formulaire = 5,
    Autre = 99
}

public enum DocumentStatus
{
    Brouillon = 0,
    EnVigueur = 1,
    Archive = 2
}
