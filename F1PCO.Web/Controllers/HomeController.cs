using System.Linq;
using System.Web.Mvc;
using F1PCO.Web.Models;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(IDocumentSession session)
        {
            if (session.Query<PersistedF1Token>().Any() && session.Query<PersistedPCOToken>().Any())
            {
                return RedirectToAction("Authenticate", "F1Auth");
            }

            return View();
        }

        public ActionResult Ready()
        {
            return View();
        }
    }
}
