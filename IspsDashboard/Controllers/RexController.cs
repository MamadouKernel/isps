using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Controllers;

[Authorize]
public class RexController : Controller
{
    private readonly IRexService _rex;
    private readonly ApplicationDbContext _db;
    private readonly ISoftDeleteService _softDelete;

    public RexController(IRexService rex, ApplicationDbContext db, ISoftDeleteService softDelete)
    {
        _rex = rex;
        _db = db;
        _softDelete = softDelete;
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Delete(int id)
    {
        await _softDelete.DeleteAsync<ExerciseRex>(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value);
        TempData["Success"] = "REX supprimé (corbeille).";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.Recent = await _rex.GetRecentAsync(20);
        ViewBag.Exercises = await _db.Exercises.AsNoTracking()
            .OrderByDescending(e => e.PlannedDate)
            .Take(50)
            .ToListAsync();
        return View();
    }

    [Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(int exerciseId)
    {
        var exercise = await _db.Exercises.AsNoTracking().FirstOrDefaultAsync(e => e.Id == exerciseId);
        if (exercise is null) return NotFound();
        var existing = await _rex.GetForExerciseAsync(exerciseId)
                       ?? new ExerciseRex { ExerciseId = exerciseId };
        ViewBag.Exercise = exercise;
        return View(existing);
    }

    [HttpPost, ValidateAntiForgeryToken, Authorize(Policy = "RequireEditor")]
    public async Task<IActionResult> Edit(ExerciseRex input)
    {
        if (!ModelState.IsValid)
        {
            TempData["Error"] = "REX non enregistré : un des champs dépasse la longueur maximale autorisée (4000 caractères).";
            return RedirectToAction(nameof(Edit), new { exerciseId = input.ExerciseId });
        }

        input.WrittenBy = string.IsNullOrEmpty(input.WrittenBy) ? (User.Identity?.Name ?? "system") : input.WrittenBy;
        await _rex.CreateOrUpdateAsync(input);
        TempData["Success"] = "REX enregistré.";
        return RedirectToAction(nameof(Index));
    }
}
