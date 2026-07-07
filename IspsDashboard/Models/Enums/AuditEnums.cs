namespace IspsDashboard.Models.Enums;

public enum AuditType
{
    Interne = 0,
    Externe = 1,
    Inopine = 2
}

public enum AuditStatus
{
    Planifie = 0,
    EnCours = 1,
    Cloture = 2
}

public enum FindingResult
{
    Conforme = 0,
    NonConforme = 1,
    Observation = 2,
    NonApplicable = 3
}
