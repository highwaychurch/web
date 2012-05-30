using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using F1PCO.Integration.PCO;

namespace F1PCO.Web.Controllers
{
    public class PCOSearchController : ApiController
    {
        private readonly IPCOPersonRepository _pcoPersonRepository;

        public PCOSearchController(IPCOPersonRepository pcoPersonRepository)
        {
            _pcoPersonRepository = pcoPersonRepository;
        }

        public async Task<IEnumerable<PCOPerson>> GetPeople(string searchTerm)
        {
            return await _pcoPersonRepository.SearchByNameAsync(searchTerm);
        }
    }
}