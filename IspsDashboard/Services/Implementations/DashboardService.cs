using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace IspsDashboard.Services.Implementations;

public class DashboardService : IDashboardService
{
    private const string CacheKey = "dashboard:vm";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(5);

    private readonly ApplicationDbContext _db;
    private readonly IExerciseService _exerciseService;
    private readonly IAccessPassService _accessPassService;
    private readonly IMemoryCache _cache;

    public DashboardService(
        ApplicationDbContext db,
        IExerciseService exerciseService,
        IAccessPassService accessPassService,
        IMemoryCache cache)
    {
        _db = db;
        _exerciseService = exerciseService;
        _accessPassService = accessPassService;
        _cache = cache;
    }

    public Task<DashboardViewModel> BuildDashboardAsync()
        => _cache.GetOrCreateAsync(CacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = CacheTtl;
            return await LoadDashboardAsync();
        })!;

    public void InvalidateCache() => _cache.Remove(CacheKey);

    private async Task<DashboardViewModel> LoadDashboardAsync()
    {
        var settings = await _db.DashboardSettings.AsNoTracking().FirstOrDefaultAsync() ?? new DashboardSettings();
        var cards = await _db.KpiCards.AsNoTracking().OrderBy(k => k.DisplayOrder).ToListAsync();
        var gauges = await _db.Gauges.AsNoTracking().OrderBy(g => g.DisplayOrder).ToListAsync();
        var bars = await _db.ProgressBars.AsNoTracking().OrderBy(p => p.DisplayOrder).ToListAsync();
        var table = await _db.KpiTableRows.AsNoTracking().OrderBy(k => k.DisplayOrder).ToListAsync();
        var agents = await _db.Agents.AsNoTracking().OrderBy(a => a.Position).ToListAsync();
        var exercises = await _db.Exercises.AsNoTracking().OrderBy(e => e.Type).ThenBy(e => e.DisplayOrder).ToListAsync();

        var trainings = exercises.Where(e => e.Type == ExerciseType.Training).Select(_exerciseService.Wrap).ToList();
        var grand = exercises.FirstOrDefault(e => e.Type == ExerciseType.GrandNature);
        var crisis = exercises.Where(e => e.Type == ExerciseType.Crisis).Select(_exerciseService.Wrap).ToList();

        var expiringPasses = await _accessPassService.GetExpiringSoonAsync(30);
        var activePasses = await _accessPassService.CountActiveAsync();

        return new DashboardViewModel
        {
            Settings = settings,
            KpiCards = cards,
            Gauges = gauges,
            ProgressBars = bars,
            KpiTable = table,
            Agents = agents,
            Trainings = trainings,
            GrandNature = grand is null ? null : _exerciseService.Wrap(grand),
            CrisisExercises = crisis,
            ExpiringAccessPasses = expiringPasses.Select(_accessPassService.Wrap).ToList(),
            ActiveAccessPasses = activePasses,
            RadarScores = ComputeRadarScores(table, gauges, agents, exercises)
        };
    }

    public async Task<AdminViewModel> BuildAdminAsync()
    {
        var exercises = await _db.Exercises.OrderBy(e => e.Type).ThenBy(e => e.DisplayOrder).ToListAsync();

        return new AdminViewModel
        {
            Settings = await _db.DashboardSettings.FirstOrDefaultAsync() ?? new DashboardSettings(),
            KpiCards = await _db.KpiCards.OrderBy(k => k.DisplayOrder).ToListAsync(),
            Gauges = await _db.Gauges.OrderBy(g => g.DisplayOrder).ToListAsync(),
            ProgressBars = await _db.ProgressBars.OrderBy(p => p.DisplayOrder).ToListAsync(),
            Agents = await _db.Agents.OrderBy(a => a.Position).ToListAsync(),
            Trainings = exercises.Where(e => e.Type == ExerciseType.Training).ToList(),
            GrandNature = exercises.FirstOrDefault(e => e.Type == ExerciseType.GrandNature),
            CrisisExercises = exercises.Where(e => e.Type == ExerciseType.Crisis).ToList(),
            KpiTable = await _db.KpiTableRows.OrderBy(k => k.DisplayOrder).ToListAsync()
        };
    }

    internal static Dictionary<string, double> ComputeRadarScores(
        IReadOnlyList<KpiTableRow> table,
        IReadOnlyList<Gauge> gauges,
        IReadOnlyList<Agent> agents,
        IReadOnlyList<Exercise> exercises)
    {
        double ScoreByStatus(IEnumerable<KpiTableRow> rows)
        {
            var list = rows.ToList();
            if (list.Count == 0) return 100;
            var sum = list.Sum(r => r.Status switch
            {
                KpiStatus.Conforme => 100,
                KpiStatus.Attention => 60,
                _ => 20
            });
            return Math.Round((double)sum / list.Count, 1);
        }

        var trainingScore = gauges.FirstOrDefault(g => g.Label.Contains("formé"))?.Value ?? 0;
        var staffScore = agents.Count == 0 ? 0 : Math.Round((double)agents.Count(a => a.IsPresent) / agents.Count * 100, 1);
        var conformityScore = ScoreByStatus(table.Where(r => r.Category == "Conformité ISPS"));
        var exercisesScore = exercises.Count == 0 ? 0 :
            Math.Round((double)exercises.Count(e => e.Status == ExerciseStatus.Realise) / exercises.Count * 100, 1);
        var reportingScore = ScoreByStatus(table.Where(r => r.Category == "Reporting"));

        return new Dictionary<string, double>
        {
            ["Formation"] = trainingScore,
            ["Effectif"] = staffScore,
            ["Conformité"] = conformityScore,
            ["Exercices"] = exercisesScore,
            ["Reporting"] = reportingScore
        };
    }
}
