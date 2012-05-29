
namespace F1PCO.Web.ViewModels.F1People
{
    public class PersonMergeViewModel
    {
        public PersonMergeViewModel(F1Person personFromF1, F1Person lastSeenPersonFromF1)
        {
            PersonFromF1 = personFromF1;
            LastSeenPersonFromF1 = lastSeenPersonFromF1;
        }

        public F1Person PersonFromF1 { get; set; }
        public F1Person LastSeenPersonFromF1 { get; set; }
    }
}