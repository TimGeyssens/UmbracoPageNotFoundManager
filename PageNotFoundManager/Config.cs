using NPoco;
using PageNotFoundManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Scoping;

namespace PageNotFoundManager
{
    public class Config : IPageNotFoundManagerConfig
    {
        private const string CacheKey = "pageNotFoundManagerConfig";
        private readonly IScopeProvider scopeProvider;

        public Config(IScopeProvider scopeProvider)
        {
            this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
        }

        public int GetNotFoundPage(int parentId)
        {
            var x = ConfiguredPages.FirstOrDefault(p => p.ParentId == parentId);
            return x != null ? x.NotFoundPageId : 0;
        }

        public void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache)
        {
            using (var scope = scopeProvider.CreateScope())
            {
                var db = scope.Database;
                var page = db.Query<PageNotFound>().Where(p => p.ParentId == parentId).FirstOrDefault();
                if (page == null)
                {
                    // create the page
                    db.Insert<PageNotFound>(new PageNotFound { ParentId = parentId, NotFoundPageId = pageNotFoundId });
                }
                else
                {
                    // update the existing page
                    page.NotFoundPageId = pageNotFoundId;
                    db.Update(PageNotFound.TableName, "ParentId", page);
                }
                scope.Complete();
            }
            if(refreshCache)
                RefreshCache();
        }

        public void RefreshCache()
        {
            HttpRuntime.Cache.Remove(CacheKey);
            LoadFromDb();
        }

        private IEnumerable<PageNotFound> ConfiguredPages
        {
            get
            {
                var us = (IEnumerable<PageNotFound>)HttpRuntime.Cache[CacheKey] ?? LoadFromDb();
                return us;
            }
        }

        private IEnumerable<PageNotFound> LoadFromDb()
        {
            using (var scope = scopeProvider.CreateScope())
            {
                var db = scope.Database;
                var sql = new Sql().Select("*").From(PageNotFound.TableName);
                var pages = db.Fetch<PageNotFound>(sql);
                HttpRuntime.Cache.Insert(CacheKey, pages);
                scope.Complete();
                return pages;
            }
        }
    }
}
