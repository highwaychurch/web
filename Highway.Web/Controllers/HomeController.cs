using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Highway.Shared.Diagnostics;
using Highway.Shared.Time;
using Raven.Client;

namespace Highway.Web.Controllers
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
            ViewBag.Message = "Welcome to Highway Christian Church!";

            return View();
        }

    }
}
