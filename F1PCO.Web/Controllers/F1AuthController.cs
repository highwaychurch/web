using System.Web.Mvc;
using F1PCO.Web.Integration.F1;

namespace F1PCO.Web.Controllers
{
    public class F1AuthController : Controller
    {
        private readonly IF1AuthorizationService _f1AuthorizationService;

        public F1AuthController(IF1AuthorizationService f1AuthorizationService)
        {
            _f1AuthorizationService = f1AuthorizationService;
        }

        public ActionResult Authenticate()
        {
            if (_f1AuthorizationService.IsAuthorized)
            {
                return RedirectToAction("Ready");
            }
            
            var callbackUrl = Url.Action("CallBack", "F1Auth", null, Request.Url.Scheme);
            return Redirect(_f1AuthorizationService.BuildPortalUserAuthorizationRequestUrl(callbackUrl));
        }

        public ActionResult CallBack()
        {
            _f1AuthorizationService.RequestAndPersistAccessToken();
            return RedirectToAction("Ready");
        }

        public ActionResult Ready()
        {
            return View();
        }
    }
}
