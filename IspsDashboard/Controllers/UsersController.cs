using IspsDashboard.Data.Seed;
using IspsDashboard.Models.Entities;
using IspsDashboard.Models.ViewModels;
using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Controllers;

[Authorize(Policy = "RequireAdmin")]
public class UsersController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IAuditService _audit;

    public UsersController(UserManager<ApplicationUser> userManager, IAuditService audit)
    {
        _userManager = userManager;
        _audit = audit;
    }

    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var items = new List<UserListItem>();
        foreach (var u in users)
        {
            var roles = await _userManager.GetRolesAsync(u);
            items.Add(new UserListItem
            {
                Id = u.Id,
                Email = u.Email ?? string.Empty,
                FullName = u.FullName,
                Role = roles.FirstOrDefault() ?? string.Empty,
                CreatedAt = u.CreatedAt
            });
        }
        return View(items);
    }

    [HttpGet] public IActionResult Create() => View(new CreateUserViewModel());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            EmailConfirmed = true,
            FullName = model.FullName
        };
        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var e in result.Errors) ModelState.AddModelError(string.Empty, e.Description);
            return View(model);
        }

        var role = model.Role is DataSeeder.AdminRole or DataSeeder.EditorRole or DataSeeder.ReaderRole
            ? model.Role : DataSeeder.ReaderRole;
        await _userManager.AddToRoleAsync(user, role);
        await _audit.LogAsync("CreateUser", $"{model.Email} ({role})");
        TempData["Success"] = $"Utilisateur {model.Email} créé.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        return View(new UserListItem
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = roles.FirstOrDefault() ?? string.Empty,
            CreatedAt = user.CreatedAt
        });
    }

    public async Task<IActionResult> Edit(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        var roles = await _userManager.GetRolesAsync(user);
        return View(new EditUserViewModel
        {
            Id = user.Id,
            Email = user.Email ?? string.Empty,
            FullName = user.FullName,
            Role = roles.FirstOrDefault() ?? DataSeeder.ReaderRole,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserViewModel model)
    {
        if (id != model.Id) return BadRequest();
        ModelState.Remove(nameof(EditUserViewModel.Email));
        if (!ModelState.IsValid) return View(model);

        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        user.FullName = model.FullName.Trim();
        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        var role = model.Role is DataSeeder.AdminRole or DataSeeder.EditorRole or DataSeeder.ReaderRole
            ? model.Role : DataSeeder.ReaderRole;
        if (!currentRoles.Contains(role))
        {
            if (currentRoles.Count > 0) await _userManager.RemoveFromRolesAsync(user, currentRoles);
            await _userManager.AddToRoleAsync(user, role);
        }

        if (!string.IsNullOrEmpty(model.NewPassword))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var resetResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
            if (!resetResult.Succeeded)
            {
                foreach (var e in resetResult.Errors) ModelState.AddModelError(string.Empty, e.Description);
                model.Id = user.Id;
                model.Email = user.Email ?? string.Empty;
                return View(model);
            }
        }

        await _audit.LogAsync("UpdateUser", $"{user.Email} ({role})");
        TempData["Success"] = "Utilisateur mis à jour.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return NotFound();
        if (user.UserName == User.Identity?.Name)
        {
            TempData["Error"] = "Impossible de supprimer son propre compte.";
            return RedirectToAction(nameof(Index));
        }
        await _userManager.DeleteAsync(user);
        await _audit.LogAsync("DeleteUser", $"{user.Email}");
        TempData["Success"] = "Utilisateur supprimé.";
        return RedirectToAction(nameof(Index));
    }
}
