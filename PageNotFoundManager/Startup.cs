using PageNotFoundManager.Models;
using Umbraco.Core;
using Umbraco.Core.Persistence;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Routing;
using Umbraco.Web.Trees;

namespace PageNotFoundManager
{
    public class Startup : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentLastChanceFinderResolver.Current.SetFinder(new PageNotFoundContentFinder());
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            DatabaseContext ctx = ApplicationContext.Current.DatabaseContext;
            DatabaseSchemaHelper dbSchema = new DatabaseSchemaHelper(ctx.Database, ApplicationContext.Current.ProfilingLogger.Logger, ctx.SqlSyntax);

            // use for testing dbSchema.DropTable<PageNotFound>();
            if (!dbSchema.TableExist(PageNotFound.TableName))
            {
                dbSchema.CreateTable<PageNotFound>(false);
                var migrator = new Migrator(ctx.Database);
                migrator.Start();
            }
        }

        void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (Config.RunningOnCloud())
            {
                if (sender.TreeAlias == "content"
                   && e.NodeId != "-1" && e.NodeId != "-20")
                {
                    var mi = new MenuItem("pageNotFound", "404 page - Not Licensed For Cloud");
                    mi.Icon = "dollar-bag";
                    mi.LaunchDialogView("/App_Plugins/PageNotFoundManager/Backoffice/Dialogs/cloud.html", "404 Page");
                    mi.SeperatorBefore = true;
                    e.Menu.Items.Insert(e.Menu.Items.Count - 1, mi);
                }
            }
            else
            {
                if (sender.TreeAlias == "content"
                    && sender.Security.CurrentUser.UserType.Alias == "admin"
                    && e.NodeId != "-1" && e.NodeId != "-20")
                {
                    var mi = new MenuItem("pageNotFound", "404 page");
                    mi.Icon = "document";
                    mi.LaunchDialogView("/App_Plugins/PageNotFoundManager/Backoffice/Dialogs/dialog.html", "404 Page");
                    mi.SeperatorBefore = true;
                    e.Menu.Items.Insert(e.Menu.Items.Count - 1, mi);
                }
            }
        }
    }
}