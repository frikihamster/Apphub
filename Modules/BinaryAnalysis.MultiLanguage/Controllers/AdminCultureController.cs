using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using BinaryAnalysis.MultiLanguage.Services;
using Orchard;
using Orchard.Localization;
using Orchard.Mvc.Extensions;
using Orchard.UI.Admin;

namespace BinaryAnalysis.MultiLanguage.Controllers
{
    [Admin]
    public class AdminCultureController : Controller
    {
        public IOrchardServices Services { get; set; }
        private readonly IWorkContextAccessor _workContextAccessor;
        public Localizer T { get; set; }

        public AdminCultureController(IOrchardServices services, IWorkContextAccessor workContextAccessor)
        {
            Services = services;
            _workContextAccessor = workContextAccessor;
        }

        public ActionResult Index() {
            return View();
        }
    }
}
