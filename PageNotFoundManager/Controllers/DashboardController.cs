using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using PageNotFoundManager.Models;
using Umbraco.Core.Models;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace PageNotFoundManager.Controllers
{
    [PluginController("PageNotFoundManager")]
    public class DashboardController : UmbracoAuthorizedJsonController  
    {

        //public IEnumerable<Models.Language> GetAllLanguages()
        //{
        //    var langs = new List<Models.Language>();

        //    var dl = new Models.Language {Name = "Default", IsoCode = "default"};

        //    var nfp = Config.GetLanguageNotFoundPage("default");
        //    var temp = 0;
        //    if (int.TryParse(nfp, out temp))
        //        dl.NodeRedirect = temp;
        //    else
        //        dl.XPathRedirect = nfp;

        //    langs.Add(dl);

        //    foreach (var lang in Services.LocalizationService.GetAllLanguages())
        //    {
        //        var l = new Models.Language {Name = lang.CultureInfo.DisplayName, IsoCode = lang.IsoCode};

        //        nfp = Config.GetLanguageNotFoundPage(lang.IsoCode);
        //        temp = 0;
        //        if (int.TryParse(nfp, out temp))
        //            l.NodeRedirect = temp;
        //        else
        //            l.XPathRedirect = nfp;

        //        langs.Add(l);
        //    }

        //    return langs;
        //}

        //public void PostSave(IEnumerable<Models.Language> languages)
        //{
        //    Config.SaveLanguageNotFoundPages(languages);
        //}

        public int GetNotFoundPage(int pageId)
        {
            return Config.GetNotFoundPage(pageId);
        }
        [HttpPost]
        public void SetNotFoundPage(PageNotFound pnf)
        {
            Config.SetNotFoundPage(pnf.ParentId, pnf.NotFoundPageId);
        }
    }
}