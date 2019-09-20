using System;
using PageNotFoundManager.CacheRefreshers;
using PageNotFoundManager.Models;
using Umbraco.Web.Cache;

namespace PageNotFoundManager.Extensions
{
  public static class PageNotFoundManagerExtensions
    {
        public static void RefreshPageNotFoundConfig(this DistributedCache dc, PageNotFoundRequest pageNotFound)
        {
            dc.RefreshByPayload(new Guid(PageNotFoundManagerCacheRefresher.Id), new[] { pageNotFound });
        }
    }
}