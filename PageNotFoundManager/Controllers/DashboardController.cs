using System;
using System.Web.Http;
using PageNotFoundManager.Extensions;
using PageNotFoundManager.Models;
using Umbraco.Web.Cache;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace PageNotFoundManager.Controllers
{
    [PluginController("PageNotFoundManager")]
    public class DashboardController : UmbracoAuthorizedJsonController  
    {
        private readonly DistributedCache distributedCache;
        private readonly IPageNotFoundManagerConfig config;

        public DashboardController(DistributedCache distributedCache, IPageNotFoundManagerConfig config)
        {
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }
        public int GetNotFoundPage(int pageId)
        {
            return config.GetNotFoundPage(pageId);
        }
        [HttpPost]
        public void SetNotFoundPage(PageNotFoundRequest pnf)
        {
            config.SetNotFoundPage(pnf.ParentId, pnf.NotFoundPageId, true);
            
            distributedCache.RefreshPageNotFoundConfig(pnf);
        }
    }
}