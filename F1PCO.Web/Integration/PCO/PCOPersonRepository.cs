using System;
using System.Collections.Generic;
using System.Net;
using F1PCO.Web.Models;
using Hammock;

namespace F1PCO.Web.Integration.PCO
{
    public class PCOPersonRepository : IPCOPersonRepository
    {
        private readonly IPCOClientProvider _clientProvider;

        public PCOPersonRepository(IPCOClientProvider clientProvider)
        {
            _clientProvider = clientProvider;
        }

        public IEnumerable<PCOPerson> GetPeople()
        {
            var request = new RestRequest
            {
                Path = "people.xml",
            };

            using (var response = _clientProvider.GetRestClient().Request(request))
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var people = GetPCOPeopleFromXml(response.Content);
                    return people;
                }

                throw new Exception("An error occured: Status code: " + response.StatusCode, response.InnerException);
            }
        }

        private static IEnumerable<PCOPerson> GetPCOPeopleFromXml(string xml)
        {
            yield return new PCOPerson();
        }
    }
}