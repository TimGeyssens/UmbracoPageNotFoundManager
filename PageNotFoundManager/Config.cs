using PageNotFoundManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using Umbraco.Core.Persistence;
using Umbraco.Web;

namespace PageNotFoundManager
{
    public class Config
    {

        private const string CacheKey = "pageNotFoundManagerConfig";

        public static int GetNotFoundPage(int parentId)
        {
            var x = ConfiguredPages.FirstOrDefault(p => p.ParentId == parentId);
            return x != null ? x.NotFoundPageId : 0;
        }

        public static void SetNotFoundPage(int parentId, int pageNotFoundId)
        {
            var db = UmbracoContext.Current.Application.DatabaseContext.Database;
            var page = db.FirstOrDefault<PageNotFound>(new Sql().Where<PageNotFound>(p => p.ParentId == parentId));
            if (page == null)
            {
                // create the page
                db.Insert(new PageNotFound { ParentId = parentId, NotFoundPageId = pageNotFoundId });
            }
            else
            {
                // update the existing page
                page.NotFoundPageId = pageNotFoundId;
                db.Update(page);
            }

            RefreshCache();
        }

        public static void RefreshCache()
        {
            HttpRuntime.Cache.Remove(CacheKey);
            LoadFromDb();
        }

        private static IEnumerable<PageNotFound> ConfiguredPages
        {
            get
            {
                var us = (IEnumerable<PageNotFound>)HttpRuntime.Cache[CacheKey] ?? LoadFromDb();
                return us;
            }
        }

        private static IEnumerable<PageNotFound> LoadFromDb()
        {

            var db = UmbracoContext.Current.Application.DatabaseContext.Database;
            var sql = new Sql().Select("*").From(PageNotFound.TableName);
            var pages = db.Fetch<PageNotFound>(sql);
            HttpRuntime.Cache.Insert(CacheKey, pages);
            return pages;

        }

        public static bool RunningOnCloud()
        {
           return AppDomain.CurrentDomain.GetAssemblies()
                            .Any(a => a.FullName.StartsWith("Concorde.Messaging.Web"));
           
        }
    }
}
