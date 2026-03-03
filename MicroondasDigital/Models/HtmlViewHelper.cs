using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace MicroondasDigital.Models;

public static class HtmlViewHelper
{
    public static string RenderPartialViewToString(Controller controller, string viewName, object model)
    {
        controller.ViewData.Model = model;
        using var writer = new StringWriter();
        var viewResult = controller.HttpContext.RequestServices
            .GetService<ICompositeViewEngine>()?
            .FindView(controller.ControllerContext, viewName, false);

        if (viewResult?.View == null)
            throw new ArgumentNullException($"View '{viewName}' não encontrada.");

        var viewContext = new Microsoft.AspNetCore.Mvc.Rendering.ViewContext(
            controller.ControllerContext,
            viewResult.View,
            controller.ViewData,
            controller.TempData,
            writer,
            new Microsoft.AspNetCore.Mvc.ViewFeatures.HtmlHelperOptions()
        );
        viewResult.View.RenderAsync(viewContext).GetAwaiter().GetResult();
        return writer.GetStringBuilder().ToString();
    }
    
}