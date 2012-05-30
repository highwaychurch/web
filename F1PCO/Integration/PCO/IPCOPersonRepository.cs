using System.Collections.Generic;
using System.Threading.Tasks;

namespace F1PCO.Integration.PCO
{
    public interface IPCOPersonRepository
    {
        Task<IEnumerable<PCOPerson>> SearchByNameAsync(string searchTerm);
    }
}