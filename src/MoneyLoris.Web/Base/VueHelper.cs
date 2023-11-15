using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MoneyLoris.Web.Base;

[Obsolete]
public static class VueHelper
{
    private const string ScriptsKey = "VueComponentScripts";
    private const string StylesKey = "VueComponentStyles";
    private const string TemplatesKey = "VueComponentTemplates";


    public static IDisposable VueScript(this IHtmlHelper helper)
    {
        return new ScriptBlock(helper.ViewContext);
    }

    public static HtmlString RenderVueScripts(this IHtmlHelper helper)
    {
        return new HtmlString(string.Join(Environment.NewLine, GetPageScriptsList(helper.ViewContext.HttpContext, ScriptsKey)));
    }


    public static IDisposable VueStyle(this IHtmlHelper helper)
    {
        return new StyleBlock(helper.ViewContext);
    }

    public static HtmlString RenderVueStyles(this IHtmlHelper helper)
    {
        return new HtmlString(string.Join(Environment.NewLine, GetPageScriptsList(helper.ViewContext.HttpContext, StylesKey)));
    }


    public static IDisposable VueTemplate(this IHtmlHelper helper)
    {
        return new TemplateBlock(helper.ViewContext);
    }

    public static HtmlString RenderVueTemplates(this IHtmlHelper helper)
    {
        return new HtmlString(string.Join(Environment.NewLine, GetPageScriptsList(helper.ViewContext.HttpContext, TemplatesKey)));
    }



    private static List<string> GetPageScriptsList(HttpContext httpContext, string key)
    {
        var pageScripts = (List<string>)httpContext.Items[key]!;
        if (pageScripts == null)
        {
            pageScripts = new List<string>();
            httpContext.Items[key] = pageScripts;
        }
        return pageScripts;
    }

    private class ScriptBlock : IDisposable
    {
        private readonly TextWriter _originalWriter;
        private readonly StringWriter _scriptsWriter;
        private readonly ViewContext _viewContext;

        public ScriptBlock(ViewContext viewContext)
        {
            _viewContext = viewContext;
            _originalWriter = _viewContext.Writer;
            _viewContext.Writer = _scriptsWriter = new StringWriter();
        }

        public void Dispose()
        {
            _viewContext.Writer = _originalWriter;
            var pageScripts = GetPageScriptsList(_viewContext.HttpContext, ScriptsKey);
            pageScripts.Add(_scriptsWriter.ToString());
        }
    }

    private class StyleBlock : IDisposable
    {
        private readonly TextWriter _originalWriter;
        private readonly StringWriter _stylesWriter;
        private readonly ViewContext _viewContext;

        public StyleBlock(ViewContext viewContext)
        {
            _viewContext = viewContext;
            _originalWriter = _viewContext.Writer;
            _viewContext.Writer = _stylesWriter = new StringWriter();
        }

        public void Dispose()
        {
            _viewContext.Writer = _originalWriter;
            var pageScripts = GetPageScriptsList(_viewContext.HttpContext, StylesKey);
            pageScripts.Add(_stylesWriter.ToString());
        }
    }

    private class TemplateBlock : IDisposable
    {
        private readonly TextWriter _originalWriter;
        private readonly StringWriter _templatesWriter;
        private readonly ViewContext _viewContext;

        public TemplateBlock(ViewContext viewContext)
        {
            _viewContext = viewContext;
            _originalWriter = _viewContext.Writer;
            _viewContext.Writer = _templatesWriter = new StringWriter();
        }

        public void Dispose()
        {
            _viewContext.Writer = _originalWriter;
            var pageScripts = GetPageScriptsList(_viewContext.HttpContext, TemplatesKey);
            pageScripts.Add(_templatesWriter.ToString());
        }
    }
}
