using System;
using System.Configuration;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using Bgt.Ocean.Models;
using Bgt.Ocean.WebAPI.Areas.HelpPage.ModelDescriptions;
using Bgt.Ocean.WebAPI.Areas.HelpPage.Models;

namespace Bgt.Ocean.WebAPI.Areas.HelpPage.Controllers
{
    /// <summary>
    /// The controller that will handle requests for the help page.
    /// </summary>
    public class HelpController : Controller
    {
        private const string ErrorViewName = "Error";

        public HelpController()
            : this(GlobalConfiguration.Configuration)
        {
        }

        public HelpController(HttpConfiguration config)
        {
            Configuration = config;
        }

        public HttpConfiguration Configuration { get; private set; }

        public ActionResult Index()
        {
            using (var db = new OceanDbEntities())
            {
                bool isStagging = Convert.ToBoolean(ConfigurationManager.AppSettings["EnvSTG"]);
                string version =  db.TblSystemVersion.SingleOrDefault(o => o.FlagStagging == isStagging).VersionNumber;
                string env = isStagging ? "STAGGING" : "DBO";
                string versionsStr = "Ocean Online Web API {0} ({1}) is running...";

                ViewBag.DocumentationProvider = Configuration.Services.GetDocumentationProvider();
                ViewBag.OOVersion = string.Format(versionsStr, version, env);
                return View(Configuration.Services.GetApiExplorer().ApiDescriptions);
            }
           
        }

        public ActionResult Api(string apiId)
        {
            if (!String.IsNullOrEmpty(apiId))
            {
                HelpPageApiModel apiModel = Configuration.GetHelpPageApiModel(apiId);
                if (apiModel != null)
                {
                    return View(apiModel);
                }
            }

            return View(ErrorViewName);
        }

        public ActionResult ResourceModel(string modelName)
        {
            if (!String.IsNullOrEmpty(modelName))
            {
                ModelDescriptionGenerator modelDescriptionGenerator = Configuration.GetModelDescriptionGenerator();
                ModelDescription modelDescription;
                if (modelDescriptionGenerator.GeneratedModels.TryGetValue(modelName, out modelDescription))
                {
                    return View(modelDescription);
                }
            }

            return View(ErrorViewName);
        }
    }
}