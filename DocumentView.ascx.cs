using System;
using System.Web;

using DotNetNuke.Entities.Modules;
using DotNetNuke.Framework;
using DotNetNuke.Framework.JavaScriptLibraries;
using DotNetNuke.Web.Client;
using DotNetNuke.Web.Client.ClientResourceManagement;

namespace TidyModules.DocumentExplorer
{
    public partial class DocumentView : PortalModuleBase
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            DocumentSettings settings = new DocumentSettings(ModuleConfiguration);

            string scriptsPath = VirtualPathUtility.Combine(ControlPath, "Scripts/");
            string explorerJS = VirtualPathUtility.Combine(scriptsPath, "explorer.min.js");
            //string explorerJS = VirtualPathUtility.Combine(scriptsPath, "explorer.js");

            if (settings.Theme != "(none)")
            {
                string themesPath = VirtualPathUtility.Combine(scriptsPath, "themes/");
                string theme = VirtualPathUtility.Combine(themesPath, VirtualPathUtility.AppendTrailingSlash(settings.Theme));
                string themeCSS = VirtualPathUtility.Combine(theme, "theme.css");

                ClientResourceManager.RegisterStyleSheet(Page, themeCSS);
            }

            if (!string.IsNullOrEmpty(settings.JqueryUICSS))
                ClientResourceManager.RegisterStyleSheet(Page, settings.JqueryUICSS);

            if (!string.IsNullOrEmpty(settings.FontAwesomeCSS))
                ClientResourceManager.RegisterStyleSheet(Page, settings.FontAwesomeCSS);

            if (!string.IsNullOrEmpty(settings.PrimeUICSS))
                ClientResourceManager.RegisterStyleSheet(Page, settings.PrimeUICSS);

            JavaScript.RequestRegistration(CommonJs.jQueryUI);

            if (!string.IsNullOrEmpty(settings.PrimeUIJS))
                ClientResourceManager.RegisterScript(Page, settings.PrimeUIJS, FileOrder.Js.DefaultPriority);

            ClientResourceManager.RegisterScript(Page, explorerJS, FileOrder.Js.DefaultPriority);

            ServicesFramework.Instance.RequestAjaxAntiForgerySupport();
        }
    }
}