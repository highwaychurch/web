using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using F1PCO.Integration.PCO;
using F1PCO.Web.ViewModels.PCOPeople;

namespace F1PCO.Web.Controllers
{
    public class PCOPeopleController : AsyncController
    {
        private readonly IPCOPersonRepository _pcoPersonRepository;

        public PCOPeopleController(IPCOPersonRepository pcoPersonRepository)
        {
            _pcoPersonRepository = pcoPersonRepository;
        }

        public async Task<ActionResult> SearchByName(string searchTerm)
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            IEnumerable<PCOPerson> matchingPeople = new PCOPerson[] { };
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                matchingPeople = await _pcoPersonRepository.SearchByNameAsync(searchTerm);
            }
            return View(new SearchByNameViewModel(searchTerm, matchingPeople));
        }
    }
}
