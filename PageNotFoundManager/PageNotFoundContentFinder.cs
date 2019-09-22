using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Routing;

namespace PageNotFoundManager
{
    public class PageNotFoundContentFinder : IContentLastChanceFinder
    {
        private readonly IDomainService domainService;
        private readonly IUmbracoContextFactory umbracoContextFactory;
        private readonly IPageNotFoundManagerConfig config;

        public PageNotFoundContentFinder(IDomainService domainService, IUmbracoContextFactory umbracoContextFactory, IPageNotFoundManagerConfig config)
        {
            this.domainService = domainService ?? throw new ArgumentNullException(nameof(domainService));
            this.umbracoContextFactory = umbracoContextFactory ?? throw new ArgumentNullException(nameof(umbracoContextFactory));
            this.config = config ?? throw new ArgumentNullException(nameof(config));
        }
        public bool TryFindContent(PublishedRequest contentRequest)
        {

            var uri = contentRequest.Uri.GetAbsolutePathDecoded();
            // a route is "/path/to/page" when there is no domain, and "123/path/to/page" when there is a domain, and then 123 is the ID of the node which is the root of the domain
            //get domain name from Uri
            // find umbraco home node for uri's domain, and get the id of the node it is set on
            
            var domains = domainService.GetAll(true) as IList<IDomain> ?? domainService.GetAll(true).ToList();
            var domainRoutePrefixId = String.Empty;
            if (domains.Any())
            {
                // a domain is set, so I think we need to prefix the request to GetByRoute by the id of the node it is attached to.
                // I guess if the Uri contains one of these, lets use it's RootContentid as a prefix for the subsequent calls to GetByRoute...

                //A domain can be defined with or without http(s) so we neet to check for both cases.
                IDomain domain = null;
                foreach (IDomain currentDomain in domains)
                {
                    if (currentDomain.DomainName.ToLower().StartsWith("http") && contentRequest.Uri.AbsoluteUri.ToLower().StartsWith(currentDomain.DomainName.ToLower()))
                    {
                        domain = currentDomain;
                        break;
                    }
                    else if((contentRequest.Uri.Authority.ToLower() + contentRequest.Uri.AbsolutePath.ToLower()).StartsWith(currentDomain.DomainName.ToLower())) {
                        domain = currentDomain;
                        break;
                    }
                }

                if (domain != null)
                {
                    // the domain has a RootContentId that we can use as the prefix.
                    domainRoutePrefixId = domain.RootContentId.ToString();
                }
            }
            using (var umbracoContext = umbracoContextFactory.EnsureUmbracoContext())
            {
                var closestContent = umbracoContext.UmbracoContext.Content.GetByRoute(domainRoutePrefixId + uri.ToString(), false, culture: contentRequest?.Culture?.Name);
                while (closestContent == null)
                {
                    uri = uri.Remove(uri.Length - 1, 1);
                    closestContent = umbracoContext.UmbracoContext.Content.GetByRoute(domainRoutePrefixId + uri.ToString(), false, culture: contentRequest?.Culture?.Name);
                }
                var nfp = config.GetNotFoundPage(closestContent.Id);

                while (nfp == 0)
                {
                    closestContent = closestContent.Parent;

                    if (closestContent == null) return false;

                    nfp = config.GetNotFoundPage(closestContent.Id);
                }

                var content = umbracoContext.UmbracoContext.Content.GetById(nfp);
                if (content == null) return false;

                contentRequest.SetResponseStatus(404);
                contentRequest.PublishedContent = content;
                return true;
            }

        }

    }
}
