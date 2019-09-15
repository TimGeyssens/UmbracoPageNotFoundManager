using Microsoft.Web.XmlTransform;
using System.Web;
using System.Xml.Linq;
using PA = Umbraco.Core.PackageActions;

namespace PageNotFoundManager.Umbraco.Installer
{
    public class PackageActions
    {
        public class TransformConfig : PA.IPackageAction
        {

            public TransformConfig()
            {

            }
            public string Alias()
            {
                return "PNFM.TransformConfig";
            }

            private bool Transform(string packageName, XElement xmlData, bool uninstall = false)
            {
                //The config file we want to modify
                var file = xmlData.Attribute("file").Value;

                string sourceDocFileName = VirtualPathUtility.ToAbsolute(file);

                //The xdt file used for tranformation 
                //var xdtfile = xmlData.Attributes.GetNamedItem("xdtfile").Value;
                var fileEnd = "install.xdt";
                if (uninstall)
                {
                    fileEnd = string.Format("un{0}", fileEnd);
                }

                var xdtfile = string.Format("{0}.{1}", xmlData.Attribute("xdtfile").Value, fileEnd);
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



            

            public bool Undo(string packageName, XElement xmlData)
            {
                return Transform(packageName, xmlData, true);
            }

            public bool Execute(string packageName, XElement xmlData)
            {
                return Transform(packageName, xmlData);
            }
        }
    }
}