using System.Collections.Generic;
using F1PCO.Integration.PCO;

namespace F1PCO.Web.ViewModels.PCOPeople
{
    public class SearchByNameViewModel
    {
        public string SearchTerm { get; set; }
        public IEnumerable<PCOPerson> MatchingPeople { get; set; }

        public SearchByNameViewModel(string searchTerm, IEnumerable<PCOPerson> matchingPeople)
        {
            SearchTerm = searchTerm;
            MatchingPeople = matchingPeople;
        }
    }
}