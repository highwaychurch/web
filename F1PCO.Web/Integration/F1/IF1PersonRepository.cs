using System.Collections.Generic;

namespace F1PCO.Web.Integration.F1
{
    public interface IF1PersonRepository
    {
        IEnumerable<F1Person> SearchByName(string searchTerm, int maxResults = 10);
    }
}