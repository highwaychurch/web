using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using F1PCO.Web.Integration.F1;

namespace F1PCO.Web.Controllers
{
    public class PersonController : Controller
    {
        private readonly IF1PersonRepository _f1PersonRepository;

        public PersonController(IF1PersonRepository f1PersonRepository)
        {
            _f1PersonRepository = f1PersonRepository;
        }

        public ActionResult Index()
        {
            var people = _f1PersonRepository.GetPeople();
            return View(people);
        }
    }
}
