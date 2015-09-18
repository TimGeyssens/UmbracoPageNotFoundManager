using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace PageNotFoundManager
{
    public class PageNotFoundContentFinder : IContentFinder
    {
        public bool TryFindContent(PublishedContentRequest contentRequest)
        {
            
            var uri = contentRequest.Uri.GetAbsolutePathDecoded();
            var closestContent = UmbracoContext.Current.ContentCache.GetByRoute(uri.ToString());
            while (closestContent == null)
            {
                uri = uri.Remove(uri.Length - 1, 1);
                closestContent = UmbracoContext.Current.ContentCache.GetByRoute(uri.ToString());
            }
            var nfp = Config.GetNotFoundPage(closestContent.Id);

            while (nfp == 0)
            {
                closestContent = closestContent.Parent;

                if (closestContent == null) return false;

                nfp = Config.GetNotFoundPage(closestContent.Id);
            }

            var content = UmbracoContext.Current.ContentCache.GetById(nfp);
            if(content == null) return false;

            contentRequest.PublishedContent = content;
            return true;

           
        }

       

    }
}