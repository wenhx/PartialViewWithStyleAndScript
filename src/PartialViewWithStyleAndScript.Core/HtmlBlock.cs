using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace PartialViewWithStyleAndScript.Core
{
    public sealed class HtmlBlock : IDisposable
    {
        static readonly string _UniqueId = "HtmlBlock-" + Guid.NewGuid().ToString();
        TextWriter m_VariableTextWriter = new StringWriter(CultureInfo.CurrentCulture);
        WebViewPage m_CurrentPage;

        public HtmlBlock(WebViewPage page)
        {
            if (page == null)
                throw new ArgumentNullException("context");

            m_CurrentPage = page;
            m_CurrentPage.OutputStack.Push(m_VariableTextWriter);
        }

        public void Dispose()
        {
            var blocks = m_CurrentPage.Context.Items[_UniqueId] as IList<string>;
            if (blocks == null)
            {
                blocks = new List<string>();
                m_CurrentPage.Context.Items[_UniqueId] = blocks;
            }
            blocks.Add(m_VariableTextWriter.ToString());
            m_VariableTextWriter.Dispose();
            m_CurrentPage.OutputStack.Pop();
        }

        internal static IEnumerable<string> GetRegisteredScriptBlocks(HttpContextBase context)
        {
            Debug.Assert(context != null);
            var blocks = context.Items[_UniqueId] as IList<string>;
            return blocks;
        }
    }
}