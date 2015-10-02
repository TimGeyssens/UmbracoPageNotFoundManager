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
      // a route is "/path/to/page" when there is no domain, and "123/path/to/page" when there is a domain, and then 123 is the ID of the node which is the root of the domain
           //get domain name from Uri
            // find umbraco home node for uri's domain, and get the id of the node it is set on
            var ds = ApplicationContext.Current.Services.DomainService;
            var domains = ds.GetAll(true) as IList<IDomain> ?? ds.GetAll(true).ToList();
            var domainRoutePrefixId = String.Empty;
            if (domains.Any())
            {
                // a domain is set, so I think we need to prefix the request to GetByRoute by the id of the node it is attached to.
                // I guess if the Uri contains one of these, lets use it's RootContentid as a prefix for the subsequent calls to GetByRoute...
                var domain = domains.FirstOrDefault(d => (contentRequest.Uri.Authority.ToLower() + contentRequest.Uri.AbsolutePath.ToLower())
                                .StartsWith(d.DomainName.ToLower()));
                if (domain != null)
                {
                    // the domain has a RootContentId that we can use as the prefix.
                    domainRoutePrefixId = domain.RootContentId.ToString();
                }
            }
            var closestContent = UmbracoContext.Current.ContentCache.GetByRoute(domainRoutePrefixId + uri.ToString(),false);
            while (closestContent == null)
            {
                uri = uri.Remove(uri.Length - 1, 1);
                closestContent = UmbracoContext.Current.ContentCache.GetByRoute(domainRoutePrefixId + uri.ToString(), false);
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
