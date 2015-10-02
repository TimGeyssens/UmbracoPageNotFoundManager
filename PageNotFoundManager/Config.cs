using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Xml;
using umbraco.BusinessLogic;
using UC = Umbraco.Core;

namespace PageNotFoundManager
{
    public class Config
    {
        public const string PluginFolder = "~/App_plugins/PageNotFoundManager";

        public const string ConfigFileName = "PageNotFoundManager.config";

        public static int GetNotFoundPage(int parentId)
        {
            var x = ConfigFile.SelectSingleNode(string.Format("//notFoundPage [@parent = '{0}']", parentId));
            return x != null ? Convert.ToInt32(x.InnerText) : 0;
        }

        public static void SetNotFoundPage(int parentId, int pageNotFoundId)
        {
            var x = ConfigFile.SelectSingleNode(string.Format("//notFoundPage [@parent = '{0}']", parentId.ToString()));

            if (x == null)
            {
                x = ConfigFile.CreateElement("notFoundPage");
                var a = ConfigFile.CreateAttribute("parent");
                a.Value = parentId.ToString();
                x.Attributes.Append(a);
                ConfigFile.DocumentElement.AppendChild(x);
            }

            x.InnerText = pageNotFoundId.ToString();

            ConfigFile.Save(HostingEnvironment.MapPath(PluginFolder + "/" + ConfigFileName));

            HttpRuntime.Cache.Remove("pageNotFoundManagerSettingsFile");
            EnsureConfig();

        }

       

        public static XmlDocument ConfigFile
        {
            get
            {
                var us = (XmlDocument)HttpRuntime.Cache["pageNotFoundManagerSettingsFile"] ?? EnsureConfig();
                return us;
            }
        }

        private static XmlDocument EnsureConfig()
        {
            var settingsFile = HttpRuntime.Cache["pageNotFoundManagerSettingsFile"];
            var fullPath = HostingEnvironment.MapPath(PluginFolder + "/" + ConfigFileName);

            if (settingsFile == null)
            {
                var temp = new XmlDocument();
                var settingsReader = new XmlTextReader(fullPath);
                try
                {
                    temp.Load(settingsReader);
                    HttpRuntime.Cache.Insert("pageNotFoundManagerSettingsFile", temp, new CacheDependency(fullPath));
                }
                catch (XmlException e)
                {
                    throw new XmlException("Your PageNotFoundManager.config file fails to pass as valid XML. Refer to the InnerException for more information", e);
                }
                catch (Exception e)
                {
                    
                    UC.Logging.LogHelper.Error(typeof(Config), e.Message, e);

                }
                settingsReader.Close();
                return temp;
            }
            return (XmlDocument)settingsFile;
        }
    }
}