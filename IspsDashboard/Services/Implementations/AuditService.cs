using IspsDashboard.Data;
using IspsDashboard.Models.Entities;
using IspsDashboard.Services.Interfaces;

namespace IspsDashboard.Services.Implementations;

public class AuditService : IAuditService
{
    private readonly ApplicationDbContext _db;
    private readonly IHttpContextAccessor _httpContext;

    public AuditService(ApplicationDbContext db, IHttpContextAccessor httpContext)
    {
        _db = db;
        _httpContext = httpContext;
    }

    public async Task LogAsync(string action, string detail, string? userId = null, string? userName = null)
    {
        var user = _httpContext.HttpContext?.User;
        _db.AuditLogs.Add(new AuditLog
        {
            UserId = userId ?? user?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            UserName = userName ?? user?.Identity?.Name ?? "system",
            Action = action,
            Detail = detail,
            Timestamp = DateTime.UtcNow
        });
        await _db.SaveChangesAsync();
    }
}
