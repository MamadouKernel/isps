namespace IspsDashboard.Services.Interfaces;

/// <summary>
/// Rend une vue Razor (.cshtml) en string HTML, utilisé pour les templates d'emails.
/// </summary>
public interface IRazorTemplateRenderer
{
    Task<string> RenderAsync<TModel>(string viewName, TModel model);
}
