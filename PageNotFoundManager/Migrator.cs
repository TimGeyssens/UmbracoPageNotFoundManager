using PageNotFoundManager.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Xml.Linq;
using Umbraco.Core.Persistence;
using UC = Umbraco.Core;

namespace PageNotFoundManager
{
    internal class Migrator
    {
        private const string ConfigFilePath = "~/App_plugins/PageNotFoundManager/PageNotFoundManager.config";

        private UmbracoDatabase _database;

        public Migrator(UmbracoDatabase database)
        {
            _database = database;
        }

        public void Start()
        {
            var fullPath = HostingEnvironment.MapPath(ConfigFilePath);
            if (!File.Exists(fullPath))
            {
                return;
            }

            try
            {
                var oldNodes = ReadConfig(fullPath);
                if (oldNodes.Count() > 0)
                {
                    _database.BulkInsertRecords(oldNodes);
                }
            }
            catch (Exception e)
            {
                UC.Logging.LogHelper.Error(typeof(Migrator), e.Message, e);
            }
        }

        private IEnumerable<PageNotFound> ReadConfig(string fullPath)
        {
            var configuredNodes = new List<PageNotFound>();
            var xDoc = XDocument.Load(fullPath);
            var pages = from page in xDoc.Descendants("notFoundPage")
                        select new PageNotFound
                        {
                             ParentId = (int)page.Attribute("parent"),
                             NotFoundPageId = int.Parse(page.Value)
                        };
            return pages.ToList();
        }
    }
}