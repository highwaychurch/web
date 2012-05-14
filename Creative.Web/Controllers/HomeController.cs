using System;
using System.Web.Mvc;
using Highway.Shared.Diagnostics;
using Highway.Shared.Time;
using Raven.Client;

namespace Creative.Web.Controllers
{
    public class HomeController : Controller
    {
        readonly ILog<HomeController> _log;

        public HomeController(ILog<HomeController> log)
        {
            if (log == null) throw new ArgumentNullException("log");
            _log = log;
        }

        public ActionResult Index(IClock clock, IDocumentSession session)
        {
            if (clock == null) throw new ArgumentNullException("clock");

            return View();
        }

    }
}
