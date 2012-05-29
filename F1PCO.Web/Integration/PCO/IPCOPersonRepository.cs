using System.Collections.Generic;

namespace F1PCO.Web.Integration.PCO
{
    public interface IPCOPersonRepository
    {
        IEnumerable<PCOPerson> GetPeople();
        IEnumerable<PCOPerson> SearchByName(string searchTerm);
    }
}