using System.Linq;
using System.Web.Mvc;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(IDocumentSession session)
        {
            var mike = session.Query<User>().FirstOrDefault();
            if (mike == null)
            {
                mike = new User {FirstName = "Mike", LastName = "Noonan"};
                session.Store(mike);
            }
            if (mike.F1AccessToken == null || mike.PCOAccessToken == null)
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
