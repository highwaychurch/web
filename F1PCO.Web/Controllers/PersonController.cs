using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using F1PCO.Web.Integration.F1;
using F1PCO.Web.Models;
using F1PCO.Web.ViewModels.Person;
using Raven.Client;

namespace F1PCO.Web.Controllers
{
    public class PersonController : Controller
    {
        private readonly IF1PersonRepository _f1PersonRepository;

        public PersonController(IF1PersonRepository f1PersonRepository)
        {
            _f1PersonRepository = f1PersonRepository;
        }

        public ActionResult Index(IDocumentSession documentSession)
        {
            var f1People = _f1PersonRepository.GetPeople().ToArray();
            var f1PeopleIds = f1People.Select(p => p.F1ID).ToArray();

            var previouslySeenPeople = documentSession.Advanced.LuceneQuery<F1Person>()
                   .Where(string.Format("F1ID:({0})", string.Join(" OR ", f1PeopleIds)));
            var previouslySeenPeopleIds = previouslySeenPeople.Select(p => p.F1ID).ToArray();


            var newPeople = f1People.Where(f1p => previouslySeenPeopleIds.Contains(f1p.F1ID) == false).ToArray();
            foreach (var newPerson in newPeople)
            {
                documentSession.Store(newPerson);
            }

            var viewModel = BuildViewModel(f1People, previouslySeenPeople);

            return View(viewModel);
        }

        public ActionResult ExistingPeople(IDocumentSession documentSession)
        {
            var people = documentSession.Query<F1Person>().Where(p => p.DateOfBirth > new DateTime(1990, 1, 1));
            return View(people);
        }

        private IndexViewModel BuildViewModel(IEnumerable<F1Person> f1People, IEnumerable<F1Person> previouslySeenPeople)
        {
            return new IndexViewModel(f1People.Select(f1p => new PersonMergeViewModel(f1p, previouslySeenPeople.FirstOrDefault(p => p.F1ID == f1p.F1ID))).ToArray());
        }
    }
}
