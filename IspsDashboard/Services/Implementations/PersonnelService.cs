using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IspsDashboard.Services.Implementations;

public sealed class PersonnelService : IPersonnelService
{
    private readonly ApplicationDbContext _db;
    private readonly IAuditService _audit;

    public PersonnelService(ApplicationDbContext db, IAuditService audit)
    {
        _db = db;
        _audit = audit;
    }

    public async Task<IReadOnlyList<Agent>> GetAllAsync()
        => await _db.Agents.AsNoTracking()
            .Include(a => a.Habilitations)
            .OrderBy(a => a.Position)
            .ToListAsync();

    public Task<Agent?> GetByIdAsync(int id)
        => _db.Agents.Include(a => a.Habilitations).FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Agent> CreateAsync(Agent input)
    {
        var agent = new Agent
        {
            Name = input.Name.Trim(),
            IsPresent = input.IsPresent,
            Position = input.Position,
            BadgeNumber = input.BadgeNumber?.Trim() ?? string.Empty,
            Phone = input.Phone?.Trim() ?? string.Empty,
            Email = input.Email?.Trim() ?? string.Empty,
            HiredAt = input.HiredAt,
            Role = input.Role?.Trim() ?? "Agent de sûreté",
            Notes = input.Notes?.Trim() ?? string.Empty
        };
        _db.Agents.Add(agent);
        await _db.SaveChangesAsync();
        await _audit.LogAsync("CreatePersonnel", $"Agent #{agent.Position} — {agent.Name}");
        return agent;
    }

    public async Task<bool> UpdateAsync(Agent input, byte[] rowVersion)
    {
        var existing = await _db.Agents.FirstOrDefaultAsync(a => a.Id == input.Id);
        if (existing is null) return false;
        _db.Entry(existing).Property(nameof(Agent.RowVersion)).OriginalValue = rowVersion;

        existing.Name = input.Name.Trim();
        existing.IsPresent = input.IsPresent;
        existing.BadgeNumber = input.BadgeNumber?.Trim() ?? string.Empty;
        existing.Phone = input.Phone?.Trim() ?? string.Empty;
        existing.Email = input.Email?.Trim() ?? string.Empty;
        existing.HiredAt = input.HiredAt;
        existing.Role = input.Role?.Trim() ?? "Agent de sûreté";
        existing.Notes = input.Notes?.Trim() ?? string.Empty;
        existing.PhotoPath = input.PhotoPath ?? existing.PhotoPath;

        await _db.SaveChangesAsync();
        await _audit.LogAsync("UpdatePersonnel", $"Agent #{existing.Position} — {existing.Name}");
        return true;
    }

    public Task<int> CountPresentAsync() => _db.Agents.CountAsync(a => a.IsPresent);
}
