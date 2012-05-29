using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using RestSharp;

namespace F1PCO.Integration.PCO
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
            var request = new RestRequest("people.xml");
            var response = _clientProvider.GetRestClient().Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var people = GetPCOPeopleFromXml(response.Content);
                return people;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
        }

        public IEnumerable<PCOPerson> SearchByName(string searchTerm)
        {
            var request = new RestRequest("people.xml");
            request.AddParameter("name", searchTerm);
            var response = _clientProvider.GetRestClient().Execute(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var people = GetPCOPeopleFromXml(response.Content);
                return people;
            }

            throw new Exception("An error occured: Status code: " + response.StatusCode, response.ErrorException);
        }

        private static IEnumerable<PCOPerson> GetPCOPeopleFromXml(string xml)
        {
            var xPeople = XDocument.Parse(xml).Element("people");
            return xPeople.Elements().Select(GetPCOPersonFromXml);
        }

        private static PCOPerson GetPCOPersonFromXml(XElement xPerson)
        {
            return new PCOPerson
                       {
                           PCOID = (string)xPerson.Element("id"),
                           FirstName = (string)xPerson.Element("first-name"),
                           LastName = (string)xPerson.Element("last-name"),
                       };
        }
    }
}