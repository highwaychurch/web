using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Http;
using F1PCO.Integration.F1;

namespace F1PCO.Web.Controllers
{
    public class F1SearchController : ApiController
    {
        private readonly IF1PersonRepository _f1PersonRepository;

        public F1SearchController(IF1PersonRepository f1PersonRepository)
        {
            _f1PersonRepository = f1PersonRepository;
        }

        public async Task<IEnumerable<F1Person>> GetPeople(string searchTerm)
        {
            return await _f1PersonRepository.SearchByNameAsync(searchTerm);
        }
    }
}