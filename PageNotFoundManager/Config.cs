using NPoco;
using PageNotFoundManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Scoping;
using Umbraco.Web;

namespace PageNotFoundManager
{
    public class Config : IPageNotFoundManagerConfig
    {
        private const string CacheKey = "pageNotFoundManagerConfig";
        private readonly IScopeProvider scopeProvider;
        private readonly IUmbracoContextFactory umbracoContextFactory;

        public Config(IScopeProvider scopeProvider, IUmbracoContextFactory umbracoContextFactory)
        {
            this.scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            this.umbracoContextFactory = umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
        }

        public int GetNotFoundPage(int parentId)
        {
            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
            {
                var parentNode = umbracoContext.UmbracoContext.Content.GetById(parentId);
                return parentNode != null ? GetNotFoundPage(parentNode.Key) : 0;
            }   
        }

        public int GetNotFoundPage(Guid parentKey)
        {
            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
            {
                var x = ConfiguredPages.FirstOrDefault(p => p.ParentId == parentKey);
                var page = x != null ? umbracoContext.UmbracoContext.Content.GetById(x.NotFoundPageId) : null;
                return page != null ? page.Id : 0;
            }
        }

        public void SetNotFoundPage(int parentId, int pageNotFoundId, bool refreshCache)
        {
            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
            {
                var parentPage = umbracoContext.UmbracoContext.Content.GetById(parentId);
                var pageNotFoundPage = umbracoContext.UmbracoContext.Content.GetById(pageNotFoundId);
                SetNotFoundPage(parentPage.Key, pageNotFoundPage.Key, refreshCache);   
            }
        }

        public void SetNotFoundPage(Guid parentKey, Guid pageNotFoundKey, bool refreshCache)
        {
            
            using (var scope = scopeProvider.CreateScope())
            {
                var db = scope.Database;
                var page = db.Query<PageNotFound>().Where(p => p.ParentId == parentKey).FirstOrDefault();
                if (page == null)
                {
                    // create the page
                    db.Insert<PageNotFound>(new PageNotFound { ParentId = parentKey, NotFoundPageId = pageNotFoundKey });
                }
                else
                {
                    // update the existing page
                    page.NotFoundPageId = pageNotFoundKey;
                    db.Update(PageNotFound.TableName, "ParentId", page);
                }
                scope.Complete();
            }
            
            if (refreshCache)
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
