using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace PartialViewWithStyleAndScript.Core
{
    public static class ScriptHelper
    {
        static readonly string _UniqueId = "_scripts_" + Guid.NewGuid().ToString();

        public static HtmlBlock BeginScriptBlock(this WebViewPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            var block = new HtmlBlock(page);
            return block;
        }

        public static IHtmlString RenderRegisteredScripts(this WebViewPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            var context = page.Context;
            var registeredScripts = (context.Items[_UniqueId] as IList<string>) ?? new List<string>(0);
            var scriptBlocks = HtmlBlock.GetRegisteredScriptBlocks(context) ?? new List<string>(0);

            var sb = new StringBuilder();
            foreach (var script in registeredScripts)
            {
                sb.AppendLine(script);
            }
            foreach (var script in scriptBlocks)
            {
                sb.AppendLine(script);
            }
            return new HtmlString(sb.ToString());
        }

        public static IHtmlString RegisterScriptFiles(this HtmlHelper htmlHelper, params string[] paths)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");
            if (paths == null)
                throw new ArgumentNullException("paths");

            var resolver = BundleResolver.Current;
            foreach (var path in paths)
            {
                if (string.IsNullOrWhiteSpace(path))
                    throw new ArgumentException("paths");

                string scriptHtml;
                if (resolver.IsBundleVirtualPath(path))
                {
                    scriptHtml = Scripts.Render(path).ToHtmlString();
                    if (string.IsNullOrEmpty(scriptHtml)) // 地址配错了。
                    {
                        scriptHtml = GetScriptHtml("/Bundle-Script-Config-Error/" + path);
                    }
                }
                else
                {
                    scriptHtml = GetScriptHtml(path);
                }
                htmlHelper.RegisterScript(scriptHtml);
            }
            return new HtmlString(string.Empty);
        }

        static string GetScriptHtml(string path)
        {
            string scriptHtml;
            var scriptBuilder = new TagBuilder("style");
            scriptBuilder.Attributes["type"] = "text/javascript";
            scriptBuilder.Attributes["src"] = Scripts.Url(path).ToString();
            scriptHtml = scriptBuilder.ToString(TagRenderMode.Normal);
            return scriptHtml;
        }

        static void RegisterScript(this HtmlHelper htmlHelper, string script)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");
            if (script == null)
                throw new ArgumentNullException("path");

            var ctx = htmlHelper.ViewContext.HttpContext;
            var registeredScripts = ctx.Items[_UniqueId] as IList<string>;
            if (registeredScripts == null)
            {
                registeredScripts = new List<string>();
                ctx.Items[_UniqueId] = registeredScripts;
            }
            if (!registeredScripts.Contains(script))
            {
                registeredScripts.Add(script);
            }
        }
    }
}