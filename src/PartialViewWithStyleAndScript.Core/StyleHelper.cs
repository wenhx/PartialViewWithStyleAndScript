using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;

namespace PartialViewWithStyleAndScript.Core
{
    public static class StyleHelper
    {
        static readonly string _UniqueId = "_scripts_" + Guid.NewGuid().ToString();

        public static IHtmlString RenderRegisteredStyles(this WebViewPage page)
        {
            if (page == null)
                throw new ArgumentNullException("page");

            var context = page.Context;
            var registeredStyles = (context.Items[_UniqueId] as IList<string>) ?? new List<string>(0);

            var sb = new StringBuilder();
            foreach (var style in registeredStyles)
            {
                sb.AppendLine(style);
            }
            return new HtmlString(sb.ToString());
        }

        public static IHtmlString RegisterStyleFiles(this HtmlHelper htmlHelper, params string[] paths)
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

                string styleHtml;
                if (resolver.IsBundleVirtualPath(path))
                {
                    styleHtml = Styles.Render(path).ToHtmlString();
                    if (string.IsNullOrEmpty(styleHtml)) // 地址配错了。
                    {
                        styleHtml = GetStyleHtml("/Bundle-Style-Config-Error/" + path);
                    }
                }
                else
                {
                    styleHtml = GetStyleHtml(path);
                }
                htmlHelper.RegisterStyle(styleHtml);
            }
            return new HtmlString(string.Empty);
        }

        static string GetStyleHtml(string path)
        { //<link href="/Content/site.css" rel="stylesheet"/>
            string html;
            var htmlBuilder = new TagBuilder("link");
            htmlBuilder.Attributes["rel"] = "stylesheet";
            htmlBuilder.Attributes["href"] = Scripts.Url(path).ToString();
            html = htmlBuilder.ToString(TagRenderMode.Normal);
            return html;
        }

        static void RegisterStyle(this HtmlHelper htmlHelper, string style)
        {
            if (htmlHelper == null)
                throw new ArgumentNullException("htmlHelper");
            if (style == null)
                throw new ArgumentNullException("style");

            var ctx = htmlHelper.ViewContext.HttpContext;
            var registeredStyles = ctx.Items[_UniqueId] as IList<string>;
            if (registeredStyles == null)
            {
                registeredStyles = new List<string>();
                ctx.Items[_UniqueId] = registeredStyles;
            }
            if (!registeredStyles.Contains(style))
            {
                registeredStyles.Add(style);
            }
        }
    }
}
