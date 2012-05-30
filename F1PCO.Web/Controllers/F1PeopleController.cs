using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using F1PCO.Integration.F1;
using F1PCO.Web.ViewModels.F1People;
using Highway.Shared.Persistence;

namespace F1PCO.Web.Controllers
{
    public class F1PeopleController : AsyncController
    {
        private readonly IF1PersonRepository _f1PersonRepository;

        public F1PeopleController(IF1PersonRepository f1PersonRepository)
        {
            _f1PersonRepository = f1PersonRepository;
        }

        [NoTransaction]
        public async Task<ViewResult> SearchByName(string searchTerm)
        {
            // Remove when MVC 4 is released (http://forums.asp.net/p/1778103/4880898.aspx/1?Re+Using+an+Async+Action+to+Run+Synchronous+Code)
            await Task.Yield();

            IEnumerable<F1Person> matchingPeople = new F1Person[] {};
            if (string.IsNullOrWhiteSpace(searchTerm) == false)
            {
                matchingPeople = await _f1PersonRepository.SearchByNameAsync(searchTerm);
            }
            return View(new SearchByNameViewModel(searchTerm, matchingPeople));
        }
    }
}
