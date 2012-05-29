using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using F1PCO.Integration.F1;
using F1PCO.Web.ViewModels.F1People;

namespace F1PCO.Web.Controllers
{
    public class F1PeopleController : Controller
    {
        private readonly IF1PersonRepository _f1PersonRepository;

        public F1PeopleController(IF1PersonRepository f1PersonRepository)
        {
            _f1PersonRepository = f1PersonRepository;
        }

        public ActionResult SearchByName(string searchTerm)
        {
            IEnumerable<F1Person> matchingPeople = new F1Person[] {};
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                matchingPeople = _f1PersonRepository.SearchByName(searchTerm);
            }
            return View(new SearchByNameViewModel(searchTerm, matchingPeople));
        }
    }
}
