using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Routing;
using Umbraco.Web.Trees;

namespace PageNotFoundManager
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentFinderResolver.Current.InsertTypeBefore<ContentFinderByNotFoundHandlers, PageNotFoundContentFinder>();
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
        }

        void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (sender.TreeAlias == "content"
                && sender.Security.CurrentUser.UserType.Alias == "admin")
            {
                var mi = new MenuItem("pageNotFound", "404 page");
                mi.Icon = "document";
                mi.LaunchDialogView("/App_Plugins/PageNotFoundManager/Backoffice/Dialogs/dialog.html", "404 Page");
                e.Menu.Items.Add(mi);
            }
        }
    }
}