using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using Xunit;

namespace IspsDashboard.Tests;

public class AuditScoreTests
{
    private static SecurityAudit WithFindings(params FindingResult[] results)
        => new()
        {
            Findings = results.Select((r, i) => new AuditFinding { ItemNumber = i + 1, Result = r }).ToList()
        };

    [Fact]
    public void ConformityScore_AllConforme_Returns100()
    {
        var audit = WithFindings(FindingResult.Conforme, FindingResult.Conforme, FindingResult.Conforme);
        Assert.Equal(100, audit.ConformityScore());
    }

    [Fact]
    public void ConformityScore_HalfNonConforme_Returns50()
    {
        var audit = WithFindings(FindingResult.Conforme, FindingResult.NonConforme);
        Assert.Equal(50, audit.ConformityScore());
    }

    [Fact]
    public void ConformityScore_IgnoresNonApplicable()
    {
        // 2 conformes + 1 N/A → 100% (le N/A est exclu du dénominateur)
        var audit = WithFindings(FindingResult.Conforme, FindingResult.Conforme, FindingResult.NonApplicable);
        Assert.Equal(100, audit.ConformityScore());
    }

    [Fact]
    public void ConformityScore_ObservationCountsAsNonConforme()
    {
        // Observation n'est pas "Conforme" → 1/2 = 50%
        var audit = WithFindings(FindingResult.Conforme, FindingResult.Observation);
        Assert.Equal(50, audit.ConformityScore());
    }

    [Fact]
    public void ConformityScore_NoFindings_Returns100()
    {
        Assert.Equal(100, new SecurityAudit().ConformityScore());
    }

    [Fact]
    public void ConformityScore_AllNonApplicable_Returns100()
    {
        var audit = WithFindings(FindingResult.NonApplicable, FindingResult.NonApplicable);
        Assert.Equal(100, audit.ConformityScore());
    }
}
