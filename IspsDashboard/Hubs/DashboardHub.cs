using Microsoft.AspNetCore.SignalR;

namespace IspsDashboard.Hubs;

public class DashboardHub : Hub
{
    public const string DashboardUpdatedMethod = "DashboardUpdated";
}
