using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Implementations;
using Xunit;

namespace IspsDashboard.Tests;

public class DashboardRadarScoresTests
{
    [Fact]
    public void ComputeRadarScores_AllConforme_ShouldReturn100()
    {
        var table = new[]
        {
            new KpiTableRow { Category = "Conformité ISPS", Status = KpiStatus.Conforme },
            new KpiTableRow { Category = "Conformité ISPS", Status = KpiStatus.Conforme },
            new KpiTableRow { Category = "Reporting", Status = KpiStatus.Conforme }
        };
        var gauges = new[] { new Gauge { Label = "Personnel formé ISPS", Value = 100 } };
        var agents = Enumerable.Range(1, 12).Select(i => new Agent { Position = i, IsPresent = true }).ToArray();
        var exercises = new[] { new Exercise { Status = ExerciseStatus.Realise } };

        var scores = DashboardService.ComputeRadarScores(table, gauges, agents, exercises);

        Assert.Equal(100, scores["Formation"]);
        Assert.Equal(100, scores["Effectif"]);
        Assert.Equal(100, scores["Conformité"]);
        Assert.Equal(100, scores["Exercices"]);
        Assert.Equal(100, scores["Reporting"]);
    }

    [Fact]
    public void ComputeRadarScores_OneAgentAbsent_ShouldComputeEffectifProportionally()
    {
        var agents = Enumerable.Range(1, 12).Select(i => new Agent { Position = i, IsPresent = i != 7 }).ToArray();

        var scores = DashboardService.ComputeRadarScores(
            Array.Empty<KpiTableRow>(), Array.Empty<Gauge>(), agents, Array.Empty<Exercise>());

        // 11 / 12 = 91.666... arrondi à 91.7
        Assert.Equal(91.7, scores["Effectif"]);
    }

    [Theory]
    [InlineData(KpiStatus.Conforme, 100)]
    [InlineData(KpiStatus.Attention, 60)]
    [InlineData(KpiStatus.Critique, 20)]
    public void ComputeRadarScores_ConformityByStatus_FollowsScale(KpiStatus status, double expected)
    {
        var table = new[]
        {
            new KpiTableRow { Category = "Conformité ISPS", Status = status }
        };
        var scores = DashboardService.ComputeRadarScores(
            table, Array.Empty<Gauge>(), Array.Empty<Agent>(), Array.Empty<Exercise>());
        Assert.Equal(expected, scores["Conformité"]);
    }

    [Fact]
    public void ComputeRadarScores_NoData_ShouldReturnZeroForEmptySections_AndDefault100ForEmptyKpiTable()
    {
        // Lorsque la table KPI est vide pour une catégorie, le score retourné est 100 (rien à reprocher).
        var scores = DashboardService.ComputeRadarScores(
            Array.Empty<KpiTableRow>(), Array.Empty<Gauge>(), Array.Empty<Agent>(), Array.Empty<Exercise>());

        Assert.Equal(0, scores["Formation"]);     // pas de gauge "formé"
        Assert.Equal(0, scores["Effectif"]);      // pas d'agents
        Assert.Equal(100, scores["Conformité"]);  // catégorie vide → 100
        Assert.Equal(0, scores["Exercices"]);     // pas d'exercices
        Assert.Equal(100, scores["Reporting"]);
    }
}
