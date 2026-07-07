namespace IspsDashboard.Models.Enums;

public enum NonConformityStatus
{
    Identifiee = 0,
    EnTraitement = 1,
    Levee = 2,
    Rejetee = 3
}

public enum NonConformitySource
{
    AuditInterne = 0,
    AuditExterne = 1,
    Inspection = 2,
    Incident = 3,
    Signalement = 4,
    AutreSource = 99
}
