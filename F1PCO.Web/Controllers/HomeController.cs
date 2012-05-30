using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Raven.Client;
using Raven.Client.Linq;

namespace F1PCO.Web.Controllers
{
    public class HomeController : AsyncController
    {
        public async Task<ActionResult> Index(IAsyncDocumentSession asyncSession)
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            var mike = (await asyncSession.Query<User>().Take(1).ToListAsync()).FirstOrDefault();
            if (mike == null)
            {
                mike = new User {FirstName = "Mike", LastName = "Noonan"};
                asyncSession.Store(mike);
                await asyncSession.SaveChangesAsync();
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
