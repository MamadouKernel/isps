namespace IspsDashboard.Models.Enums;

public enum CameraStatus
{
    Operationnelle = 0,
    Defaillante = 1,
    EnMaintenance = 2,
    HorsService = 3
}

public enum CameraType
{
    Fixe = 0,
    PTZ = 1,            // Pan-Tilt-Zoom
    Dome = 2,
    Thermique = 3,
    LecteurPlaques = 4,
    Autre = 99
}
