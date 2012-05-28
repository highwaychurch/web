using System.Collections.Generic;
using F1PCO.Web.Models;

namespace F1PCO.Web.Integration.PCO
{
    public interface IPCOPersonRepository
    {
        IEnumerable<PCOPerson> GetPeople();
    }
}