using IspsDashboard.Models.Entities;
using IspsDashboard.Models.Enums;
using IspsDashboard.Services.Implementations;
using Xunit;

namespace IspsDashboard.Tests;

public class ExerciseServiceTests
{
    private readonly ExerciseService _service;

    public ExerciseServiceTests()
    {
        var db = TestHelpers.CreateInMemoryDb();
        _service = new ExerciseService(db);
    }

    [Theory]
    [InlineData(45, ExerciseColor.Vert)]     // > 30 jours
    [InlineData(31, ExerciseColor.Vert)]
    [InlineData(30, ExerciseColor.Ambre)]    // dans 30 jours
    [InlineData(15, ExerciseColor.Ambre)]
    [InlineData(7, ExerciseColor.Ambre)]
    [InlineData(6, ExerciseColor.Rouge)]     // < 7 jours
    [InlineData(0, ExerciseColor.Rouge)]
    [InlineData(-3, ExerciseColor.Rouge)]    // dépassé
    public void ComputeColor_ShouldReturnExpectedColor_ForPlannedExercise(int daysFromToday, ExerciseColor expected)
    {
        var exercise = new Exercise
        {
            PlannedDate = DateTime.UtcNow.Date.AddDays(daysFromToday),
            Status = ExerciseStatus.Planifie
        };

        var color = _service.ComputeColor(exercise);

        Assert.Equal(expected, color);
    }

    [Fact]
    public void ComputeColor_ShouldReturnBleu_WhenExerciseIsRealise()
    {
        var exercise = new Exercise
        {
            PlannedDate = DateTime.UtcNow.Date.AddDays(-5),
            Status = ExerciseStatus.Realise
        };

        Assert.Equal(ExerciseColor.Bleu, _service.ComputeColor(exercise));
    }

    [Theory]
    [InlineData(10, "J-10")]
    [InlineData(1, "J-1")]
    [InlineData(0, "Aujourd'hui")]
    [InlineData(-1, "En retard de 1 j")]
    [InlineData(-7, "En retard de 7 j")]
    public void ComputeCountdownLabel_ShouldFormatCorrectly(int daysFromToday, string expected)
    {
        var exercise = new Exercise
        {
            PlannedDate = DateTime.UtcNow.Date.AddDays(daysFromToday),
            Status = ExerciseStatus.Planifie
        };

        Assert.Equal(expected, _service.ComputeCountdownLabel(exercise));
    }

    [Fact]
    public void ComputeCountdownLabel_ShouldReturnRealiseMark_WhenStatusIsRealise()
    {
        var exercise = new Exercise { PlannedDate = DateTime.UtcNow.Date, Status = ExerciseStatus.Realise };
        Assert.Equal("✓ Réalisé", _service.ComputeCountdownLabel(exercise));
    }

    [Fact]
    public void Wrap_ShouldPopulateAllFields()
    {
        var exercise = new Exercise
        {
            Id = 42,
            Name = "Test",
            PlannedDate = DateTime.UtcNow.Date.AddDays(20),
            Status = ExerciseStatus.Planifie
        };

        var wrapped = _service.Wrap(exercise);

        Assert.Same(exercise, wrapped.Exercise);
        Assert.Equal(20, wrapped.DaysRemaining);
        Assert.Equal(ExerciseColor.Ambre, wrapped.Color);
        Assert.Equal("J-20", wrapped.CountdownLabel);
    }
}
