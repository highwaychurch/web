namespace F1PCO.Web.ViewModels.PCOPeople
{
    public class PersonMergeViewModel
    {
        public PersonMergeViewModel(PCOPerson personFromF1, PCOPerson lastSeenPersonFromPCO)
        {
            PersonFromPCO = personFromF1;
            LastSeenPersonFromPCO = lastSeenPersonFromPCO;
        }

        public PCOPerson PersonFromPCO { get; set; }
        public PCOPerson LastSeenPersonFromPCO { get; set; }
    }
}