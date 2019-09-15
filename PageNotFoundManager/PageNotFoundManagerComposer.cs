using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;

namespace PageNotFoundManager
{
    [RuntimeLevel(MinLevel = RuntimeLevel.Run)]
    public class PageNotFoundManagerComposer : IUserComposer
    {
        public void Compose(Composition composition)
        {
            composition.Register<IPageNotFoundManagerConfig, Config>(Lifetime.Singleton);
            composition.Components()
                .Append<PageNotFoundManagerComponent>();
            composition.SetContentLastChanceFinder<PageNotFoundContentFinder>();
            TreeControllerBase.MenuRendering += TreeControllerBase_MenuRendering;
        }

        void TreeControllerBase_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (sender.TreeAlias == "content"
                && sender.Security.CurrentUser.Groups.Any(g=>g.Alias.InvariantEquals("admin"))
                && e.NodeId != "-1" && e.NodeId != "-20")
            {
                var mi = new MenuItem("pageNotFound", "404 page")
                {
                    Icon = "document"
                };
                mi.LaunchDialogView("/App_Plugins/PageNotFoundManager/Backoffice/Dialogs/dialog.html", "404 Page");
                mi.SeparatorBefore = true;
                e.Menu.Items.Insert(e.Menu.Items.Count - 1, mi);
            }
        }
    }
}