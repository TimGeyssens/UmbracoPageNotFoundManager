using Microsoft.Web.XmlTransform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using umbraco.cms.businesslogic.packager.standardPackageActions;
using umbraco.interfaces;

namespace PageNotFoundManager.Umbraco.Installer
{
    public class PackageActions
    {
        public class TransformConfig: IPackageAction
        {

            public string Alias()
            {
                return "PNFM.TransformConfig";
            }

            public bool Execute(string packageName, System.Xml.XmlNode xmlData)
            {
                

                //The config file we want to modify
                var file = xmlData.Attributes.GetNamedItem("file").Value;
                string sourceDocFileName = VirtualPathUtility.ToAbsolute(file);
                
                //The xdt file used for tranformation 
                var xdtfile = xmlData.Attributes.GetNamedItem("xdtfile").Value;
                string xdtFileName = VirtualPathUtility.ToAbsolute(xdtfile);
                
                // The translation at-hand
                using (var xmlDoc = new XmlTransformableDocument())
                {
                    xmlDoc.PreserveWhitespace = true;
                    xmlDoc.Load(HttpContext.Current.Server.MapPath(sourceDocFileName));

                    using (var xmlTrans = new XmlTransformation(HttpContext.Current.Server.MapPath(xdtFileName)))
                    {
                        if (xmlTrans.Apply(xmlDoc))
                        {
                            // If we made it here, sourceDoc now has transDoc's changes
                            // applied. So, we're going to save the final result off to
                            // destDoc.
                            xmlDoc.Save(HttpContext.Current.Server.MapPath(sourceDocFileName));
                        }
                    }
                }

                return true;
            }

            public System.Xml.XmlNode SampleXml()
            {
                string str = "<Action runat=\"install\" undo=\"false\" alias=\"PNFM.TransformConfig\" file=\"~/web.config\" xdtfile=\"~/app_plugins/demo/web.config.xdt\">" +
                         "</Action>";
                return helper.parseStringToXmlNode(str);
            }

            public bool Undo(string packageName, System.Xml.XmlNode xmlData)
            {
                return false;
            }
        }
    }
}