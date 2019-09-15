using System;
using System.Web.Script.Serialization;
using PageNotFoundManager.Models;
using Umbraco.Core.Cache;

namespace PageNotFoundManager.CacheRefreshers
{
    public class PageNotFoundManagerCacheRefresher : PayloadCacheRefresherBase<PageNotFoundManagerCacheRefresher, PageNotFound>
    {
        public const string Id = "fb97db9a-e67d-4b38-a320-58ecc1e326a7";
        private readonly IPageNotFoundManagerConfig config;

        public PageNotFoundManagerCacheRefresher(AppCaches appCaches, IPageNotFoundManagerConfig config) : base(appCaches)
        {
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public override string Name
        {
            get { return "PageNotFoundManager Cache Refresher"; }
        }

        protected override PageNotFoundManagerCacheRefresher This => this;

        public override Guid RefresherUniqueId => new Guid(Id);

        
        protected override PageNotFound[] Deserialize(string json)
        {
           return new JavaScriptSerializer().Deserialize<PageNotFound[]>(json);
        }

        public override void Refresh(PageNotFound[] payloads)
        {
            foreach(var payload in payloads)
                config.SetNotFoundPage(payload.ParentId, payload.NotFoundPageId, false);
            config.RefreshCache();
        }

    }
}