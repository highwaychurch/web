using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1PCO.Integration.F1
{
    public interface IF1PersonRepository
    {
        Task<IEnumerable<F1Person>> SearchByNameAsync(string searchTerm, int maxResults = 10);
    }
}