using System.Collections.Generic;
using System.Web.Mvc;
using F1PCO.Integration.PCO;
using F1PCO.Web.ViewModels.PCOPeople;

namespace F1PCO.Web.Controllers
{
    public class PCOPeopleController : Controller
    {
        private readonly IPCOPersonRepository _pcoPersonRepository;

        public PCOPeopleController(IPCOPersonRepository pcoPersonRepository)
        {
            _pcoPersonRepository = pcoPersonRepository;
        }

        public ActionResult SearchByName(string searchTerm)
        {
            IEnumerable<PCOPerson> matchingPeople = new PCOPerson[] { };
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                matchingPeople = _pcoPersonRepository.SearchByName(searchTerm);
            }
            return View(new SearchByNameViewModel(searchTerm, matchingPeople));
        }
    }
}
