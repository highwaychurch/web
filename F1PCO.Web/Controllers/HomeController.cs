using System.Linq;
using System.Web.Mvc;
using F1PCO.Web.Integration.F1;
using F1PCO.Web.Models;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index(IF1AuthorizationService f1AuthorizationService, IF1PersonRepository personRepository, IDocumentSession documentSession)
        {
            //var persistedF1AccessToken = documentSession.Query<PersistedF1Token>().FirstOrDefault();
            //if (persistedF1AccessToken != null)
            //{
            //    if (f1AuthorizationService.TryAuthorizeWithPersistedAccessToken(persistedF1AccessToken.AccessToken))
            //    {
            //        return RedirectToAction("Ready", "F1Auth");
            //    }
            //}
            return View();
        }

    }
}
