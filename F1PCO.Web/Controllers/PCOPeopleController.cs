using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using F1PCO.Web.Integration.PCO;
using F1PCO.Web.ViewModels.PCOPeople;
using Raven.Client;

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

        public ActionResult Index(IDocumentSession documentSession)
        {
            var pcoPeople = _pcoPersonRepository.GetPeople().ToArray();
            var pcoPeopleIds = pcoPeople.Select(p => p.PCOID).ToArray();

            var previouslySeenPeople = documentSession.Advanced.LuceneQuery<PCOPerson>()
                   .Where(string.Format("PCOID:({0})", string.Join(" OR ", pcoPeopleIds)));
            var previouslySeenPeopleIds = previouslySeenPeople.Select(p => p.PCOID).ToArray();


            var newPeople = pcoPeople.Where(PCOp => previouslySeenPeopleIds.Contains(PCOp.PCOID) == false).ToArray();
            foreach (var newPerson in newPeople)
            {
                documentSession.Store(newPerson);
            }

            var viewModel = BuildViewModel(pcoPeople, previouslySeenPeople);

            return View(viewModel);
        }

        public ActionResult ExistingPeople(IDocumentSession documentSession)
        {
            var people = documentSession.Query<PCOPerson>().Where(p => p.DateOfBirth > new DateTime(1990, 1, 1));
            return View(people);
        }

        private IndexViewModel BuildViewModel(IEnumerable<PCOPerson> pcoPeople, IEnumerable<PCOPerson> previouslySeenPeople)
        {
            return new IndexViewModel(pcoPeople.Select(PCOp => new PersonMergeViewModel(PCOp, previouslySeenPeople.FirstOrDefault(p => p.PCOID == PCOp.PCOID))).ToArray());
        }
    }
}
