using System.Collections.Generic;
using F1PCO.Web.Models;

namespace F1PCO.Web.Integration.F1
{
    public interface IF1PersonRepository
    {
        IEnumerable<F1Person> GetPeople();
    }
}