using System.Linq;
using System.Web.Mvc;
using Highway.Identity.Web.Models.Account;
using Highway.Shared.Mvc.FlashMessages;
using Raven.Client;

namespace Highway.Identity.Web.Controllers
{
    public class AccountController : Controller
    {
        [Authorize]
        public ActionResult MyAccount(IDocumentSession session)
        {
            var account = session.Query<MyAccountModel>().FirstOrDefault();
            if (account == null) account = new MyAccountModel();
            return View(account);
        }

        [Authorize]
        [HttpPost]
        public ActionResult MyAccount(MyAccountModel model, IDocumentSession session)
        {
            if (ModelState.IsValid == false)
            {
                TempData.FlashError("Whoops!", "There seems to be some problems with the information you provided. Please correct the errors and try again.");
                return View();
            }

            var account = session.Query<MyAccountModel>().FirstOrDefault();
            if (account == null)
            {
                account = new MyAccountModel();
                session.Store(account);
            }

            account.FirstName = model.FirstName;

            TempData.FlashSuccess("Your changes have been saved");

            return View("MyAccount", model);
        }

    }
}
