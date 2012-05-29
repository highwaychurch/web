using System.Collections.Generic;

namespace F1PCO.Integration.PCO
{
    public interface IPCOPersonRepository
    {
        IEnumerable<PCOPerson> GetPeople();
        IEnumerable<PCOPerson> SearchByName(string searchTerm);
    }
}