using System.Collections.Generic;
using F1PCO.Web.Models;

namespace F1PCO.Web.ViewModels.F1People
{
    public class SearchByNameViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<F1Person> MatchingPeople { get; set; }

        public SearchByNameViewModel(string searchTerm, IEnumerable<F1Person> matchingPeople)
        {
            SearchTerm = searchTerm;
            MatchingPeople = matchingPeople;
        }
    }
}