using System;
using System.Web.Script.Serialization;
using PageNotFoundManager.Models;
using Umbraco.Core.Cache;

namespace PageNotFoundManager.CacheRefreshers
{
    public class PageNotFoundManagerCacheRefresher : PayloadCacheRefresherBase<PageNotFoundManagerCacheRefresher>
    {
        public const string Id = "fb97db9a-e67d-4b38-a320-58ecc1e326a7";

        protected override PageNotFoundManagerCacheRefresher Instance
        {
            get { return this; }
        }

        public override Guid UniqueIdentifier
        {
            get { return new Guid(Id); }
        }

        public override string Name
        {
            get { return "PageNotFoundManager Cache Refresher"; }
        }

        protected override object Deserialize(string json)
        {
           return new JavaScriptSerializer().Deserialize<PageNotFound>(json);
        }

        public override void Refresh(object payload)
        {
            var jsonPayload = (PageNotFound)payload;
            Config.RefreshCache();
        }
    }
}