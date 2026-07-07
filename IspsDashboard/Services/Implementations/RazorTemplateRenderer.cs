using IspsDashboard.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

namespace IspsDashboard.Services.Implementations;

public sealed class RazorTemplateRenderer : IRazorTemplateRenderer
{
    private readonly IRazorViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;
    private readonly IServiceProvider _serviceProvider;

    public RazorTemplateRenderer(
        IRazorViewEngine viewEngine,
        ITempDataProvider tempDataProvider,
        IServiceProvider serviceProvider)
    {
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
        _serviceProvider = serviceProvider;
    }

    public async Task<string> RenderAsync<TModel>(string viewName, TModel model)
    {
        var actionContext = new ActionContext(
            new DefaultHttpContext { RequestServices = _serviceProvider },
            new RouteData(),
            new ActionDescriptor());

        var viewResult = _viewEngine.GetView(null, viewName, false);
        if (!viewResult.Success)
            viewResult = _viewEngine.FindView(actionContext, viewName, false);

        if (!viewResult.Success)
            throw new InvalidOperationException($"Vue Razor introuvable : {viewName}");

        await using var writer = new StringWriter();
        var viewDictionary = new ViewDataDictionary<TModel>(
            new EmptyModelMetadataProvider(),
            new ModelStateDictionary()) { Model = model };

        var viewContext = new ViewContext(
            actionContext,
            viewResult.View,
            viewDictionary,
            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
            writer,
            new HtmlHelperOptions());

        await viewResult.View.RenderAsync(viewContext);
        return writer.ToString();
    }
}
