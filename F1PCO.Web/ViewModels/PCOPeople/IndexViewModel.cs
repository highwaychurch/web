using System.Collections.Generic;

namespace F1PCO.Web.ViewModels.PCOPeople
{
    public class IndexViewModel
    {
        public IndexViewModel(IEnumerable<PersonMergeViewModel> merges)
        {
            Merges = merges;
        }

        public IEnumerable<PersonMergeViewModel> Merges { get; set; }
    }
}